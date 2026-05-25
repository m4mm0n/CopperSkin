/*
 * ====================================================================================================
 *  Project        : CopperSkin
 *  File           : src\CopperSkin.Core\Theming\ThemeResolver.cs
 *  Author         : Geir Gustavsen, ZeroLinez Softworx 2024 - 2026
 *  Created        : 2026-05-25 09:31:37 +02:00
 *  Last Modified  : 2026-05-25 11:04:38 +02:00
 *  CRC32          : E899D070
 *
 *  Description    :
 *                   CopperSkin WPF theme engine source file with live theming, custom controls, and designer support.
 *
 *  License        :
 *                   MIT
 *                   https://opensource.org/licenses/MIT
 *
 *  Notes          :
 *                   WPF theme engine extracted from the amChipper custom skin.
 * ====================================================================================================
 */
// CRC32-BODY: E899D070
namespace CopperSkin.Core.Theming;

/// <summary>
/// Resolves CopperSkin theme inheritance, aliases, and derived runtime tokens.
/// </summary>
public sealed class ThemeResolver
{
    /// <summary>
    /// Resolves the requested theme from a pack into a complete runtime token set.
    /// </summary>
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

    /// <summary>
    /// Recursively merges inherited theme tokens while detecting base-theme cycles.
    /// </summary>
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

    /// <summary>
    /// Populates legacy amChipper token names from their canonical CopperSkin equivalents.
    /// </summary>
    private static void AddLegacyAliases(Dictionary<string, string> tokens)
    {
        foreach (KeyValuePair<string, string> pair in LegacyTokenAliases.Map)
        {
            if (tokens.TryGetValue(pair.Value, out string? value))
                tokens[pair.Key] = value ?? string.Empty;
        }
    }

    /// <summary>
    /// Adds runtime convenience tokens for chrome, drawing surfaces, metrics, fonts, and effects.
    /// </summary>
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

    /// <summary>
    /// Copies a token value into a derived key only when the destination is not already defined.
    /// </summary>
    private static void Copy(Dictionary<string, string> tokens, string from, string to)
    {
        if (tokens.TryGetValue(from, out string? value))
            AddIfMissing(tokens, to, value);
    }

    /// <summary>
    /// Adds a default token value without overwriting a value supplied by the theme.
    /// </summary>
    private static void AddIfMissing(Dictionary<string, string> tokens, string key, string value)
    {
        if (!tokens.ContainsKey(key))
            tokens[key] = value;
    }
}
