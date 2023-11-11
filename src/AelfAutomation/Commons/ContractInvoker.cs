using System;
using System.Collections.Generic;
using System.Linq;
using AElf;
using AElf.Client;
using AElf.Client.Dto;
using AElf.Types;
using AelfAutomation.Account;
using Google.Protobuf;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace AelfAutomation.Commons;

public abstract class ContractInvoker
{

    protected AElfClient Client;
    protected Dictionary<string, string> ContractAddress;
    protected readonly JsonSerializerSettings Settings = new JsonSerializerSettings();

    protected ContractInvoker(IConfiguration config)
    {
        Client = new AElfClient(config.GetSection("nodeUrl").Get<string>());
        ContractAddress = config.GetSection("ContractAddress").Get<Dictionary<string, string>>();
    }


    protected Tuple<Hash, Transaction> CreateContractRawTransactionAsync(AccountHolder user, Address contractAddress, string methodName, IMessage parameters)
    {
        var status = Client.GetChainStatusAsync().GetAwaiter().GetResult();
        var height = status.BestChainHeight;
        var blockHash = status.BestChainHash;

        // create row transaction
        var transaction = new Transaction()
        {
            From = user.AddressObj(),
            To = contractAddress,
            MethodName = methodName,
            Params = parameters == null ? ByteString.Empty : parameters.ToByteString(),
            RefBlockNumber = height,
            RefBlockPrefix = ByteString.CopyFrom(Hash.LoadFromHex(blockHash).Value.Take(4).ToArray())
        };

        var transactionId = HashHelper.ComputeFrom(transaction.ToByteArray());
        transaction.Signature = user.GetSignatureWith(transactionId.ToByteArray());

        Console.WriteLine(
            $@"[{contractAddress.ToBase58()}.{methodName}] >>> Raw contract tx: {JsonConvert.SerializeObject(new Dictionary<string, object>
            {
                ["transaction"] = transaction,
                ["parameter"] = parameters
            }, Settings)}");
        return Tuple.Create(transactionId, transaction);
    }

    
    
    
    
}