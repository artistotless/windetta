using Windetta.Common.Messages;

namespace Windetta.Identity.Messages.Comands;

public class Register : IRequest
{
    public string Email { get; set; }
    public string Password { get; set; }
}
