using CopperSkin.Core.Audit;
using CopperSkin.Core.Theming;

namespace CopperSkin.Core.Tests;

public sealed class ThemeCatalogTests
{
    [Fact]
    public void BuiltInCatalogContainsAllAmChipperThemes()
    {
        ThemePack pack = BuiltInThemeCatalog.Create();

        Assert.Equal(24, pack.Themes.Count);
        Assert.Contains(pack.Themes, theme => theme.Name == "FL Grape");
        Assert.Contains(pack.Themes, theme => theme.Name == "Copper Desk");
        Assert.Contains(pack.Themes, theme => theme.Name == "Deep Red");
    }

    [Fact]
    public void ResolverProvidesLegacyAliases()
    {
        ResolvedTheme theme = new ThemeResolver().Resolve(BuiltInThemeCatalog.Create(), "Copper Desk");

        Assert.Equal(theme.Get("color.surface.deep"), theme.Get("BgDeep"));
        Assert.Equal(theme.Get("color.accent.primary"), theme.Get("Accent"));
        Assert.Equal(theme.Get("color.text.primary"), theme.Get("TextPrimary"));
    }

    [Fact]
    public void ValidatorAcceptsBuiltIns()
    {
        IReadOnlyList<ThemeDiagnostic> diagnostics = new ThemeValidator().Validate(BuiltInThemeCatalog.Create());

        Assert.DoesNotContain(diagnostics, diagnostic => diagnostic.Severity == "Error");
    }

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
