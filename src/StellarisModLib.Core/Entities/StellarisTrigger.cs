using StellarisModLib.Core.Objects;

namespace StellarisModLib.Core.Entities;

public sealed class StellarisTrigger
{
    public StellarisCondition? Ethics { get; set; }
    public StellarisCondition? Authority { get; set; }
}