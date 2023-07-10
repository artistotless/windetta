namespace Windetta.Identity.Messages.Responses;

public class LoggedOutResponse
{
    public string PostLogoutRedirectUri { get; set; }
    public string ClientName { get; set; }
    public string SignOutIframeUrl { get; set; }
    public bool AutomaticRedirectAfterSignOut { get; set; }
    public bool IsLocalLogout { get; set; }
}
