using Microsoft.Extensions.Configuration;
using NETCore.Encrypt;

namespace Windetta.Common.Helpers;

public sealed class AesEncryptor
{
    private readonly string _privateKey;
    private readonly string _vector;

    public AesEncryptor(IConfiguration cfg)
    {
        var section = cfg.GetSection("EncryptionOptions");

        _privateKey = section.GetValue<string>("PrivateKey")!;
        _vector = section.GetValue<string>("Vector")!;

        CheckKeys();
    }

    /// <returns>Encrypted value</returns>
    public string Encrypt(string input)
        => EncryptProvider.AESEncrypt(input, _privateKey, _vector);

    /// <returns>Decrypted value</returns>
    public string Decrypt(string input)
        => EncryptProvider.AESDecrypt(input, _privateKey, _vector);

    private void CheckKeys()
    {
        ArgumentException
        .ThrowIfNullOrEmpty(_privateKey, nameof(_privateKey));

        ArgumentException
        .ThrowIfNullOrEmpty(_vector, nameof(_vector));

        if (_privateKey.Length != 32)
            throw new ArgumentNullException("private key's length shoud be 32");

        if (_vector.Length != 16)
            throw new ArgumentNullException("vector's length shoud be 16");
    }
}
