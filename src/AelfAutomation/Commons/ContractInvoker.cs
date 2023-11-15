using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    protected Dictionary<string, AccountHolder> AccountHolders;

    protected static JsonSerializerSettings JsonSerializerSettings =
        JsonSettingsBuilder.New().WithAElfTypesConverters().Build();

    protected ContractInvoker(IConfiguration config, Dictionary<string, AccountHolder> accountHolders)
    {
        AccountHolders = accountHolders;
        Client = new AElfClient(config.GetSection("nodeUrl").Get<string>());
        ContractAddress = config.GetSection("ContractAddress").Get<Dictionary<string, string>>();
    }


    protected async Task<Tuple<Hash, Transaction>> CreateContractRawTransactionAsync(AccountHolder user, string contractName,
        string methodName, IMessage parameters)
    {
        return await CreateContractRawTransactionAsync(user,
            await Client.GetContractAddressByNameAsync(HashHelper.ComputeFrom(contractName)),
            methodName, parameters);
    }

    protected async Task<Tuple<Hash, Transaction>> CreateContractRawTransactionAsync(AccountHolder user, Address contractAddress,
        string methodName, IMessage parameters)
    {
        var status = await Client.GetChainStatusAsync();
        var height = status.BestChainHeight;
        var blockHash = status.BestChainHash;

        // create row transaction
        var transaction = new Transaction
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
            }, JsonSerializerSettings)}");
        return Tuple.Create(transactionId, transaction);
    }
}