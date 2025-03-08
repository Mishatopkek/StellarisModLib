namespace StellarisModLib.Core.Objects;

/// <summary>
/// Base class for all Stellaris mod objects
/// </summary>
public abstract class StellarisObject
{
    // Original formatting information for reconstructing the file
    public string LeadingWhitespace { get; set; } = "";
    public string TrailingWhitespace { get; set; } = "";
    public List<string> Comments { get; set; } = [];

    // Abstract method that all Stellaris objects must implement
    public abstract string Serialize(int indentLevel = 0);
}