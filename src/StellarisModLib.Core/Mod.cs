namespace StellarisModLib.Core;

public class Mod
{
    public required string Name { get; set; }
    public Version Version { get; set; } = new();
    public Version SupportedGameVersion { get; set; } = new();
    public List<string> Dependencies { get; set; }

    public void Build()
    {
        throw new NotImplementedException();
    }
}