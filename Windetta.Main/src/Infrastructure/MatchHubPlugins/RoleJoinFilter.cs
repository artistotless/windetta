using Windetta.Main.MatchHubs.Filters;

namespace Windetta.Main.Infrastructure.MatchHubPlugins;

public class RoleJoinFilter : IJoinFilter<string>
{
    private readonly IHttpContextAccessor _contextAccessor;

    public RoleJoinFilter(IHttpContextAccessor contextAccessor)
    {
        _contextAccessor = contextAccessor;
        Data = "admin";
    }

    public string Data { get; set; }

    public ValueTask<(bool isValid, string? error)> ValidateAsync(Guid userId, CancellationToken token)
    {
        if (_contextAccessor.HttpContext is null)
            return ValueTask.FromResult<(bool, string?)>((false, "HttpContext is not available"));

        if (_contextAccessor.HttpContext.User.IsInRole(Data))
        {
            return ValueTask.FromResult<(bool, string?)>((true, null));
        }

        return ValueTask.FromResult<(bool, string?)>((false, $"Use is not in Role - {Data}"));
    }
}
