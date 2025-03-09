using StellarisModLib.Core.Entities;
using StellarisModLib.Core.Objects;
using StellarisModLib.Core.Parser;

namespace StellarisModLib.Core;

public class StellarisModManager
{
    public static readonly string ModsDirectory = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
        "Paradox Interactive", "Stellaris", "mod");

    private readonly Dictionary<string, StellarisDocument> _documents = new();
    private readonly string _modPath;
    private readonly string _name;
    private readonly string _authorPrefix;

    public StellarisModManager(string name, string authorPrefix = "csharp")
    {
        _name = name;
        _authorPrefix = authorPrefix;
        _modPath = Path.Combine(ModsDirectory, _name);

        // Create directories if they don't exist
        Directory.CreateDirectory(_modPath);
        Directory.CreateDirectory(Path.Combine(_modPath, "common"));
    }

    // Load an existing mod file
    public StellarisDocument LoadFile(string relativePath)
    {
        string fullPath = Path.Combine(_modPath, relativePath);

        if (!File.Exists(fullPath))
            throw new FileNotFoundException($"File not found: {fullPath}");

        string content = File.ReadAllText(fullPath);
        FormattingPreservingParser parser = new(content);
        StellarisDocument document = parser.Parse(relativePath);

        _documents[relativePath] = document;
        return document;
    }

    // Create a new mod file
    public StellarisDocument CreateFile(string relativePath)
    {
        StellarisDocument document = new() {FileName = relativePath};
        CreateFile(relativePath, _modPath, document);
        return document;
    }

    private StellarisDocumentDescriptor CreateDescriptor(string relativePath)
    {
        StellarisDocumentDescriptor document = new() {FileName = relativePath, ModName = _name};
        CreateFile(relativePath, _modPath, document);
        return document;
    }

    private void CreateFile(string relativePath, string absolutePath, StellarisDocument document)
    {
        _documents[relativePath] = document;

        // Ensure directory exists
        string directory = Path.GetDirectoryName(Path.Combine(absolutePath, relativePath));
        if (!string.IsNullOrEmpty(directory))
            Directory.CreateDirectory(directory);
    }

    // Save all modified files
    public void SaveAllFiles()
    {
        foreach (StellarisDocument document in _documents.Values)
        {
            document.SaveToFile(_modPath);
        }
    }

    // Save a specific file
    public void SaveFile(string relativePath)
    {
        if (_documents.TryGetValue(relativePath, out StellarisDocument? document))
        {
            document.SaveToFile(_modPath);
        }
    }

    // Add generic object to a document
    public void AddObjectToDocument(string relativePath, StellarisObject obj)
    {
        if (!_documents.TryGetValue(relativePath, out StellarisDocument? document))
        {
            document = CreateFile(relativePath);
        }

        document.Children.Add(obj);
    }

    #region Civics Management

    // Add a civic to the mod
    public void AddCivic(Civic civic)
    {
        string civicsPath = $"common/governments/civics/{_authorPrefix}_civics.txt";

        if (!_documents.TryGetValue(civicsPath, out StellarisDocument? document))
        {
            document = CreateFile(civicsPath);
        }

        StellarisBlock block = civic.ToBlock();
        document.Children.Add(block);
    }

    // Get all civics from the mod
    public List<Civic> GetAllCivics()
    {
        List<Civic> civics = [];

        foreach (StellarisDocument document in _documents.Values)
        {
            if (document.FileName.Contains("civics"))
            {
                foreach (StellarisObject child in document.Children)
                {
                    if (child is StellarisBlock block)
                    {
                        Civic civic = new();
                        civic.FromBlock(block);
                        civics.Add(civic);
                    }
                }
            }
        }

        return civics;
    }

    #endregion

    #region Traits Management

    // Add a trait to the mod
    public void AddTrait(Trait trait)
    {
        string traitsPath = "common/traits/mod_traits.txt";

        if (!_documents.TryGetValue(traitsPath, out StellarisDocument? document))
        {
            document = CreateFile(traitsPath);
        }

        StellarisBlock block = trait.ToBlock();
        document.Children.Add(block);
    }

    // Get all traits from the mod
    public List<Trait> GetAllTraits()
    {
        List<Trait> traits = [];

        foreach (StellarisDocument document in _documents.Values)
        {
            if (document.FileName.Contains("traits"))
            {
                foreach (StellarisObject child in document.Children)
                {
                    if (child is StellarisBlock block)
                    {
                        Trait trait = new();
                        trait.FromBlock(block);
                        traits.Add(trait);
                    }
                }
            }
        }

        return traits;
    }

    #endregion

    #region Localization Management

    // Add localization entries to the mod
    public void AddLocalization(Dictionary<string, string> entries, string language = "english")
    {
        string locPath = $"localisation/{language}/{_authorPrefix}_l_{language}.yml";

        if (!_documents.TryGetValue(locPath, out StellarisDocument? document))
        {
            document = CreateFile(locPath);

            // Add YAML header
            document.Children.Add(new StellarisProperty
            {
                Key = $"l_{language}:",
                Value = string.Empty,
                Operator = string.Empty
            });
        }

        foreach (KeyValuePair<string, string> entry in entries)
        {
            document.Children.Add(new StellarisProperty
            {
                Key = $" {entry.Key}:0",
                Value = $"\"{entry.Value}\"",
                Operator = string.Empty
            });
        }
    }

    #endregion

    #region Events Management

    // Add an event to the mod
    public void AddEvent(string eventId, string title, string description, List<StellarisObject> options)
    {
        string eventsPath = "events/mod_events.txt";

        if (!_documents.TryGetValue(eventsPath, out StellarisDocument? document))
        {
            document = CreateFile(eventsPath);
        }

        StellarisBlock eventBlock = new()
        {
            Name = "country_event"
        };

        eventBlock.Children.Add(new StellarisProperty {Key = "id", Value = eventId});
        eventBlock.Children.Add(new StellarisProperty {Key = "title", Value = title});
        eventBlock.Children.Add(new StellarisProperty {Key = "desc", Value = description});

        foreach (StellarisObject option in options)
        {
            eventBlock.Children.Add(option);
        }

        document.Children.Add(eventBlock);
    }

    #endregion

    #region Descriptor Management

    // Create or update the mod descriptor file
    public void UpdateModDescriptor(string version, string supportedVersion, string[]? tags)
    {
        const string descriptorPath = "descriptor.mod";

        if (tags is {Length: > 10})
        {
            throw new ArgumentOutOfRangeException(nameof(tags), "Tags must be less than 10.");
        }

        if (!_documents.TryGetValue(descriptorPath, out StellarisDocument? document))
        {
            document = CreateDescriptor(descriptorPath);
        }
        else
        {
            document.Children.Clear();
        }

        document.Children.Add(new StellarisProperty {Key = "version", Value = $"\"{version}\""});
        if (tags != null)
        {
            document.Children.Add(new StellarisProperty
            {
                Key = "tags", Operator = string.Empty, Value = new StellarisBlock
                {
                    Children = tags
                        .Select(tag => new StellarisValue {Value = tag})
                        .Cast<StellarisObject>()
                        .ToList()
                }
            });
        }
        document.Children.Add(new StellarisProperty {Key = "name", Value = $"\"{_name}\""});
        document.Children.Add(new StellarisProperty {Key = "supported_version", Value = $"\"{supportedVersion}\""});
        document.Children.Add(new StellarisProperty {Key = "path", Value = $"\"{_modPath.Replace("\\", "/")}\""});
    }

    #endregion
}