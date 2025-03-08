using System.Text;

namespace StellarisModLib.Core.Objects;

/// <summary>
/// Document-level class representing a full Stellaris mod file
/// </summary>
public class StellarisDocument : StellarisObject
{
    public List<StellarisObject> Children { get; set; } = [];
    public string FileName { get; set; }

    public override string Serialize(int indentLevel = 0)
    {
        StringBuilder sb = new();
        foreach (StellarisObject child in Children)
        {
            sb.AppendLine(child.Serialize());
        }
        return sb.ToString();
    }

    public virtual void SaveToFile(string path)
    {
        string serialiseText = Serialize();
        File.WriteAllText(Path.Combine(path, FileName), serialiseText);
    }
}