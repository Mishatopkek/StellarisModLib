namespace StellarisModLib.Core.Entities;

/// <summary>
/// Extension methods for string manipulations
/// </summary>
public static class StringExtensions
{
    public static string ToFirstUpper(this string str)
    {
        if (string.IsNullOrEmpty(str))
            return string.Empty;
            
        return char.ToUpper(str[0]) + str.Substring(1);
    }
}