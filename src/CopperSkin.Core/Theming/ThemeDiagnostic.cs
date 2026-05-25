/*
 * ====================================================================================================
 *  Project        : CopperSkin
 *  File           : src\CopperSkin.Core\Theming\ThemeDiagnostic.cs
 *  Author         : Geir Gustavsen, ZeroLinez Softworx 2024 - 2026
 *  Created        : 2026-05-25 09:31:37 +02:00
 *  Last Modified  : 2026-05-25 11:25:22 +02:00
 *  CRC32          : 9097905F
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
// CRC32-BODY: 9097905F
namespace CopperSkin.Core.Theming;

/// <summary>
/// Carries one validation result produced while checking a CopperSkin theme pack.
/// </summary>
public sealed record ThemeDiagnostic(string Severity, string Code, string Message, string? Path = null)
{
    /// <summary>
    /// Formats the value as a concise human-readable string.
    /// </summary>
    public override string ToString() => string.IsNullOrWhiteSpace(Path)
        ? $"{Severity} {Code}: {Message}"
        : $"{Severity} {Code} at {Path}: {Message}";
}
