/*
 * ====================================================================================================
 *  Project        : CopperSkin
 *  File           : src\CopperSkin.Core\Theming\ThemeTokenKey.cs
 *  Author         : Geir Gustavsen, ZeroLinez Softworx 2024 - 2026
 *  Created        : 2026-05-25 09:31:37 +02:00
 *  Last Modified  : 2026-05-25 11:25:22 +02:00
 *  CRC32          : 61321C55
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
// CRC32-BODY: 61321C55
namespace CopperSkin.Core.Theming;

/// <summary>
/// Represents a strongly named CopperSkin token key for APIs that should avoid raw string ambiguity.
/// </summary>
public readonly record struct ThemeTokenKey(string Value)
{

    /// <summary>
    /// Creates a validated value object from raw text.
    /// </summary>
    public static ThemeTokenKey From(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Theme token key cannot be empty.", nameof(value));

        return new ThemeTokenKey(value.Trim());
    }
    /// <summary>
    /// Formats the value as a concise human-readable string.
    /// </summary>
    public override string ToString() => Value;
}
