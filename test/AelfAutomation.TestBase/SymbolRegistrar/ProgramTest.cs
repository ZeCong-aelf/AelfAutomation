using System.Threading.Tasks;
using Xunit;

namespace AelfAutomation.SymbolRegistrar;

public class ProgramTest
{



    [Fact]
    public async Task Run()
    {
        
        await Program.Main(new []{
            "SpecialSeed.add", 
            "2CpKfnoWTk69u6VySHMeuJvrX2hGrMw9pTyxcD4VM6Q28dJrhk", 
            "uniq",
            "1"
            });

    }
    
    
}