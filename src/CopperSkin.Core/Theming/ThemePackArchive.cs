/*
 * ====================================================================================================
 *  Project        : CopperSkin
 *  File           : src\CopperSkin.Core\Theming\ThemePackArchive.cs
 *  Author         : Geir Gustavsen, ZeroLinez Softworx 2024 - 2026
 *  Created        : 2026-05-25 09:31:37 +02:00
 *  Last Modified  : 2026-05-25 11:25:22 +02:00
 *  CRC32          : 0E3B960D
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
// CRC32-BODY: 0E3B960D
using System.IO.Compression;

namespace CopperSkin.Core.Theming;

/// <summary>
/// Creates and extracts zip-backed CopperSkin .cskin theme archives.
/// </summary>
public static class ThemePackArchive
{
    /// <summary>
    /// Packs a directory into a .cskin archive while preserving relative paths.
    /// </summary>
    public static void PackDirectory(string sourceDirectory, string outputPath)
    {
        if (!Directory.Exists(sourceDirectory))
            throw new DirectoryNotFoundException(sourceDirectory);

        var outputDirectory = Path.GetDirectoryName(outputPath);
        if (!string.IsNullOrWhiteSpace(outputDirectory))
            Directory.CreateDirectory(outputDirectory);

        if (File.Exists(outputPath))
            File.Delete(outputPath);

        ZipFile.CreateFromDirectory(sourceDirectory, outputPath, CompressionLevel.Optimal, includeBaseDirectory: false);
    }

    /// <summary>
    /// Extracts a .cskin archive to a target directory.
    /// </summary>
    public static void Unpack(string archivePath, string outputDirectory)
    {
        if (Directory.Exists(outputDirectory))
            Directory.Delete(outputDirectory, recursive: true);

        ZipFile.ExtractToDirectory(archivePath, outputDirectory);
    }
}
