namespace CopperSkin.Core.Theming;

public static class ThemeNames
{
    public static string Normalize(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return string.Empty;

        return value.Trim().ToUpperInvariant().Replace("_", " ").Replace("-", " ");
    }

    public static string Slug(string value)
    {
        var chars = Normalize(value).ToLowerInvariant()
            .Select(ch => char.IsLetterOrDigit(ch) ? ch : '-')
            .ToArray();

        string slug = new(chars);
        while (slug.Contains("--"))
            slug = slug.Replace("--", "-");

        return slug.Trim('-');
    }
}
