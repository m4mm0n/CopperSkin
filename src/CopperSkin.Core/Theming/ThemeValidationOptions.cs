/*
 * ====================================================================================================
 *  Project        : CopperSkin
 *  File           : src\CopperSkin.Core\Theming\ThemeValidationOptions.cs
 *  Author         : Geir Gustavsen, ZeroLinez Softworx 2024 - 2026
 *  Created        : 2026-06-08 00:00:00 +02:00
 *  Last Modified  : 2026-06-08 19:36:14 +02:00
 *  CRC32          : F0C2BF6C
 *
 *  Description    :
 *                   Configures CopperSkin theme validation strictness and diagnostics.
 *
 *  License        :
 *                   MIT
 *                   https://opensource.org/licenses/MIT
 *
 *  Notes          :
 *                   WPF theme engine extracted from the amChipper custom skin.
 * ====================================================================================================
 */
// CRC32-BODY: F0C2BF6C
namespace CopperSkin.Core.Theming;

/// <summary>
/// Configures how strictly CopperSkin validates token packs.
/// </summary>
public sealed class ThemeValidationOptions
{
    /// <summary>
    /// Gets or sets whether contrast warnings are promoted to errors.
    /// </summary>
    public bool StrictContrast { get; set; }

    /// <summary>
    /// Gets or sets whether unknown token keys are reported.
    /// </summary>
    public bool WarnUnknownTokens { get; set; } = true;

    /// <summary>
    /// Gets or sets whether recommended non-critical tokens should be reported when missing.
    /// </summary>
    public bool RequireRecommendedTokens { get; set; }
}
