/*
 * ====================================================================================================
 *  Project        : CopperSkin
 *  File           : src\CopperSkin.Core\Theming\ThemeJsonSerializer.cs
 *  Author         : Geir Gustavsen, ZeroLinez Softworx 2024 - 2026
 *  Created        : 2026-05-25 09:31:37 +02:00
 *  Last Modified  : 2026-05-25 11:25:22 +02:00
 *  CRC32          : E71B2DCF
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
// CRC32-BODY: E71B2DCF
using System.Text.Json;

namespace CopperSkin.Core.Theming;

/// <summary>
/// Reads and writes CopperSkin theme packs using the canonical JSON representation.
/// </summary>
public static class ThemeJsonSerializer
{
    /// <summary>
    /// Stores the canonical JSON formatting options used for CopperSkin theme packs.
    /// </summary>
    private static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true
    };

    /// <summary>
    /// Reads a CopperSkin theme pack from disk.
    /// </summary>
    public static ThemePack ReadPack(string path)
    {
        var json = File.ReadAllText(path);
        return JsonSerializer.Deserialize<ThemePack>(json, Options) ?? new ThemePack();
    }

    /// <summary>
    /// Writes a CopperSkin theme pack to disk with stable, indented JSON.
    /// </summary>
    public static void WritePack(string path, ThemePack pack)
    {
        var directory = Path.GetDirectoryName(path);
        if (!string.IsNullOrWhiteSpace(directory))
            Directory.CreateDirectory(directory);

        File.WriteAllText(path, JsonSerializer.Serialize(pack, Options));
    }
}
