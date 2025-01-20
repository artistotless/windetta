using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http.Features;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Text;
using Windetta.Web.Clients;

namespace Windetta.Main.Infrastructure.Middlewares;

public class ProxyMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger _logger;
    private readonly IHttpClientFactory _httpClientFactory;

    public ProxyMiddleware(
        RequestDelegate next,
        ILogger<ProxyMiddleware> logger,
        IHttpClientFactory httpClientFactory)
    {
        _next = next;
        _logger = logger;
        _httpClientFactory = httpClientFactory;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            if (!context.Request.Path.StartsWithSegments("/api"))
                await _next.Invoke(context);
            else
            {
                var authResult = await context.AuthenticateAsync
                        (CookieAuthenticationDefaults.AuthenticationScheme).ConfigureAwait(false);

                using var client = _httpClientFactory.CreateClient(ClientsNames.MainClient);

                if (client is null)
                    throw new Exception("Http proxy client not registered");

                if (client.BaseAddress is null)
                    throw new Exception("Base address not defined in Http proxy client");

                if (authResult.Succeeded)
                {
                    var accessToken = authResult.Properties.GetTokenValue("access_token");
                    if (accessToken is not null)
                        client.SetBearerToken(accessToken);
                }

                var endpoint = TrimTrailingSlashes
                    ($"{TrimTrailingSlashes(client.BaseAddress.OriginalString)}{context.Request.Path}{context.Request.QueryString}");

                var proxiedRequest = CreateProxiedHttpRequest(context, endpoint, true);

                var proxiedResponse = await client.SendAsync(
                    proxiedRequest,
                    HttpCompletionOption.ResponseHeadersRead,
                    context.RequestAborted)
                    .ConfigureAwait(false);

                await WriteProxiedHttpResponseAsync(context, proxiedResponse);
            }
        }
        catch (Exception e)
        {
            context.Response.StatusCode = 500;
            await context.Response.WriteAsync("Uknown server error");
            _logger.LogError(e, "Server Error {0}", e.Message);
        }
    }

    private static HttpRequestMessage CreateProxiedHttpRequest(HttpContext context, string uriString, bool shouldAddForwardedHeaders)
    {
        var uri = new Uri(uriString);
        var request = context.Request;

        var requestMessage = new HttpRequestMessage();
        var requestMethod = request.Method;
        var usesStreamContent = true; // When using other content types, they specify the Content-Type header, and may also change the Content-Length.

        // Write to request content, when necessary.
        if (!HttpMethods.IsGet(requestMethod) &&
            !HttpMethods.IsHead(requestMethod) &&
            !HttpMethods.IsDelete(requestMethod) &&
            !HttpMethods.IsTrace(requestMethod))
        {
            if (request.HasFormContentType)
            {
                usesStreamContent = false;
                requestMessage.Content = ToHttpContent(request.Form, request);
            }
            else
            {
                requestMessage.Content = new StreamContent(request.Body);
            }
        }

        // Copy the request headers.
        foreach (var header in request.Headers)
        {
            if (!usesStreamContent && (header.Key.Equals("Content-Type", StringComparison.OrdinalIgnoreCase) || header.Key.Equals("Content-Length", StringComparison.OrdinalIgnoreCase)))
                continue;
            if (!requestMessage.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray()))
                requestMessage.Content?.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray());
        }

        // Add forwarded headers.
        if (shouldAddForwardedHeaders)
            AddForwardedHeadersToHttpRequest(context, requestMessage);

        // Set destination and method.
        requestMessage.Headers.Host = uri.Authority;
        requestMessage.RequestUri = uri;
        requestMessage.Method = new HttpMethod(requestMethod);

        return requestMessage;
    }

    private static Task WriteProxiedHttpResponseAsync(HttpContext context, HttpResponseMessage responseMessage)
    {
        var response = context.Response;

        response.StatusCode = (int)responseMessage.StatusCode;

        var httpResponseFeature = context.Features.Get<IHttpResponseFeature>();
        if (httpResponseFeature != null)
        {
            httpResponseFeature.ReasonPhrase = responseMessage.ReasonPhrase;
        }

        foreach (var header in responseMessage.Headers)
        {
            response.Headers[header.Key] = header.Value.ToArray();
        }

        foreach (var header in responseMessage.Content.Headers)
        {
            response.Headers[header.Key] = header.Value.ToArray();
        }

        response.Headers.Remove("transfer-encoding");

        return responseMessage.Content.CopyToAsync(response.Body);
    }

    private static void AddForwardedHeadersToHttpRequest(HttpContext context, HttpRequestMessage requestMessage)
    {
        var request = context.Request;
        var connection = context.Connection;

        var host = request.Host.ToString();
        var protocol = request.Scheme;

        var localIp = connection.LocalIpAddress?.ToString();
        var isLocalIpV6 = connection.LocalIpAddress?.AddressFamily == AddressFamily.InterNetworkV6;

        var remoteIp = context.Connection.RemoteIpAddress?.ToString();
        var isRemoteIpV6 = connection.RemoteIpAddress?.AddressFamily == AddressFamily.InterNetworkV6;

        if (remoteIp != null)
            requestMessage.Headers.TryAddWithoutValidation("X-Forwarded-For", remoteIp);
        requestMessage.Headers.TryAddWithoutValidation("X-Forwarded-Proto", protocol);
        requestMessage.Headers.TryAddWithoutValidation("X-Forwarded-Host", host);

        // Fix IPv6 IPs for the `Forwarded` header.
        var forwardedHeader = new StringBuilder($"proto={protocol};host={host};");

        if (localIp != null)
        {
            if (isLocalIpV6)
                localIp = $"\"[{localIp}]\"";

            forwardedHeader.Append("by=").Append(localIp).Append(';');
        }

        if (remoteIp != null)
        {
            if (isRemoteIpV6)
                remoteIp = $"\"[{remoteIp}]\"";

            forwardedHeader.Append("for=").Append(remoteIp).Append(';');
        }

        requestMessage.Headers.TryAddWithoutValidation("Forwarded", forwardedHeader.ToString());
    }

    private static HttpContent ToHttpContent(IFormCollection collection, HttpRequest request)
    {
        // @PreferLinux:
        // Form content types resource: https://stackoverflow.com/questions/4526273/what-does-enctype-multipart-form-data-mean/28380690
        // There are three possible form content types:
        // - text/plain, which should never be used and this does not handle (a request with that will not have IsFormContentType true anyway)
        // - application/x-www-form-urlencoded, which doesn't handle file uploads and escapes any special characters
        // - multipart/form-data, which does handle files and doesn't require any escaping, but is quite bulky for short data (due to using some content headers for each value, and a boundary sequence between them)

        // A single form element can have multiple values. When sending them they are handled as separate items with the same name, not a singe item with multiple values.
        // For example, a=1&a=2.

        var contentType = MediaTypeHeaderValue.Parse(request.ContentType);

        if (contentType.MediaType.Equals("application/x-www-form-urlencoded", StringComparison.OrdinalIgnoreCase)) // specification: https://url.spec.whatwg.org/#concept-urlencoded
            return new FormUrlEncodedContent(collection.SelectMany(formItemList => formItemList.Value.Select(value => new KeyValuePair<string, string>(formItemList.Key, value))));

        if (!contentType.MediaType.Equals("multipart/form-data", StringComparison.OrdinalIgnoreCase))
            throw new Exception($"Unknown form content type `{contentType.MediaType}`.");

        // multipart/form-data specification https://tools.ietf.org/html/rfc7578
        // It has each value separated by a boundary sequence, which is specified in the Content-Type header.
        // As a proxy it is probably best to reuse the boundary used in the original request as it is not necessarily random.
        var delimiter = contentType.Parameters.Single(p => p.Name.Equals("boundary", StringComparison.OrdinalIgnoreCase)).Value.Trim('"');

        var multipart = new MultipartFormDataContent(delimiter);
        foreach (var formVal in collection)
        {
            foreach (var value in formVal.Value)
                multipart.Add(new StringContent(value), formVal.Key);
        }
        foreach (var file in collection.Files)
        {
            var content = new StreamContent(file.OpenReadStream());
            foreach (var header in file.Headers.Where(h => !h.Key.Equals("Content-Disposition", StringComparison.OrdinalIgnoreCase)))
                content.Headers.TryAddWithoutValidation(header.Key, (IEnumerable<string>)header.Value);

            // Force content-disposition header to use raw string to ensure UTF-8 is well encoded.
            content.Headers.TryAddWithoutValidation("Content-Disposition",
                new string(Encoding.UTF8.GetBytes($"form-data; name=\"{file.Name}\"; filename=\"{file.FileName}\"").
                Select(b => (char)b).ToArray()));

            multipart.Add(content);
        }
        return multipart;
    }

    private static string TrimTrailingSlashes(string url)
    {
        if (!url.EndsWith("/"))
            return url;

        var span = url.AsSpan();
        var count = 0;

        for (int k = span.Length - 1; k >= 0; k--)
        {
            if (url[k] == '/')
                count++;
            else
                break;
        }

        return span.Slice(0, span.Length - count).ToString();
    }
}
