using IdentityServer4.Models;

namespace Windetta.Identity.Models;

public class ErrorViewModel
{
    public ErrorMessage Error { get; set; }

    public ErrorViewModel()
    {
    }

    public ErrorViewModel(string error)
    {
        Error = new ErrorMessage { Error = error };
    }

}
