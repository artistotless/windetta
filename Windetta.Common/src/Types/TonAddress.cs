namespace Windetta.Common.Types;

public struct TonAddress
{
    public readonly string Value;

    public TonAddress(string address)
    {
        if (address.Length != 48)
            throw new ArgumentException(nameof(address));

        if (!address.StartsWith("EQ") || !address.StartsWith("UQ"))
            throw new ArgumentException(nameof(address));

        Value = address;
    }

    public static implicit operator string(TonAddress address) => address.Value;
}
