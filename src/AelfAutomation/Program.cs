using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using AelfAutomation.Account;
using AelfAutomation.Commons;
using AelfAutomation.SymbolRegistrar;
using AelfAutomation.TokenContract;
using Microsoft.Extensions.Configuration;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.KeyStore;

namespace AelfAutomation
{
    public class Program
    {
        private static IConfiguration _config;
        private static Dictionary<string, AccountHolder> _accountHolders = new();
        private static KeyStoreService _keyStoreService = new();
        
        
        public static async Task Main(string[] args)
        {
            Console.WriteLine("start");
            
            LoadConfigurations();
            LoadAccounts();
            LoadKeyStores();

            var cmd = args.Length < 1 ? null : args[0];
            var accountAddress = "";
            AssertHelper.NotEmpty(cmd, "missing cmd param: args[0]");
            switch (cmd)
            {
                case "SpecialSeed.add" :
                    accountAddress = args[1]; // sender
                    var type = args[2]; // uniq / notable
                    var pageString = args[3]; // [opt] send page only
                    await new AddSpecialSeed(_config, _accountHolders).DoAdd(accountAddress, type, pageString);
                    break;
                case "SpecialSeed.check" : 
                    var resultFile = args[1]; // result file to check
                    await new AddSpecialSeed(_config, _accountHolders).DoCheck(resultFile);
                    break;
                case "SetPrice.common" : 
                    accountAddress = args[1]; // sender
                    await new SetPrice(_config, _accountHolders).SetCommonPrice(accountAddress);
                    break;
                case "SetPrice.uniq" : 
                    accountAddress = args[1]; // sender
                    await new SetPrice(_config, _accountHolders).SetUniqExtensionPrice(accountAddress);
                    break;
                case "GetSelfBalance" : 
                    accountAddress = args[1]; // sender
                    await new TokenContractInvoker(_config, _accountHolders).GetSelfBalance(accountAddress);
                    break;
                default: throw new Exception($"Invalid cmd param: {cmd}");
            }
            
            Console.WriteLine("Finish");
        }
        
        private static void LoadConfigurations()
        {
            _config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();
        
        }

        private static void LoadAccounts()
        {
            var keyFilePath = _config.GetSection("keyFilePath").Get<string>();
            var passwords = _config.GetSection("passwords").Get<Dictionary<string, string>>() ?? new Dictionary<string, string>();
            foreach (var (address, password) in passwords)
            {
                var path = PathHelper.ResolvePath(keyFilePath + "/" + address + ".json");
                using var textReader = File.OpenText(path);
                var json = textReader.ReadToEnd();
                
                var privateKey = _keyStoreService.DecryptKeyStoreFromJson(password, json);
                Console.WriteLine($"Load account {address} success");
                _accountHolders[address] = new AccountHolder(privateKey.ToHex());
            }
        }

        private static void LoadKeyStores()
        {
            var keyFilePath = _config.GetSection("keyFilePath").Get<string>();
            var loadAddresses = _config.GetSection("loadKeystore").Get<List<string>>() ?? new List<string>();
            if (loadAddresses.Count < 1) return;
            
            
            Console.WriteLine("Press [Enter] to decode keystore");
            while(ConsoleKey.Enter != Console.ReadKey(true).Key)
            {
                
            }
            
            foreach (var address in loadAddresses)
            {
                
                var path = PathHelper.ResolvePath(keyFilePath + "/" + address + ".json");
                using var textReader = File.OpenText(path);
                var json = textReader.ReadToEnd();
                
                /* read password from console */
                Console.Write($"input password of account {address}: ");
                var pwd = "";
                while (true)
                {
                    var key = Console.ReadKey(true);
                    if (key.Key == ConsoleKey.Enter)
                        break;
                    if (key.Key == ConsoleKey.Backspace && pwd.Length > 0)
                        pwd = pwd[0..^1];
                    else if (key.Key != ConsoleKey.Backspace)
                        pwd += key.KeyChar;
                }
                
                var privateKey = _keyStoreService.DecryptKeyStoreFromJson(pwd, json);
                Console.WriteLine("success");
                _accountHolders[address] = new AccountHolder(privateKey.ToHex());
            }
        }
        
    }
}