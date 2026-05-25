namespace CopperSkin.Core.Theming;

public sealed class ThemeResolver
{
    public ResolvedTheme Resolve(ThemePack pack, string idOrName)
    {
        if (pack is null)
            throw new ArgumentNullException(nameof(pack));

        ThemeDefinition theme = pack.FindTheme(idOrName)
            ?? throw new InvalidOperationException($"Theme '{idOrName}' was not found in pack '{pack.Name}'.");

        var merged = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        MergeBaseThemes(pack, theme, merged, new HashSet<string>(StringComparer.OrdinalIgnoreCase));

        foreach (KeyValuePair<string, string> pair in theme.Tokens)
            merged[LegacyTokenAliases.Canonicalize(pair.Key)] = pair.Value;

        AddLegacyAliases(merged);
        AddDerivedTokens(merged);

        return new ResolvedTheme(theme.Id, theme.Name, merged);
    }

    private static void MergeBaseThemes(ThemePack pack, ThemeDefinition theme, Dictionary<string, string> merged, HashSet<string> seen)
    {
        if (string.IsNullOrWhiteSpace(theme.BaseThemeId))
            return;

        string baseThemeId = theme.BaseThemeId ?? string.Empty;
        if (!seen.Add(baseThemeId))
            throw new InvalidOperationException($"Theme inheritance cycle detected at '{baseThemeId}'.");

        ThemeDefinition? parent = pack.FindTheme(baseThemeId);
        if (parent is null)
            throw new InvalidOperationException($"Base theme '{baseThemeId}' was not found.");

        MergeBaseThemes(pack, parent, merged, seen);
        foreach (KeyValuePair<string, string> pair in parent.Tokens)
            merged[LegacyTokenAliases.Canonicalize(pair.Key)] = pair.Value;
    }

    private static void AddLegacyAliases(Dictionary<string, string> tokens)
    {
        foreach (KeyValuePair<string, string> pair in LegacyTokenAliases.Map)
        {
            if (tokens.TryGetValue(pair.Value, out string? value))
                tokens[pair.Key] = value ?? string.Empty;
        }
    }

    private static void AddDerivedTokens(Dictionary<string, string> tokens)
    {
        Copy(tokens, "color.surface.deep", "chrome.caption.background");
        Copy(tokens, "color.text.primary", "chrome.caption.foreground");
        Copy(tokens, "color.accent.primary", "chrome.caption.border");
        Copy(tokens, "color.editor.playhead", "PlayheadColor");
        Copy(tokens, "color.editor.grid.line", "GridLineColor");
        Copy(tokens, "color.editor.grid.bar", "GridBarColor");

        AddIfMissing(tokens, "metric.radius.xs", "2");
        AddIfMissing(tokens, "metric.radius.sm", "4");
        AddIfMissing(tokens, "metric.radius.md", "7");
        AddIfMissing(tokens, "metric.border.thin", "1");
        AddIfMissing(tokens, "metric.scrollbar.size", "17");
        AddIfMissing(tokens, "metric.density.scale", "1");
        AddIfMissing(tokens, "font.ui", "Segoe UI");
        AddIfMissing(tokens, "font.mono", "Consolas");
        AddIfMissing(tokens, "font.size.base", "12");
        AddIfMissing(tokens, "motion.transition.ms", "120");
        AddIfMissing(tokens, "effect.shine.opacity", "0.58");
        AddIfMissing(tokens, "effect.shadow.opacity", "0.34");
        AddIfMissing(tokens, "effect.glow.opacity", "0.50");
    }

    private static void Copy(Dictionary<string, string> tokens, string from, string to)
    {
        if (tokens.TryGetValue(from, out string? value))
            AddIfMissing(tokens, to, value);
    }

    private static void AddIfMissing(Dictionary<string, string> tokens, string key, string value)
    {
        if (!tokens.ContainsKey(key))
            tokens[key] = value;
    }
}
