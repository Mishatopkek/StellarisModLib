using System.Text;

namespace StellarisModLib.Core.Objects;

/// <summary>
/// Represents a conditional expression (AND, OR, NOT, NOR)
/// </summary>
public class StellarisCondition : StellarisObject
{
    public string ConditionOperator { get; set; } // AND, OR, NOT, NOR
    public List<StellarisObject> Operands { get; set; } = [];
    public string Operator { get; set; } = "=";
        
    // Additional formatting info for conditions
    public string OperatorWhitespace { get; set; } = " ";
    public string OpenBraceWhitespace { get; set; } = " ";
    public string CloseBraceWhitespace { get; set; } = "";

    public override string Serialize(int indentLevel = 0)
    {
        string indent = new('\t', indentLevel);
        StringBuilder sb = new();

        sb.Append($"{LeadingWhitespace}{indent}{{{LeadingWhitespace}{ConditionOperator}{LeadingWhitespace}{Operator}{OperatorWhitespace}");
        sb.Append($"{{{OpenBraceWhitespace}");
            
        if (Operands.Count > 0)
        {
            sb.AppendLine();
            foreach (StellarisObject operand in Operands)
            {
                sb.AppendLine(operand.Serialize(indentLevel + 1));
            }
            sb.Append($"{indent}}}}}");
        }
        else
        {
            sb.Append($"}}{CloseBraceWhitespace}");
        }

        sb.Append(TrailingWhitespace);
        return sb.ToString();
    }
}