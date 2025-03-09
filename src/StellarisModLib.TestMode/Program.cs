using StellarisModLib.Core;
using StellarisModLib.Core.Entities;
using StellarisModLib.Core.Objects;

StellarisModManager modManager = new("Csharp test mod 2");

modManager.UpdateModDescriptor(
    "0.1",
    "v3.14.15926", // Current Stellaris version
    [
        "Utilities",
        "Total Conversion",
        "Species",
        "Military",
        "Sound",
        "Leaders",
        "Gameplay",
        "Font",
        "Events",
        "Diplomacy"
    ]
);

// Add a custom civic
Civic civic = new()
{
    Id = "civic_scientific_legacy",
    Description = "This civilization has a long history of scientific discovery and innovation.",
    Potential = new StellarisTrigger
    {
        Ethics = new StellarisCondition
        {
            ConditionOperator = "NOT",
            Operands =
            [
                new StellarisProperty
                {
                    Key = "value",
                    Value = "ethic_gestalt_consciousness",
                    ContainBrackets = false
                }
            ]
        },
        Authority = new StellarisCondition
        {
            ConditionOperator = "NOT",
            Operands =
            [
                new StellarisProperty
                {
                    Key = "value",
                    Value = "auth_corporate",
                    ContainBrackets = false
                }
            ]
        }
    },
    Possible = new StellarisTrigger
    {
        Ethics = new StellarisCondition
        {
            ConditionOperator = "OR",
            Operands =
            [
                new StellarisProperty
                {
                    Key = "text",
                    Value = "civic_tooltip_pacifist",
                    ContainBrackets = false
                },
                new StellarisProperty
                {
                    Key = "value",
                    Value = "ethic_pacifist",
                    ContainBrackets = false
                },
                new StellarisProperty
                {
                    Key = "value",
                    Value = "ethic_fanatic_pacifist",
                    ContainBrackets = false
                },
            ]
        }
    }
};

modManager.AddCivic(civic);

modManager.AddLocalization(new Dictionary<string, string>
{
    {"civic_scientific_legacy", "Scientific Legacy"},
    {"civic_scientific_legacy_desc", "This civilization has a long history of scientific discovery and innovation."}
});

modManager.SaveAllFiles();

Console.WriteLine("Mod successfully created!");