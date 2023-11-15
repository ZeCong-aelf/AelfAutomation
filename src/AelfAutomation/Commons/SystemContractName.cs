using System.Collections.Generic;

namespace AelfAutomation.Commons;

public static class SystemContractName
{
    public const string SystemBasicContractZero = "";
    public const string SystemCrossChainContract = "AElf.ContractNames.CrossChain";
    public const string SystemTokenContract = "AElf.ContractNames.Token";
    public const string SystemParliamentContract = "AElf.ContractNames.Parliament";
    public const string SystemConsensusContract = "AElf.ContractNames.Consensus";
    public const string SystemReferendumContract = "AElf.ContractNames.Referendum";
    public const string SystemTreasuryContract = "AElf.ContractNames.Treasury";
    public const string SystemAssociationContract = "AElf.ContractNames.Association";
    public const string SystemTokenConverterContract = "AElf.ContractNames.TokenConverter";
    public const string SystemVoteContract = "AElf.ContractNames.Vote";
    public const string SystemGenesisContract = "AElf.ContractNames.Genesis";
    public const string SystemProfitContract = "AElf.ContractNames.Profit";
    public const string SystemElectionContract = "AElf.ContractNames.Election";
    public const string SystemConfigurationContract = "AElf.ContractNames.Configuration";
    public const string SystemTokenHolderContract = "AElf.ContractNames.TokenHolder";
    public const string SystemEconomicContract = "AElf.ContractNames.Economic";

    public static List<string> AllSystemContract = new()
    {
        SystemBasicContractZero, SystemCrossChainContract, SystemTokenContract,
        SystemParliamentContract, SystemConsensusContract, SystemReferendumContract,
        SystemTreasuryContract, SystemAssociationContract, SystemTokenConverterContract,
        SystemVoteContract, SystemGenesisContract, SystemProfitContract, 
        SystemElectionContract, SystemConfigurationContract, SystemTokenHolderContract,
        SystemEconomicContract
    };
}
