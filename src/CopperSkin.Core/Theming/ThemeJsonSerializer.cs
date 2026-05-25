using System.Text.Json;

namespace CopperSkin.Core.Theming;

public static class ThemeJsonSerializer
{
    private static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true
    };

    public static ThemePack ReadPack(string path)
    {
        string json = File.ReadAllText(path);
        return JsonSerializer.Deserialize<ThemePack>(json, Options) ?? new ThemePack();
    }

    public static void WritePack(string path, ThemePack pack)
    {
        string? directory = Path.GetDirectoryName(path);
        if (!string.IsNullOrWhiteSpace(directory))
            Directory.CreateDirectory(directory);

        File.WriteAllText(path, JsonSerializer.Serialize(pack, Options));
    }
}
