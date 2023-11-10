using System;
using System.Collections.Generic;
using System.Linq;

namespace AelfAutomation.Commons;

public static class AssertHelper
{
    private const string DefaultErrorReason = "Assert failed";

    
    public static void IsTrue(bool expression, string? reason)
    {
        IsTrue(expression, -1, reason);
    }


    public static void NotEmpty(string? str, string? reason)
    {
        IsTrue(str != null && str.Any(), reason);
    }
    
    public static void NotEmpty(Guid guid, string? reason)
    {
        IsTrue(guid != Guid.Empty, reason);
    }
    
    public static void NotEmpty<T>(IEnumerable<T>? collection, string? reason)
    {
        IsTrue(collection != null && collection.Any(), reason);
    }

    public static void NullOrEmpty(string? str, string? reason)
    {
        IsTrue(str == null || !str.Any(), reason);
    }
    
    public static void NullOrEmpty(Guid guid, string? reason)
    {
        IsTrue(guid == Guid.Empty, reason);
    }
    
    public static void NullOrEmpty<T>(IEnumerable<T>? collection, string? reason)
    {
        IsTrue(collection == null || !collection.Any(), reason);
    }

    public static void NotNull(object obj, string? reason)
    {
        IsTrue(obj != null, reason);
    }

    public static void IsTrue(bool expression, int code = -1, string? reason = DefaultErrorReason)
    {
        if (!expression)
        {
            throw new Exception(string.Join(",", code, reason));
        }
    }
}