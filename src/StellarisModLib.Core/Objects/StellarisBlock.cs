using System.Text;

namespace StellarisModLib.Core.Objects;

/// <summary>
/// Represents a block (enclosed in { }) in Stellaris files
/// </summary>
public class StellarisBlock : StellarisObject
{
    public string Name { get; set; } // Optional
    public List<StellarisObject> Children { get; set; } = [];
        
    // Additional formatting info specifically for blocks
    public string OpenBraceWhitespace { get; set; } = " ";
    public string CloseBraceWhitespace { get; set; } = "";

    public override string Serialize(int indentLevel = 0)
    {
        string indent = new('\t', indentLevel);
        StringBuilder sb = new();

        sb.Append(LeadingWhitespace);
            
        if (!string.IsNullOrEmpty(Name))
            sb.Append($"{indent}{Name}");
            
        sb.Append($"{OpenBraceWhitespace}{{");
            
        if (Children.Count > 0)
        {
            sb.AppendLine();
            foreach (StellarisObject child in Children)
            {
                sb.AppendLine(child.Serialize(indentLevel + 1));
            }
            sb.Append($"{indent}}}");
        }
        else
        {
            sb.Append($"}}{CloseBraceWhitespace}");
        }

        sb.Append(TrailingWhitespace);
        return sb.ToString();
    }
}