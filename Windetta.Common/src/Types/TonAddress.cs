namespace Windetta.Common.Types;

public struct TonAddress
{
    public string Value { get; init; }

    public TonAddress(string address)
    {
        if (string.IsNullOrEmpty(address))
            throw new ArgumentNullException(nameof(address));

        if (address.Length != 48)
            throw new ArgumentException(nameof(address));

        if (!address.StartsWith("EQ") && !address.StartsWith("UQ"))
            throw new ArgumentException(nameof(address));

        Value = address;
    }
    public override string ToString() => Value;

    public static implicit operator string(TonAddress address) => address.Value;
}
