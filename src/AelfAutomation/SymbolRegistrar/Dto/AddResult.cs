using CAServer.Commons;

namespace AelfAutomation.SymbolRegistrar.Dto;

public class AddResult
{
    public int Page { get; set; }
    public string FromWord { get; set; }
    public string ToWord { get; set; }
    public string TransactionId { get; set; }


    public string ToResultLine()
    {
        return string.Join(",", Page, FromWord, ToWord, TransactionId);
    }

    public static AddResult FromResultLine(string stringLine)
    {
        var vals = stringLine.Split(",");
        return new AddResult
        {
            Page = vals[0].SafeToInt(),
            FromWord = vals[1],
            ToWord = vals[2],
            TransactionId = vals[3]
        };
    }

}