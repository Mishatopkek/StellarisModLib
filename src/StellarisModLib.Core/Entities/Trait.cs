using StellarisModLib.Core.Objects;

namespace StellarisModLib.Core.Entities;

/// <summary>
/// Represents a trait in Stellaris
/// </summary>
public class Trait : StellarisEntity
{
    public string Name { get; set; }
    public string Description { get; set; }
    public int Cost { get; set; }
    public Dictionary<string, string> Modifiers { get; set; } = new();
    public List<string> RequiredTraits { get; set; } = [];
    public List<string> ExcludedTraits { get; set; } = [];
    public List<SpeciesClass> AllowedClasses { get; set; } = [];
        
    public override StellarisBlock ToBlock()
    {
        StellarisBlock block = base.ToBlock();
            
        block.Children.Add(new StellarisProperty { Key = "name", Value = Name });
        block.Children.Add(new StellarisProperty { Key = "description", Value = Description });
        block.Children.Add(new StellarisProperty { Key = "cost", Value = Cost.ToString() });
            
        // Add modifiers
        foreach (KeyValuePair<string, string> modifier in Modifiers)
        {
            block.Children.Add(new StellarisProperty 
            { 
                Key = modifier.Key, 
                Value = modifier.Value 
            });
        }
            
        // Handle allowed classes if specified
        if (AllowedClasses.Any())
        {
            StellarisBlock allowedBlock = new() { Name = "allowed_archetypes" };
            foreach (SpeciesClass species in AllowedClasses)
            {
                allowedBlock.Children.Add(new StellarisProperty 
                { 
                    Key = species.ToString().ToLower(), 
                    Value = "yes" 
                });
            }
            block.Children.Add(allowedBlock);
        }
            
        // Handle prerequisites if any
        if (RequiredTraits.Any())
        {
            StellarisBlock prerequisites = new() { Name = "prerequisites" };
            foreach (string trait in RequiredTraits)
            {
                prerequisites.Children.Add(new StellarisProperty { Key = trait, Value = "yes" });
            }
            block.Children.Add(prerequisites);
        }
            
        // Handle opposites/conflicts if any
        if (ExcludedTraits.Any())
        {
            StellarisBlock opposites = new() { Name = "opposites" };
            foreach (string trait in ExcludedTraits)
            {
                opposites.Children.Add(new StellarisProperty { Key = trait, Value = "yes" });
            }
            block.Children.Add(opposites);
        }
            
        return block;
    }
        
    public override void FromBlock(StellarisBlock block)
    {
        base.FromBlock(block);
            
        foreach (StellarisObject child in block.Children)
        {
            if (child is StellarisProperty property)
            {
                switch (property.Key)
                {
                    case "name":
                        Name = property.Value.ToString().Trim('"');
                        break;
                    case "description":
                        Description = property.Value.ToString().Trim('"');
                        break;
                    case "cost":
                        int.TryParse(property.Value.ToString(), out int cost);
                        Cost = cost;
                        break;
                    default:
                        // Any other property is considered a modifier
                        if (!property.Key.StartsWith("allowed_") && 
                            !property.Key.StartsWith("opposites") && 
                            !property.Key.StartsWith("prerequisites"))
                        {
                            Modifiers[property.Key] = property.Value.ToString();
                        }
                        break;
                }
            }
            else if (child is StellarisBlock childBlock)
            {
                if (childBlock.Name == "allowed_archetypes")
                {
                    foreach (StellarisObject item in childBlock.Children)
                    {
                        if (item is StellarisProperty prop && prop.Value.ToString() == "yes")
                        {
                            if (Enum.TryParse<SpeciesClass>(
                                    prop.Key.ToFirstUpper(), out SpeciesClass species))
                            {
                                AllowedClasses.Add(species);
                            }
                        }
                    }
                }
                else if (childBlock.Name == "prerequisites")
                {
                    foreach (StellarisObject item in childBlock.Children)
                    {
                        if (item is StellarisProperty prop && prop.Value.ToString() == "yes")
                        {
                            RequiredTraits.Add(prop.Key);
                        }
                    }
                }
                else if (childBlock.Name == "opposites")
                {
                    foreach (StellarisObject item in childBlock.Children)
                    {
                        if (item is StellarisProperty prop && prop.Value.ToString() == "yes")
                        {
                            ExcludedTraits.Add(prop.Key);
                        }
                    }
                }
            }
        }
    }
}