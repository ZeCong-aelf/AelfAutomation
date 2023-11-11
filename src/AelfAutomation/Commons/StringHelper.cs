using System;
using System.Collections.Generic;
using System.Globalization;

namespace CAServer.Commons;

public static class StringHelper
{
    public static double SafeToDouble(this string s, double defaultValue = 0)
    {
        return double.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out var result) ? result : defaultValue;
    }
    public static decimal SafeToDecimal(this string s, decimal defaultValue = 0)
    {
        return decimal.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out var result) ? result : defaultValue;
    }
    
    public static int SafeToInt(this string s, int defaultValue = 0)
    {
        return int.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out var result) ? result : defaultValue;
    }
    
    public static long SafeToLong(this string s, long defaultValue = 0)
    {
        return long.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out var result) ? result : defaultValue;
    }


}