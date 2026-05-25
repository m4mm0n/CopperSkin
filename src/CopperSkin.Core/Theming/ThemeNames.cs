/*
 * ====================================================================================================
 *  Project        : CopperSkin
 *  File           : src\CopperSkin.Core\Theming\ThemeNames.cs
 *  Author         : Geir Gustavsen, ZeroLinez Softworx 2024 - 2026
 *  Created        : 2026-05-25 09:31:37 +02:00
 *  Last Modified  : 2026-05-25 11:07:57 +02:00
 *  CRC32          : 7778EB29
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
// CRC32-BODY: 7778EB29
namespace CopperSkin.Core.Theming;

/// <summary>
/// Normalizes and slugs theme names for lookup, ids, and export paths.
/// </summary>
public static class ThemeNames
{
    /// <summary>
    /// Normalizes a theme name for case-insensitive lookup.
    /// </summary>
    public static string Normalize(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return string.Empty;

        return value.Trim().ToUpperInvariant().Replace("_", " ").Replace("-", " ");
    }

    /// <summary>
    /// Converts a display name into a lowercase CopperSkin theme id slug.
    /// </summary>
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
