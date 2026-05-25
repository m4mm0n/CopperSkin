using System.IO.Compression;

namespace CopperSkin.Core.Theming;

public static class ThemePackArchive
{
    public static void PackDirectory(string sourceDirectory, string outputPath)
    {
        if (!Directory.Exists(sourceDirectory))
            throw new DirectoryNotFoundException(sourceDirectory);

        string? outputDirectory = Path.GetDirectoryName(outputPath);
        if (!string.IsNullOrWhiteSpace(outputDirectory))
            Directory.CreateDirectory(outputDirectory);

        if (File.Exists(outputPath))
            File.Delete(outputPath);

        ZipFile.CreateFromDirectory(sourceDirectory, outputPath, CompressionLevel.Optimal, includeBaseDirectory: false);
    }

    public static void Unpack(string archivePath, string outputDirectory)
    {
        if (Directory.Exists(outputDirectory))
            Directory.Delete(outputDirectory, recursive: true);

        ZipFile.ExtractToDirectory(archivePath, outputDirectory);
    }
}
