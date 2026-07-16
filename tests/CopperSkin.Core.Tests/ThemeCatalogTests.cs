/*
 * ====================================================================================================
 *  Project        : CopperSkin
 *  File           : tests\CopperSkin.Core.Tests\ThemeCatalogTests.cs
 *  Author         : Geir Gustavsen, ZeroLinez Softworx 2024 - 2026
 *  Created        : 2026-05-25 09:40:59 +02:00
 *  Last Modified  : 2026-07-03 09:57:50 +02:00
 *  CRC32          : 13CBDA55
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
// CRC32-BODY: 13CBDA55

using CopperSkin.Core.Audit;
using CopperSkin.Core.Theming;

namespace CopperSkin.Core.Tests;

/// <summary>
/// Verifies the built-in theme catalog, resolver, validator, and source audit behavior.
/// </summary>
public sealed class ThemeCatalogTests
{

    /// <summary>
    /// Verifies the Audit Finds Hard Coded Colors behavior.
    /// </summary>
    [Fact]
    public void AuditFindsHardCodedColors()
    {
        var root = Path.Combine(Path.GetTempPath(), "copperskin-audit-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(root);
        var file = Path.Combine(root, "View.xaml");
        File.WriteAllText(file, "<Grid Background=\"#FF000000\"/>");

        var findings = HardCodedColorAudit.ScanDirectory(root);

        Assert.Single(findings);
        Assert.Equal("CSKIN001", findings[0].Code);
    }
    /// <summary>
    /// Verifies the Built In Catalog Contains All Am Chipper Themes behavior.
    /// </summary>
    [Fact]
    public void BuiltInCatalogContainsAllAmChipperThemes()
    {
        var pack = BuiltInThemeCatalog.Create();

        Assert.Equal(24, pack.Themes.Count);
        Assert.Contains(pack.Themes, theme => theme.Name == "FL Grape");
        Assert.Contains(pack.Themes, theme => theme.Name == "Copper Desk");
        Assert.Contains(pack.Themes, theme => theme.Name == "Deep Red");
    }

    /// <summary>
    /// Verifies the Resolver Adds Rich Derived Tokens behavior.
    /// </summary>
    [Fact]
    public void ResolverAddsRichDerivedTokens()
    {
        var theme = new ThemeResolver().Resolve(BuiltInThemeCatalog.Create(), "FL Grape");

        Assert.Equal("8", theme.Get("spacing.md"));
        Assert.Equal(theme.Get("color.accent.primary"), theme.Get("color.action.default"));
        Assert.Equal("Segoe UI", theme.Get("font.ui"));
    }

    /// <summary>
    /// Verifies the Resolver Provides Legacy Aliases behavior.
    /// </summary>
    [Fact]
    public void ResolverProvidesLegacyAliases()
    {
        var theme = new ThemeResolver().Resolve(BuiltInThemeCatalog.Create(), "Copper Desk");

        Assert.Equal(theme.Get("color.surface.deep"), theme.Get("BgDeep"));
        Assert.Equal(theme.Get("color.accent.primary"), theme.Get("Accent"));
        Assert.Equal(theme.Get("color.text.primary"), theme.Get("TextPrimary"));
    }

    /// <summary>
    /// Verifies the Theme Pack Signer Round Trips behavior.
    /// </summary>
    [Fact]
    public void ThemePackSignerRoundTrips()
    {
        var pack = BuiltInThemeCatalog.Create();
        var keyPair = ThemePackSigner.CreateKeyPair();

        var signature = ThemePackSigner.Sign(pack, "test", keyPair.PrivateKeyHex);

        Assert.False(string.IsNullOrWhiteSpace(signature));
        Assert.True(ThemePackSigner.Verify(pack, keyPair.PublicKeyHex));
        Assert.Equal("ECDSA-GF2M-B233-SHA256", pack.Metadata[ThemePackSigner.AlgorithmKey]);
        Assert.StartsWith("04", pack.Metadata[ThemePackSigner.PublicKeyKey], StringComparison.Ordinal);
        pack.Metadata["signature.signer"] = "other";
        Assert.False(ThemePackSigner.Verify(pack, keyPair.PublicKeyHex));
        var otherKeyPair = ThemePackSigner.CreateKeyPair();
        ThemePackSigner.Sign(pack, "test", keyPair.PrivateKeyHex);
        Assert.False(ThemePackSigner.Verify(pack, otherKeyPair.PublicKeyHex));
        pack.Themes[0].Tokens["color.accent.primary"] = "#FFFFFFFF";
        Assert.False(ThemePackSigner.Verify(pack, keyPair.PublicKeyHex));
    }

    /// <summary>
    /// Verifies key generation derives a real B-233 public point instead of an all-zero placeholder.
    /// </summary>
    [Fact]
    public void ThemePackSignerCreatesNonZeroPublicKey()
    {
        var keyPair = ThemePackSigner.CreateKeyPair();

        Assert.StartsWith("04", keyPair.PublicKeyHex, StringComparison.Ordinal);
        Assert.Contains(keyPair.PublicKeyHex[2..], character => character != '0');
    }

    /// <summary>
    /// Verifies the Token Catalog Includes Platform Tokens behavior.
    /// </summary>
    [Fact]
    public void TokenCatalogIncludesPlatformTokens()
    {
        Assert.Contains(ThemeTokenCatalog.All, token => token.Key == "spacing.md" && token.Type == ThemeTokenType.Number);
        Assert.Contains(ThemeTokenCatalog.All, token => token.Key == "font.ui" && token.Type == ThemeTokenType.FontFamily);
        Assert.Contains(ThemeTokenCatalog.All, token => token.Key == "motion.transition.ms" && token.Type == ThemeTokenType.Duration);
    }

    /// <summary>
    /// Verifies the Validator Accepts Built Ins behavior.
    /// </summary>
    [Fact]
    public void ValidatorAcceptsBuiltIns()
    {
        var diagnostics = new ThemeValidator().Validate(BuiltInThemeCatalog.Create());

        Assert.DoesNotContain(diagnostics, diagnostic => diagnostic.Severity == "Error");
    }

    /// <summary>
    /// Verifies the Validator Flags Bad Platform Tokens behavior.
    /// </summary>
    [Fact]
    public void ValidatorFlagsBadPlatformTokens()
    {
        var pack = BuiltInThemeCatalog.Create();
        var theme = pack.Themes[0].Clone();
        theme.Tokens["spacing.md"] = "-1";
        theme.Tokens["unknown.token"] = "wat";
        pack.Themes.Clear();
        pack.Themes.Add(theme);

        var diagnostics = new ThemeValidator().Validate(pack);

        Assert.Contains(diagnostics, diagnostic => diagnostic.Code == "TOKEN005");
        Assert.Contains(diagnostics, diagnostic => diagnostic.Code == "TOKEN003");
    }
}
