namespace StellarisModLib.Core.Objects;

/// <summary>
/// Represents a value
/// </summary>
public class StellarisValue : StellarisObject
{
    public object Value { get; set; } // Can be string, number, or complex expression

    public override string Serialize(int indentLevel = 0)
    {
        string indent = new('\t', indentLevel);
        string valueStr;
        if (Value is string str && !str.StartsWith("\"") && !str.EndsWith("\""))
            valueStr = $"\"{str}\"";
        else
            valueStr = Value?.ToString() ?? "null";

        return $"{indent}{valueStr}";
    }
}