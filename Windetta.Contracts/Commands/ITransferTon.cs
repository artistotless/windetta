namespace Windetta.Contracts.Commands;

public interface ITransferTon
{
    public Guid InitiatorUserId { get; set; }
    public Guid DestinationUserId { get; set; }
    public long Nanotons { get; set; }
}
