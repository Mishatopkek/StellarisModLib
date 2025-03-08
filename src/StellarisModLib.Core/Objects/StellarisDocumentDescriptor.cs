namespace StellarisModLib.Core.Objects;

/// <summary>
/// Represents descriptor because it requires extra files
/// </summary>
public class StellarisDocumentDescriptor : StellarisDocument
{
    public string ModName { get; set; }
    public override void SaveToFile(string path)
    {
        File.WriteAllText(Path.Combine(StellarisModManager.ModsDirectory, $"{ModName}.mod"), Serialize());
        Children.RemoveAll(x => x is StellarisProperty {Key: "path"});
        File.WriteAllText(Path.Combine(path, FileName), Serialize());
    }
}