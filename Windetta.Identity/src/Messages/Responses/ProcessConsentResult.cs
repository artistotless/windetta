using Windetta.Identity.Models;

namespace Windetta.Identity.Messages.Responses;

public class ProcessConsentResult
{
    public bool IsRedirect => RedirectUri != null;
    public string RedirectUri { get; set; }
    public bool IsNativeClient { get; set; }
    public bool ShowView => ViewModel != null;
    public ConsentViewModel ViewModel { get; set; }

    public bool HasValidationError => ValidationError != null;
    public string ValidationError { get; set; }
}
