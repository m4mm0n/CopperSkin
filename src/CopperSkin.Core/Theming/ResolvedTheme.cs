/*
 * ====================================================================================================
 *  Project        : CopperSkin
 *  File           : src\CopperSkin.Core\Theming\ResolvedTheme.cs
 *  Author         : Geir Gustavsen, ZeroLinez Softworx 2024 - 2026
 *  Created        : 2026-05-25 09:31:37 +02:00
 *  Last Modified  : 2026-05-25 11:07:57 +02:00
 *  CRC32          : 1E3E27A8
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
// CRC32-BODY: 1E3E27A8
// copperskin:allow-hardcoded-color-file
namespace CopperSkin.Core.Theming;

/// <summary>
/// Represents an inheritance-resolved, alias-expanded CopperSkin theme ready for runtime use.
/// </summary>
public sealed class ResolvedTheme
{
    /// <summary>
    /// Creates an immutable resolved theme from a final token dictionary.
    /// </summary>
    public ResolvedTheme(string id, string name, IReadOnlyDictionary<string, string> tokens)
    {
        Id = id;
        Name = name;
        Tokens = tokens;
    }

    /// <summary>
    /// Gets the stable identifier of the resolved theme.
    /// </summary>
    public string Id { get; }

    /// <summary>
    /// Gets the display name of the resolved theme.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the final token dictionary after inheritance, aliases, and derived tokens are applied.
    /// </summary>
    public IReadOnlyDictionary<string, string> Tokens { get; }

    /// <summary>
    /// Returns a token value by key, or a visible fallback when the token is missing.
    /// </summary>
    public string Get(string key, string fallback = "#FFFF00FF")
    {
        return Tokens.TryGetValue(key, out string? value) ? value : fallback;
    }
}
