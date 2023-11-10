using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AElf;
using AElf.Client.Dto;
using AElf.Types;
using AelfAutomation.Account;
using AelfAutomation.Commons;
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

    public AddSpecialSeed(IConfiguration config, Dictionary<string, AccountHolder> accountHolders) : base(config)
    {
        _accountHolders = accountHolders;
        _config = config;

        var configSection = _config.GetSection("AddSpecialSeed");
        _uniqFile = PathHelper.ResolvePath(configSection.GetSection("UniqFile").Get<string>());
        _notableFile = PathHelper.ResolvePath(configSection.GetSection("NotableFile").Get<string>());
        _uniqLogFile = PathHelper.ResolvePath(configSection.GetSection("UniqResultFile").Get<string>());
        _notableLogFile = PathHelper.ResolvePath(configSection.GetSection("NotableResultFile").Get<string>());
    }


    public async Task DoAdd(string account, string type, string? pageString)
    {
        var page = pageString.SafeToInt(-1);
        AssertHelper.IsTrue(page > 0, "Invalid page");
        
        const int pageSize = 100;
        var allLines = await File.ReadAllLinesAsync(type == "uniq" ? _uniqFile : _notableFile);
        var totalPage = (allLines.Length + pageSize - 1) / pageSize;

        for (var currentPage = 1; currentPage <= totalPage; currentPage++)
        {
            if (currentPage < page) continue;
            
            var pageLines = allLines
                .Skip((currentPage - 1) * pageSize)
                .Take(pageSize)
                .ToArray();
            await ProcessPage(account, pageLines, currentPage);

            // process the page only
            if (page > 0) break;
        }
    }

    private async Task ProcessPage(string account, string[] pageLines, int currentPage)
    {
        var list = new UniqueSeedList();
        list.Symbols.AddRange(pageLines);
        var accountExists = _accountHolders.TryGetValue(account, out var accountHolder);
        var (txId, tx) = CreateContractRawTransactionAsync(accountHolder, Address.FromBase58(""), "AddUniqueSeeds", list);
        var res = await Client.SendTransactionAsync(new SendTransactionInput
        {
            RawTransaction = tx.ToByteArray().ToHex()
        });
        //TODO wait
        //TODO record
    }
    
    
    
    


    public async Task DoCheck()
    {
        //TODO read record
        //TODO query result and print
    }

    
}