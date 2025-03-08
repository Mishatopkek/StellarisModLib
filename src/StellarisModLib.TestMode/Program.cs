// Create a mod manager with path to your mod folder

using StellarisModLib.Core;
using StellarisModLib.Core.Entities;
using StellarisModLib.Core.Objects;

StellarisModManager modManager = new("Csharp test mod 2");

// Update the mod descriptor
modManager.UpdateModDescriptor(
    "0.1",
    "v3.14.15926", // Current Stellaris version
    ["Utilities",
        "Total Conversion",
        "Species",
        "Military",
        "Sound",
        "Leaders",
        "Gameplay",
        "Font",
        "Events",
        "Diplomacy"]
);

// Add a custom civic
Civic civic = new()
{
    Id = "civic_scientific_legacy",
    Description = "This civilization has a long history of scientific discovery and innovation.",
    RequiredEthics = [EthicsType.Materialist],
    RequiredGovernments =
    [
        GovernmentType.Democratic,
        GovernmentType.Oligarchic
    ],
    ExcludedCivics = ["civic_agrarian_idyll", "civic_warrior_culture"],
    Modifiers =
    [
        "science_output_mult = 0.15",
        "research_from_pops_mult = 0.1"
    ]
};

modManager.AddCivic(civic);

// Add a custom trait
Trait trait = new()
{
    Id = "trait_quantum_brain",
    Name = "Quantum Brain",
    Description =
        "Members of this species have naturally occurring quantum structures in their brains, allowing for exceptional cognitive abilities.",
    Cost = 3,
    Modifiers = new Dictionary<string, string>
    {
        {"planet_researchers_physics_research_produces_mult", "0.15"},
        {"leader_trait_scientist_expertise_particles_research_speed_mult", "0.15"}
    },
    AllowedClasses =
    [
        SpeciesClass.Humanoid,
        SpeciesClass.Mammalian,
        SpeciesClass.Avian
    ],
    ExcludedTraits =
    [
        "trait_nerve_stapled",
        "trait_pre_sapient"
    ]
};

modManager.AddTrait(trait);

// Add localizations
modManager.AddLocalization(new Dictionary<string, string>
{
    {"civic_scientific_legacy", "Scientific Legacy"},
    {"civic_scientific_legacy_desc", "This civilization has a long history of scientific discovery and innovation."},
    {"trait_quantum_brain", "Quantum Brain"},
    {
        "trait_quantum_brain_desc",
        "Members of this species have naturally occurring quantum structures in their brains, allowing for exceptional cognitive abilities."
    }
});

// Create a simple event
StellarisBlock optionBlock = new()
{
    Name = "option"
};
optionBlock.Children.Add(new StellarisProperty {Key = "name", Value = "quantum_discovery.1.a"});
optionBlock.Children.Add(new StellarisProperty
{
    Key = "add_modifier", Value = new StellarisBlock
    {
        Children =
        [
            new StellarisProperty {Key = "modifier", Value = "quantum_research_boost"},
            new StellarisProperty {Key = "days", Value = "720"}
        ]
    }
});

modManager.AddEvent(
    "quantum_discovery.1",
    "quantum_discovery.1.name",
    "quantum_discovery.1.desc",
    [optionBlock]
);

// Add more localizations for the event
modManager.AddLocalization(new Dictionary<string, string>
{
    {"quantum_discovery.1.name", "Quantum Discovery"},
    {
        "quantum_discovery.1.desc",
        "Our scientists have made a breakthrough in quantum physics, opening up new research possibilities."
    },
    {"quantum_discovery.1.a", "Fascinating!"},
    {"quantum_research_boost", "Quantum Research Breakthrough"},
    {"quantum_research_boost_desc", "A recent breakthrough has accelerated our quantum research capabilities."}
});

// Create a static modifier
string staticModifiersPath = "common/static_modifiers/mod_static_modifiers.txt";
StellarisDocument staticModifiersDoc = modManager.CreateFile(staticModifiersPath);

StellarisBlock modifierBlock = new()
{
    Name = "quantum_research_boost"
};
modifierBlock.Children.Add(new StellarisProperty {Key = "physics_research_speed_mult", Value = "0.25"});

staticModifiersDoc.Children.Add(modifierBlock);

// Save all changes
modManager.SaveAllFiles();

Console.WriteLine("Mod successfully created!");