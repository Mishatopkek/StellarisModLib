using StellarisModLib.Core.Objects;

namespace StellarisModLib.Core.Entities;

/// <summary>
/// Represents a civic in Stellaris
/// </summary>
public class Civic : StellarisEntity
{
    public string? Description { get; set; }
    public StellarisTrigger? Potential { get; set; }
    public StellarisTrigger? Possible { get; set; }
    public List<string>? Modifiers { get; set; }

    public override StellarisBlock ToBlock()
    {
        StellarisBlock block = base.ToBlock();

        if (!string.IsNullOrEmpty(Description))
        {
            block.Children.Add(new StellarisProperty {Key = "description", Value = Description});
        }

        if (Potential != null)
        {
            StellarisBlock potential = new() {Name = "potential"};
            FillWithTriggers(potential, Potential);
            block.Children.Add(potential);
        }

        if (Possible != null)
        {
            StellarisBlock potential = new() {Name = "possible"};
            FillWithTriggers(potential, Possible);
            block.Children.Add(potential);
        }

        if (Modifiers != null && Modifiers.Any())
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

        return block;
    }

    private void FillWithTriggers(StellarisBlock block, StellarisTrigger item)
    {
        if (item.Ethics != null)
        {
            block.Children.Add(new StellarisProperty
            {
                Key = "ethics",
                Value = item.Ethics
            });
        }

        if (item.Authority != null)
        {
            block.Children.Add(new StellarisProperty
            {
                Key = "authority",
                Value = item.Authority
            });
        }
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
                    case "description":
                        Description = property.Value.ToString().Trim('"');
                        break;
                }
            }
        }
    }
}