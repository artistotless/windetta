
using Windetta.Common.Messages;

namespace Windetta.Identity.Messages.Comands;

public class ExternalLogin : ICommand
{
    public Guid UserId { get; set; }
    public string UniqueIdentifier { get; set; }
    public string Provider { get; set; }
}
