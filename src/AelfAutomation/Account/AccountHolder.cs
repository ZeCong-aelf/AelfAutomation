using AElf;
using AElf.Client;
using AElf.Cryptography;
using AElf.Types;
using Google.Protobuf;

namespace AelfAutomation.Account;

public class AccountHolder
{
    
    public string PrivateKey { get; set; }
    public string Address { get; set; }
    public string PublicKey { get; set; }
    
    public AccountHolder(string privateKey)
    {
        PrivateKey = privateKey;
        Address = new AElfClient("http://127.0.0.1:8000").GetAddressFromPrivateKey(privateKey);
        var keyPair = CryptoHelper.FromPrivateKey(ByteArrayHelper.HexStringToByteArray(privateKey));
        PublicKey = keyPair.PublicKey.ToHex();
    }

    public Address AddressObj()
    {
        return AElf.Types.Address.FromBase58(Address);
    }

    public ByteString GetSignatureWith(byte[] txData)
    {
        var signature = CryptoHelper.SignWithPrivateKey(ByteArrayHelper.HexStringToByteArray(PrivateKey), txData);
        return ByteString.CopyFrom(signature);
    }
}