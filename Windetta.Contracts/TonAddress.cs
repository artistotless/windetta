namespace Windetta.Contracts;

public struct TonAddress
{
    public string Value { get; init; }

    public TonAddress(string address)
    {
        if (string.IsNullOrEmpty(address))
            throw new ArgumentNullException(nameof(address));

        if (address.Length != 48)
            throw new ArgumentException(nameof(address));

        Value = address;
    }

    public static TonAddress Parse(string address)
        => new TonAddress(address);

    public override string ToString() => Value;

    public static implicit operator string(TonAddress address) => address.Value;
}
