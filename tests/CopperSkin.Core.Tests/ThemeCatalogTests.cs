/*
 * ====================================================================================================
 *  Project        : CopperSkin
 *  File           : tests\CopperSkin.Core.Tests\ThemeCatalogTests.cs
 *  Author         : Geir Gustavsen, ZeroLinez Softworx 2024 - 2026
 *  Created        : 2026-05-25 09:40:59 +02:00
 *  Last Modified  : 2026-05-25 11:07:57 +02:00
 *  CRC32          : 46226BDE
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
// CRC32-BODY: 46226BDE
using CopperSkin.Core.Audit;
using CopperSkin.Core.Theming;

namespace CopperSkin.Core.Tests;

/// <summary>
/// Verifies the built-in theme catalog, resolver, validator, and source audit behavior.
/// </summary>
public sealed class ThemeCatalogTests
{
    /// <summary>
    /// Verifies the Built In Catalog Contains All Am Chipper Themes behavior.
    /// </summary>
    [Fact]
    public void BuiltInCatalogContainsAllAmChipperThemes()
    {
        ThemePack pack = BuiltInThemeCatalog.Create();

        Assert.Equal(24, pack.Themes.Count);
        Assert.Contains(pack.Themes, theme => theme.Name == "FL Grape");
        Assert.Contains(pack.Themes, theme => theme.Name == "Copper Desk");
        Assert.Contains(pack.Themes, theme => theme.Name == "Deep Red");
    }

    /// <summary>
    /// Verifies the Resolver Provides Legacy Aliases behavior.
    /// </summary>
    [Fact]
    public void ResolverProvidesLegacyAliases()
    {
        ResolvedTheme theme = new ThemeResolver().Resolve(BuiltInThemeCatalog.Create(), "Copper Desk");

        Assert.Equal(theme.Get("color.surface.deep"), theme.Get("BgDeep"));
        Assert.Equal(theme.Get("color.accent.primary"), theme.Get("Accent"));
        Assert.Equal(theme.Get("color.text.primary"), theme.Get("TextPrimary"));
    }

    /// <summary>
    /// Verifies the Validator Accepts Built Ins behavior.
    /// </summary>
    [Fact]
    public void ValidatorAcceptsBuiltIns()
    {
        IReadOnlyList<ThemeDiagnostic> diagnostics = new ThemeValidator().Validate(BuiltInThemeCatalog.Create());

        Assert.DoesNotContain(diagnostics, diagnostic => diagnostic.Severity == "Error");
    }

    /// <summary>
    /// Verifies the Audit Finds Hard Coded Colors behavior.
    /// </summary>
    [Fact]
    public void AuditFindsHardCodedColors()
    {
        string root = Path.Combine(Path.GetTempPath(), "copperskin-audit-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(root);
        string file = Path.Combine(root, "View.xaml");
        File.WriteAllText(file, "<Grid Background=\"#FF000000\"/>");

        IReadOnlyList<AuditFinding> findings = HardCodedColorAudit.ScanDirectory(root);

        Assert.Single(findings);
        Assert.Equal("CSKIN001", findings[0].Code);
    }
}
