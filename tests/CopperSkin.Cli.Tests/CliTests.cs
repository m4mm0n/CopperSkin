/*
 * ====================================================================================================
 *  Project        : CopperSkin
 *  File           : tests\CopperSkin.Cli.Tests\CliTests.cs
 *  Author         : Geir Gustavsen, ZeroLinez Softworx 2024 - 2026
 *  Created        : 2026-05-25 09:40:59 +02:00
 *  Last Modified  : 2026-06-08 19:36:14 +02:00
 *  CRC32          : 9F48A340
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
// CRC32-BODY: 9F48A340

namespace CopperSkin.Cli.Tests;

/// <summary>
/// Verifies the CopperSkin CLI command paths used by release packaging.
/// </summary>
public sealed class CliTests
{
    /// <summary>
    /// Verifies the List Command Succeeds behavior.
    /// </summary>
    [Fact]
    public void ListCommandSucceeds()
    {
        var exitCode = Program.Main(["list"]);

        Assert.Equal(0, exitCode);
    }

    /// <summary>
    /// Verifies the Export And Validate Built Ins Succeeds behavior.
    /// </summary>
    [Fact]
    public void ExportAndValidateBuiltInsSucceeds()
    {
        var root = Path.Combine(Path.GetTempPath(), "copperskin-cli-" + Guid.NewGuid().ToString("N"));
        var pack = Path.Combine(root, "theme-pack.json");

        Assert.Equal(0, Program.Main(["export-builtins", pack]));
        Assert.True(File.Exists(pack));
        Assert.Equal(0, Program.Main(["validate", pack]));
    }

    /// <summary>
    /// Verifies the Authoring Commands Create Assets behavior.
    /// </summary>
    [Fact]
    public void AuthoringCommandsCreateAssets()
    {
        var root = Path.Combine(Path.GetTempPath(), "copperskin-authoring-" + Guid.NewGuid().ToString("N"));
        var scaffold = Path.Combine(root, "starter");
        var pack = Path.Combine(scaffold, "theme-pack.json");
        var gallery = Path.Combine(root, "gallery");
        var baseline = Path.Combine(root, "visual-baseline.json");

        Assert.Equal(0, Program.Main(["tokens"]));
        Assert.Equal(0, Program.Main(["adapters"]));
        Assert.Equal(0, Program.Main(["scaffold", scaffold]));
        Assert.True(File.Exists(pack));
        Assert.Equal(0, Program.Main(["sign", pack]));
        Assert.Equal(0, Program.Main(["verify-signature", pack]));
        Assert.Equal(0, Program.Main(["gallery", pack, gallery]));
        Assert.True(File.Exists(Path.Combine(gallery, "index.html")));
        Assert.True(File.Exists(Path.Combine(gallery, "visual-baseline.json")));
        Assert.Equal(0, Program.Main(["baseline", pack, baseline]));
        Assert.True(File.Exists(baseline));
    }

    /// <summary>
    /// Verifies the Migration Command Creates Report behavior.
    /// </summary>
    [Fact]
    public void MigrationCommandCreatesReport()
    {
        var root = Path.Combine(Path.GetTempPath(), "copperskin-migrate-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(root);
        File.WriteAllText(Path.Combine(root, "View.xaml"), "<Grid Background=\"#FF000000\"/>");
        var report = Path.Combine(root, "report.md");

        Assert.Equal(0, Program.Main(["migrate", root, report]));

        Assert.Contains("CopperSkin Migration Report", File.ReadAllText(report));
    }

    /// <summary>
    /// Verifies the Lzhc Command Creates Release Archive behavior.
    /// </summary>
    [Fact]
    public void LzhcCommandCreatesReleaseArchive()
    {
        var root = Path.Combine(Path.GetTempPath(), "copperskin-lzhc-" + Guid.NewGuid().ToString("N"));
        var source = Path.Combine(root, "source");
        Directory.CreateDirectory(source);
        File.WriteAllText(Path.Combine(source, "release.txt"), "CopperSkin");
        var output = Path.Combine(root, "CopperSkin.lzhc");

        Assert.Equal(0, Program.Main(["lzhc", source, output]));
        Assert.True(File.Exists(output));
        var magic = File.ReadAllBytes(output).Take(7).ToArray();
        Assert.Equal("CSLZHC1", System.Text.Encoding.ASCII.GetString(magic));
    }
}
