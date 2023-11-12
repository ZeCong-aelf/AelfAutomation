using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AElf;
using AElf.Client.Dto;
using AElf.Types;
using AelfAutomation.Account;
using AelfAutomation.Commons;
using AelfAutomation.SymbolRegistrar.Dto;
using CAServer.Commons;
using Forest.Contracts.SymbolRegistrar;
using Google.Protobuf;
using Microsoft.Extensions.Configuration;

namespace AelfAutomation.SymbolRegistrar;

public class AddSpecialSeed : ContractInvoker
{
    private IConfiguration _config;
    private Dictionary<string, AccountHolder> _accountHolders;

    private string _uniqFile;
    private string _notableFile;
    private string _uniqLogFile;
    private string _notableLogFile;
    private int _pageSize;

    public AddSpecialSeed(IConfiguration config, Dictionary<string, AccountHolder> accountHolders) : base(config)
    {
        _accountHolders = accountHolders;
        _config = config;

        var configSection = _config.GetSection("AddSpecialSeed");
        _uniqFile = PathHelper.ResolvePath(configSection.GetSection("UniqFile").Get<string>());
        _notableFile = PathHelper.ResolvePath(configSection.GetSection("NotableFile").Get<string>());
        _uniqLogFile = PathHelper.ResolvePath(configSection.GetSection("UniqResultFile").Get<string>());
        _notableLogFile = PathHelper.ResolvePath(configSection.GetSection("NotableResultFile").Get<string>());
        _pageSize = configSection.GetSection("PageSize").Get<int>();
    }
    
    public async Task DoAdd(string account, string type, string? pageString)
    {
        var page = pageString?.Split(",").Select(p => p.SafeToInt(-1)).ToList() ?? new List<int>();
        AssertHelper.IsTrue(page.Count == 0 || page.Any(p => p > 0), "Invalid page");

        var pageSize = _pageSize;
        var allLines = await File.ReadAllLinesAsync(type == "uniq" ? _uniqFile : _notableFile);
        var totalPage = (allLines.Length + pageSize - 1) / pageSize;
        var resultLines = new List<string>();

        for (var currentPage = 1; currentPage <= totalPage; currentPage++)
        {
            if (page.Count > 0 && !page.Contains(currentPage) ) continue;

            var pageLines = allLines
                .Skip((currentPage - 1) * pageSize)
                .Select(w => Regex.Replace(w, @"\s+", "")) // remove all blank character
                .Select(w => w.ToUpper()) // convert all letters uppercase
                .Take(pageSize)
                .Distinct()
                .ToArray();
            var res = type == "uniq"
                ? await ProcessPageUniq(account, pageLines, currentPage)
                : await ProcessPageNotable(account, pageLines, currentPage);
            resultLines.Add(res.ToResultLine());

            // test mode
            // if (currentPage >= 5) break;
        }

        var resultFilePath = type == "uniq" ? _uniqLogFile : _notableLogFile;
        resultFilePath += DateTime.UtcNow.ToUtc8String(TimeHelper.UnderlinePattern);
        await File.WriteAllLinesAsync(resultFilePath, resultLines);
    }

    private async Task<AddResult> ProcessPageNotable(string account, string[] pageLines, int currentPage)
    {
        var accountExists = _accountHolders.TryGetValue(account, out var accountHolder);
        AssertHelper.IsTrue(accountExists, $"Account {account} not exists");
        
        var specialList = new SpecialSeedList();

        foreach (var symbol in pageLines)
        {
            specialList.Value.Add(new SpecialSeed
            {
                SeedType = SeedType.Notable,
                AuctionType = AuctionType.English,
                Symbol = symbol,
                PriceAmount = 1000_0000_0000,
                PriceSymbol = "ELF",
                IssueChain = "ETH",
                IssueChainContractAddress = "0x00"
            });
        }
        
        // create tx
        var (txId, tx) =
            CreateContractRawTransactionAsync(accountHolder,
                Address.FromBase58(ContractAddress["Forest.SymbolRegistrarContract"]), "AddSpecialSeeds", specialList);

        // send
        var res = await Client.SendTransactionAsync(new SendTransactionInput
        {
            RawTransaction = tx.ToByteArray().ToHex()
        });

        // wait
        await Task.Delay(1000);

        // query result
        var queryResult = await Client.GetTransactionResultAsync(txId.ToHex());

        var addResult = new AddResult()
        {
            Page = currentPage,
            FromWord = pageLines.First(),
            ToWord = pageLines.Last(),
            TransactionId = txId.ToHex()
        };

        Console.WriteLine($"Result: {addResult.ToResultLine()}, status: {queryResult?.Status}, error: {queryResult?.Error}");
        return addResult;
    }

    private async Task<AddResult> ProcessPageUniq(string account, string[] pageLines, int currentPage)
    {
        var list = new UniqueSeedList();
        list.Symbols.AddRange(pageLines);
        var accountExists = _accountHolders.TryGetValue(account, out var accountHolder);
        AssertHelper.IsTrue(accountExists, $"Account {account} not exists");

        // create tx
        var (txId, tx) =
            CreateContractRawTransactionAsync(accountHolder,
                Address.FromBase58(ContractAddress["Forest.SymbolRegistrarContract"]), "AddUniqueSeeds", list);

        // send
        var res = await Client.SendTransactionAsync(new SendTransactionInput
        {
            RawTransaction = tx.ToByteArray().ToHex()
        });

        // wait
        await Task.Delay(1000);

        // query result
        var queryResult = await Client.GetTransactionResultAsync(txId.ToHex());

        var addResult = new AddResult()
        {
            Page = currentPage,
            FromWord = pageLines.First(),
            ToWord = pageLines.Last(),
            TransactionId = txId.ToHex()
        };

        Console.WriteLine($"Result: {addResult.ToResultLine()}, status: {queryResult?.Status}, error: {queryResult?.Error}");
        return addResult;
    }


    public async Task DoCheck(string resultFile)
    {
        // read result file
        var resultFilePath = PathHelper.ResolvePath(resultFile);
        var allLines = await File.ReadAllLinesAsync(resultFilePath);
        var resultList = allLines.Select(AddResult.FromResultLine).ToList();

        // query tx result and print
        var pendingPages = new List<int>();
        var failPages = new List<int>();
        foreach (var resultItem in resultList)
        {
            var queryResult = await Client.GetTransactionResultAsync(resultItem.TransactionId);
            Console.WriteLine(resultItem.ToResultLine() + $" status={queryResult?.Status}, error={queryResult?.Error}");
            if (queryResult.Status == "PENDING") pendingPages.Add(resultItem.Page);
            else if (queryResult.Status != "MINED") failPages.Add(resultItem.Page);
        }

        Console.WriteLine("Pending pages: " + string.Join(",", pendingPages));
        Console.WriteLine("Failed pages: " + string.Join(",", failPages));
    }
}