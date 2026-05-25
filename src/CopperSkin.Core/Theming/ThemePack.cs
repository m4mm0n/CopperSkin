namespace CopperSkin.Core.Theming;

public sealed class ThemePack
{
    public string Id { get; set; } = "copperskin";

    public string Name { get; set; } = "CopperSkin";

    public string Version { get; set; } = "0.1.0";

    public List<ThemeDefinition> Themes { get; set; } = new();

    public ThemeDefinition? FindTheme(string idOrName)
    {
        return Themes.FirstOrDefault(theme =>
            string.Equals(theme.Id, idOrName, StringComparison.OrdinalIgnoreCase) ||
            string.Equals(theme.Name, idOrName, StringComparison.OrdinalIgnoreCase) ||
            string.Equals(ThemeNames.Normalize(theme.Name), ThemeNames.Normalize(idOrName), StringComparison.OrdinalIgnoreCase));
    }
}
