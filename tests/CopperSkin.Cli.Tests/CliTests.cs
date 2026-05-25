using CopperSkin.Cli;

namespace CopperSkin.Cli.Tests;

public sealed class CliTests
{
    [Fact]
    public void ListCommandSucceeds()
    {
        int exitCode = Program.Main(["list"]);

        Assert.Equal(0, exitCode);
    }

    [Fact]
    public void ExportAndValidateBuiltInsSucceeds()
    {
        string root = Path.Combine(Path.GetTempPath(), "copperskin-cli-" + Guid.NewGuid().ToString("N"));
        string pack = Path.Combine(root, "theme-pack.json");

        Assert.Equal(0, Program.Main(["export-builtins", pack]));
        Assert.True(File.Exists(pack));
        Assert.Equal(0, Program.Main(["validate", pack]));
    }

    [Fact]
    public void LzhcCommandCreatesReleaseArchive()
    {
        string root = Path.Combine(Path.GetTempPath(), "copperskin-lzhc-" + Guid.NewGuid().ToString("N"));
        string source = Path.Combine(root, "source");
        Directory.CreateDirectory(source);
        File.WriteAllText(Path.Combine(source, "release.txt"), "CopperSkin");
        string output = Path.Combine(root, "CopperSkin.lzhc");

        Assert.Equal(0, Program.Main(["lzhc", source, output]));
        Assert.True(File.Exists(output));
        byte[] magic = File.ReadAllBytes(output).Take(7).ToArray();
        Assert.Equal("CSLZHC1", System.Text.Encoding.ASCII.GetString(magic));
    }
}
