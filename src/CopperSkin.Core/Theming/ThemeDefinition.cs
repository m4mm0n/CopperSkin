namespace CopperSkin.Core.Theming;

public sealed class ThemeDefinition
{
    public string Id { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string? BaseThemeId { get; set; }

    public Dictionary<string, string> Tokens { get; set; } = new(StringComparer.OrdinalIgnoreCase);

    public Dictionary<string, string> Metadata { get; set; } = new(StringComparer.OrdinalIgnoreCase);

    public ThemeDefinition Clone()
    {
        return new ThemeDefinition
        {
            Id = Id,
            Name = Name,
            BaseThemeId = BaseThemeId,
            Tokens = new Dictionary<string, string>(Tokens, StringComparer.OrdinalIgnoreCase),
            Metadata = new Dictionary<string, string>(Metadata, StringComparer.OrdinalIgnoreCase)
        };
    }
}
