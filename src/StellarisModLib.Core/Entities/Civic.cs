using StellarisModLib.Core.Objects;

namespace StellarisModLib.Core.Entities;

/// <summary>
/// Represents a civic in Stellaris
/// </summary>
public class Civic : StellarisEntity
{
    public string Name { get; set; }
    public string Description { get; set; }
    public bool IsCorporate { get; set; }
    public List<EthicsType> RequiredEthics { get; set; } = [];
    public List<GovernmentType> RequiredGovernments { get; set; } = [];
    public List<string> Modifiers { get; set; } = [];
    public List<string> RequiredCivics { get; set; } = [];
    public List<string> ExcludedCivics { get; set; } = [];
        
    public override StellarisBlock ToBlock()
    {
        StellarisBlock block = base.ToBlock();
            
        block.Children.Add(new StellarisProperty { Key = "name", Value = Name });
        block.Children.Add(new StellarisProperty { Key = "description", Value = Description });
            
        if (IsCorporate)
            block.Children.Add(new StellarisProperty { Key = "corporate", Value = "yes" });
            
        if (RequiredEthics.Any())
        {
            StellarisCondition ethicsCondition = new() { Operator = "OR" };
            foreach (EthicsType ethics in RequiredEthics)
            {
                ethicsCondition.Operands.Add(new StellarisProperty 
                { 
                    Key = "has_ethic", 
                    Value = ethics.ToString().ToLower() 
                });
            }
            block.Children.Add(ethicsCondition);
        }
            
        if (RequiredGovernments.Any())
        {
            StellarisCondition govCondition = new() { Operator = "OR" };
            foreach (GovernmentType government in RequiredGovernments)
            {
                govCondition.Operands.Add(new StellarisProperty 
                { 
                    Key = "authority", 
                    Value = government.ToString().ToLower() 
                });
            }
            block.Children.Add(govCondition);
        }
            
        if (Modifiers.Any())
        {
            foreach (string modifier in Modifiers)
            {
                string[] parts = modifier.Split('=');
                if (parts.Length == 2)
                {
                    block.Children.Add(new StellarisProperty 
                    { 
                        Key = parts[0].Trim(), 
                        Value = parts[1].Trim() 
                    });
                }
            }
        }
            
        if (RequiredCivics.Any())
        {
            StellarisCondition requiredCondition = new() { Operator = "AND" };
            foreach (string civic in RequiredCivics)
            {
                requiredCondition.Operands.Add(new StellarisProperty 
                { 
                    Key = "has_civic", 
                    Value = civic 
                });
            }
            block.Children.Add(requiredCondition);
        }
            
        if (ExcludedCivics.Any())
        {
            StellarisCondition excludedCondition = new() { Operator = "NOR" };
            foreach (string civic in ExcludedCivics)
            {
                excludedCondition.Operands.Add(new StellarisProperty 
                { 
                    Key = "has_civic", 
                    Value = civic 
                });
            }
            block.Children.Add(excludedCondition);
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
                    case "corporate":
                        IsCorporate = property.Value.ToString() == "yes";
                        break;
                }
            }
            else if (child is StellarisCondition condition)
            {
                if (condition.Operator == "OR")
                {
                    foreach (StellarisObject operand in condition.Operands)
                    {
                        if (operand is StellarisProperty prop)
                        {
                            if (prop.Key == "has_ethic" && Enum.TryParse<EthicsType>(
                                    prop.Value.ToString().Trim('"').ToFirstUpper(), out EthicsType ethic))
                            {
                                RequiredEthics.Add(ethic);
                            }
                            else if (prop.Key == "authority" && Enum.TryParse<GovernmentType>(
                                         prop.Value.ToString().Trim('"').ToFirstUpper(), out GovernmentType gov))
                            {
                                RequiredGovernments.Add(gov);
                            }
                        }
                    }
                }
                else if (condition.Operator == "AND")
                {
                    foreach (StellarisObject operand in condition.Operands)
                    {
                        if (operand is StellarisProperty prop && prop.Key == "has_civic")
                        {
                            RequiredCivics.Add(prop.Value.ToString().Trim('"'));
                        }
                    }
                }
                else if (condition.Operator == "NOR")
                {
                    foreach (StellarisObject operand in condition.Operands)
                    {
                        if (operand is StellarisProperty prop && prop.Key == "has_civic")
                        {
                            ExcludedCivics.Add(prop.Value.ToString().Trim('"'));
                        }
                    }
                }
            }
        }
    }
}