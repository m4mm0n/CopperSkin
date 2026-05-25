/*
 * ====================================================================================================
 *  Project        : CopperSkin
 *  File           : tests\CopperSkin.Cli.Tests\CliTests.cs
 *  Author         : Geir Gustavsen, ZeroLinez Softworx 2024 - 2026
 *  Created        : 2026-05-25 09:40:59 +02:00
 *  Last Modified  : 2026-05-25 11:04:38 +02:00
 *  CRC32          : DF48E882
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
// CRC32-BODY: DF48E882
using CopperSkin.Cli;

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
        int exitCode = Program.Main(["list"]);

        Assert.Equal(0, exitCode);
    }

    /// <summary>
    /// Verifies the Export And Validate Built Ins Succeeds behavior.
    /// </summary>
    [Fact]
    public void ExportAndValidateBuiltInsSucceeds()
    {
        string root = Path.Combine(Path.GetTempPath(), "copperskin-cli-" + Guid.NewGuid().ToString("N"));
        string pack = Path.Combine(root, "theme-pack.json");

        Assert.Equal(0, Program.Main(["export-builtins", pack]));
        Assert.True(File.Exists(pack));
        Assert.Equal(0, Program.Main(["validate", pack]));
    }

    /// <summary>
    /// Verifies the Lzhc Command Creates Release Archive behavior.
    /// </summary>
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
