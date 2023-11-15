using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AElf;
using AElf.Client.Dto;
using AElf.Types;
using AelfAutomation.Account;
using AelfAutomation.Commons;
using Forest.Contracts.SymbolRegistrar;
using Google.Protobuf;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace AelfAutomation.SymbolRegistrar;

public class SetPrice : ContractInvoker
{
    public SetPrice(IConfiguration config, Dictionary<string, AccountHolder> accountHolders) : base(config,
        accountHolders)
    {
    }


    public async Task SetCommonPrice(string account)
    {
        var ftPriceList = GetPriceList("FT");
        var nftPriceList = GetPriceList("NFT");

        var accountExists = AccountHolders.TryGetValue(account, out var accountHolder);
        AssertHelper.IsTrue(accountExists, $"Account {account} not exists");

        var (txId, tx) = await CreateContractRawTransactionAsync(
            accountHolder,
            Address.FromBase58(ContractAddress["Forest.SymbolRegistrarContract"]), "SetSeedsPrice", new SeedsPriceInput
            {
                FtPriceList = ftPriceList,
                NftPriceList = nftPriceList
            });
        await Client.SendTransactionAsync(new SendTransactionInput
        {
            RawTransaction = tx.ToByteArray().ToHex()
        });

        // wait
        await Task.Delay(1000);

        // query result
        var queryResult = await Client.GetTransactionResultAsync(txId.ToHex());

        Console.WriteLine(JsonConvert.SerializeObject(queryResult));
    }


    private PriceList GetPriceList(string type)
    {
        var priceList = new PriceList();
        var start = 1;
        var end = 30;
        for (var i = start; i <= end; i++)
        {
            var item = new PriceItem
            {
                SymbolLength = i,
                Symbol = "ELF"
            };
            var itemSymbolLength = item.SymbolLength;
            var amount = type.Equals("NFT") 
                ? GetPrice(itemSymbolLength - 2, type) 
                : GetPrice(itemSymbolLength, type);
            item.Amount = amount;
            priceList.Value.Add(item);
        }

        return priceList;
    }


    public long GetPrice(int itemSymbolLength, string type)
    {
        var amount = 0L;
        if (type.Equals("NFT"))
        {
            amount = itemSymbolLength <= 0 ? 100000_0000_0000
                : itemSymbolLength <= 3 ? 1000_0000_0000
                : itemSymbolLength == 4 ? 100_0000_0000
                : itemSymbolLength == 5 ? 80_0000_0000
                : itemSymbolLength <= 10 ? 20_0000_0000
                : 3_0000_0000;
        }
        else
        {
            amount = itemSymbolLength == 1 ? 1000_0000_0000
                : itemSymbolLength == 2 ? 900_0000_0000
                : itemSymbolLength == 3 ? 600_0000_0000
                : itemSymbolLength == 4 ? 200_0000_0000
                : itemSymbolLength == 5 ? 100_0000_0000
                : itemSymbolLength <= 8 ? 20_0000_0000
                : 3_0000_0000;
        }

        return amount;
    }

    public async Task SetUniqExtensionPrice(string account)
    {
        var ftPriceList = GetUniqueSeedsExternalPriceList("FT");
        var nftPriceList = GetUniqueSeedsExternalPriceList("NFT");

        var accountExists = AccountHolders.TryGetValue(account, out var accountHolder);
        AssertHelper.IsTrue(accountExists, $"Account {account} not exists");


        var (txId, tx) = await CreateContractRawTransactionAsync(
            accountHolder,
            Address.FromBase58(ContractAddress["Forest.SymbolRegistrarContract"]), "SetUniqueSeedsExternalPrice",
            new UniqueSeedsExternalPriceInput
            {
                FtPriceList = ftPriceList,
                NftPriceList = nftPriceList
            });
        await Client.SendTransactionAsync(new SendTransactionInput
        {
            RawTransaction = tx.ToByteArray().ToHex()
        });

        // wait
        await Task.Delay(1000);

        // query result
        var queryResult = await Client.GetTransactionResultAsync(txId.ToHex());

        Console.WriteLine(JsonConvert.SerializeObject(queryResult));
    }

    private PriceList GetUniqueSeedsExternalPriceList(string type)
    {
        var priceList = new PriceList();
        var start = 2;
        var end = type.Equals("NFT") ? 30 : 10;
        for (var i = start; i <= end; i++)
        {
            var item = new PriceItem
            {
                SymbolLength = i,
                Symbol = "ELF"
            };
            var itemSymbolLength = item.SymbolLength;
            var amount = type.Equals("NFT")
                ? GetUniquePrice(itemSymbolLength - 2)
                : GetUniquePrice(itemSymbolLength);
            item.Amount = amount;
            priceList.Value.Add(item);
        }

        return priceList;
    }

    public long GetUniquePrice(int itemSymbolLength)
    {
        return itemSymbolLength == 2 ? 180_0000_0000
                : itemSymbolLength == 3 ? 120_0000_0000
                : itemSymbolLength == 4 ? 90_0000_0000
                : itemSymbolLength == 5 ? 60_0000_0000
                : itemSymbolLength >= 6 ? 30_0000_0000
                : 1000000_0000_0000 ;
    }
}