namespace StellarisModLib.Core.Objects;

/// <summary>
/// Represents a key-value pair in Stellaris files
/// </summary>
public class StellarisProperty : StellarisObject
{
    public string Key { get; set; }
    public object Value { get; set; } // Can be string, number, or complex expression
    public string Operator { get; set; } = "="; // Usually "=" but could be comparison operators

    public override string Serialize(int indentLevel = 0)
    {
        string indent = new('\t', indentLevel);
        string valueStr;

        if (Value is string str && !str.StartsWith("\"") && !str.EndsWith("\""))
            valueStr = $"\"{str}\"";
        else if (Value is StellarisObject obj)
            valueStr = obj.Serialize(indentLevel);
        else
            valueStr = Value?.ToString() ?? "null";

        return $"{LeadingWhitespace}{indent}{Key} {Operator} {valueStr}{TrailingWhitespace}";
    }
}