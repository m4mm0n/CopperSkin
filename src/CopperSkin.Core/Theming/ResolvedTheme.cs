// copperskin:allow-hardcoded-color-file
namespace CopperSkin.Core.Theming;

public sealed class ResolvedTheme
{
    public ResolvedTheme(string id, string name, IReadOnlyDictionary<string, string> tokens)
    {
        Id = id;
        Name = name;
        Tokens = tokens;
    }

    public string Id { get; }

    public string Name { get; }

    public IReadOnlyDictionary<string, string> Tokens { get; }

    public string Get(string key, string fallback = "#FFFF00FF")
    {
        return Tokens.TryGetValue(key, out string? value) ? value : fallback;
    }
}
