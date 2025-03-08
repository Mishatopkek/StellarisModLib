using StellarisModLib.Core.Objects;

namespace StellarisModLib.Core.Entities;

/// <summary>
/// Base class for Stellaris defined entities that typically have a key and fields
/// </summary>
public abstract class StellarisEntity
{
    public string Id { get; set; }
        
    // Convert entity to StellarisBlock for serialization
    public virtual StellarisBlock ToBlock()
    {
        StellarisBlock block = new()
        {
            Name = Id
        };
            
        return block;
    }
        
    // Create entity from StellarisBlock
    public virtual void FromBlock(StellarisBlock block)
    {
        Id = block.Name;
    }
}