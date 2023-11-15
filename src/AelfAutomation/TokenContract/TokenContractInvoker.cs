using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AElf;
using AElf.Client.Dto;
using AElf.Client.Extensions;
using AElf.Contracts.MultiToken;
using AelfAutomation.Account;
using AelfAutomation.Commons;
using Google.Protobuf;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Org.BouncyCastle.Utilities;

namespace AelfAutomation.TokenContract;

public class TokenContractInvoker : ContractInvoker
{
    public TokenContractInvoker(IConfiguration config, Dictionary<string, AccountHolder> accountHolders) : base(config,
        accountHolders)
    {
    }


    public async Task GetSelfBalance(string account)
    {
        var accountExists = AccountHolders.TryGetValue(account, out var accountHolder);
        AssertHelper.IsTrue(accountExists, "Account not exits " + account);
        var input = new GetBalanceInput
        {
            Owner = account.ToAddress(),
            Symbol = "ELF",
        };

        var (txId, tx) = await CreateContractRawTransactionAsync(accountHolder, SystemContractName.SystemTokenContract,
            "GetBalance", input);

        var res = await Client.ExecuteTransactionAsync(new ExecuteTransactionDto
        {
            RawTransaction = tx.ToByteArray().ToHex()
        });

        Console.WriteLine(JsonConvert.SerializeObject(
            GetBalanceOutput.Parser.ParseFrom(ByteArrayHelper.HexStringToByteArray(res)), JsonSerializerSettings));
    }
}