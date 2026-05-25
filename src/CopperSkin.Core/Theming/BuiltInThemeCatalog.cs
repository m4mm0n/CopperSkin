// copperskin:allow-hardcoded-color-file
namespace CopperSkin.Core.Theming;

public static class BuiltInThemeCatalog
{
    public static ThemePack Create()
    {
        var pack = new ThemePack
        {
            Id = "copperskin.amchipper",
            Name = "CopperSkin amChipper Classics",
            Version = "0.1.0"
        };

        foreach (var palette in Palettes)
            pack.Themes.Add(CreateTheme(palette.Name, palette.Values));

        return pack;
    }

    public static IReadOnlyList<string> ThemeNames => Palettes.Select(static p => p.Name).ToArray();

    private static ThemeDefinition CreateTheme(string name, IReadOnlyDictionary<string, string> values)
    {
        var tokens = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["color.surface.deep"] = values["BgDeep"],
            ["color.surface.panel"] = values["BgPanel"],
            ["color.surface.control"] = values["BgControl"],
            ["color.surface.hover"] = values["BgHover"],
            ["color.surface.selected"] = values["BgSelect"],
            ["color.accent.primary"] = values["Accent"],
            ["color.accent.light"] = values["AccentLight"],
            ["color.border.default"] = values["Border"],
            ["color.text.primary"] = values["TextPrimary"],
            ["color.text.secondary"] = values["TextSecondary"],
            ["color.text.disabled"] = values["TextDisabled"],
            ["color.status.success"] = values.TryGetValue("Success", out string? success) ? success : "#FF59D98E",
            ["color.status.warning"] = values.TryGetValue("Warning", out string? warning) ? warning : "#FFFFCC44",
            ["color.status.danger"] = values.TryGetValue("Danger", out string? danger) ? danger : "#FFFF4466",
            ["color.editor.playhead"] = "#FFFF4444",
            ["color.editor.grid.line"] = values.TryGetValue("GridLine", out string? line) ? line : "#FF252535",
            ["color.editor.grid.bar"] = values.TryGetValue("GridBar", out string? bar) ? bar : "#FF303050",
            ["color.editor.tracker.note"] = "#FFA0D0FF",
            ["color.editor.tracker.instrument"] = "#FFFFCC44",
            ["color.editor.tracker.volume"] = "#FF44FF88",
            ["color.editor.tracker.effect"] = "#FFFF8844",
            ["color.editor.piano.white"] = "#FFF4F5FF",
            ["color.editor.piano.black"] = "#FF05050B",
            ["color.editor.clip"] = values["Accent"],
            ["color.editor.automation"] = values["AccentLight"]
        };

        foreach (KeyValuePair<string, string> pair in values)
            tokens[pair.Key] = pair.Value;

        return new ThemeDefinition
        {
            Id = CopperSkin.Core.Theming.ThemeNames.Slug(name),
            Name = name,
            Tokens = tokens,
            Metadata = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["origin"] = "amChipper",
                ["style"] = "glossy tracker workstation"
            }
        };
    }

    private sealed record Palette(string Name, IReadOnlyDictionary<string, string> Values);

    private static readonly Palette[] Palettes =
    [
        P("FL Grape", "#FF171221", "#FF241B32", "#FF342640", "#FF49345B", "#FF5F3F78", "#FFB961FF", "#FFFFA1F7", "#FF735A8C", "#FFF9F2FF", "#FFD5BEDF", "#FF806B8F"),
        P("Neon Studio", "#FF080A18", "#FF11142A", "#FF1A1F3B", "#FF252E58", "#FF2B3A74", "#FF2F8CFF", "#FF66E6FF", "#FF35406B", "#FFF1F6FF", "#FFC8D8FF", "#FF5F688E"),
        P("Classic Tracker", "#FF02110A", "#FF062016", "#FF0B2B1D", "#FF124A30", "#FF17663F", "#FF00E676", "#FF80FFB6", "#FF1E7048", "#FFF2FFF7", "#FFC8FAD8", "#FF5C8B70"),
        P("Amber CRT", "#FF130800", "#FF241107", "#FF351B0A", "#FF532C11", "#FF724015", "#FFFFB000", "#FFFFE082", "#FF8A551C", "#FFFFF5DD", "#FFFFD599", "#FF8E7051"),
        P("Midnight Pro", "#FF05070C", "#FF0D111C", "#FF151B2B", "#FF202A44", "#FF263864", "#FF4F8BFF", "#FFA7C4FF", "#FF34425F", "#FFF4F7FF", "#FFB8C3DC", "#FF586174"),
        P("Ice Matrix", "#FF061111", "#FF0D1F21", "#FF123034", "#FF1B454B", "#FF23606A", "#FF39D9C8", "#FF9BFFF4", "#FF2D6A73", "#FFEFFFFD", "#FFB5E7E1", "#FF537978"),
        P("Magenta Circuit", "#FF100713", "#FF1B0D20", "#FF2A1430", "#FF421F4B", "#FF5B2A6A", "#FFFF4FD8", "#FFFFA3ED", "#FF6C3B7A", "#FFFFF2FD", "#FFEEC5E8", "#FF84617D"),
        P("Carbon Lime", "#FF080B04", "#FF141B0B", "#FF24300F", "#FF3A5017", "#FF56751E", "#FFB7FF00", "#FFD8FF7A", "#FF4B6F3F", "#FFF6FFF0", "#FFD1E9C6", "#FF6F8668"),
        P("Ruby Wave", "#FF12070B", "#FF220D16", "#FF351522", "#FF512238", "#FF6B2D4E", "#FFFF3D7F", "#FFFF9DBF", "#FF7A3756", "#FFFFF3F7", "#FFFFC5D6", "#FF8A6270"),
        P("Ocean Lab", "#FF041218", "#FF09222C", "#FF103545", "#FF19516A", "#FF216B8B", "#FF23B7E5", "#FF8BE9FF", "#FF2B6D84", "#FFF1FCFF", "#FFBFEAF5", "#FF5D7C86"),
        P("Steel Mono", "#FF0D0F12", "#FF181B20", "#FF242932", "#FF343B47", "#FF454F60", "#FF9DAFC7", "#FFE2ECF8", "#FF596371", "#FFF7FAFF", "#FFC7D0DD", "#FF737B86"),
        P("Sunset Pop", "#FF180B16", "#FF2A1425", "#FF3D2037", "#FF5A2E51", "#FF794069", "#FFFF8A3D", "#FFFFD56F", "#FF8A516D", "#FFFFF7EF", "#FFFFD9C0", "#FF9A756D"),
        P("Cyber Cyan", "#FF020B12", "#FF071826", "#FF0C2638", "#FF123D58", "#FF185373", "#FF00B8FF", "#FF7AF0FF", "#FF235D7A", "#FFF1FDFF", "#FFBDE9F8", "#FF5D7E8B"),
        P("Toxic Acid", "#FF080B04", "#FF141B0B", "#FF24300F", "#FF3A5017", "#FF56751E", "#FFB7FF00", "#FFE8FF78", "#FF6B8B2B", "#FFFAFFF0", "#FFE2F4B8", "#FF78845E"),
        P("Royal Neon", "#FF090613", "#FF151024", "#FF241939", "#FF38275D", "#FF4B3481", "#FF7C4DFF", "#FFFF75E8", "#FF62478F", "#FFFFF3FF", "#FFE3C9FF", "#FF7F6A91"),
        P("Graphite Gold", "#FF090909", "#FF171717", "#FF252525", "#FF383329", "#FF514526", "#FFD8A72A", "#FFFFE083", "#FF6F6040", "#FFFFFBF0", "#FFE0D6C0", "#FF827A6C"),
        P("Circuit Blue", "#FF030915", "#FF08152A", "#FF102341", "#FF173763", "#FF214C88", "#FF24B6FF", "#FFA1F2FF", "#FF2A5C91", "#FFF3FAFF", "#FFC5DDF5", "#FF617590"),
        P("Laser Orange", "#FF140705", "#FF25110C", "#FF3B1C12", "#FF5B2B17", "#FF7A3D1F", "#FFFF7028", "#FFFFC75B", "#FF8B5430", "#FFFFF6ED", "#FFFFD3B5", "#FF9A7364"),
        P("Vapor Glass", "#FF0A0718", "#FF151029", "#FF241B3C", "#FF372B5B", "#FF4B3B7A", "#FF62F2FF", "#FFFF85F3", "#FF67528F", "#FFFFF8FF", "#FFD8D0FF", "#FF81749A"),
        P("Terminal Green", "#FF010806", "#FF06140F", "#FF0C2419", "#FF143B28", "#FF1B5737", "#FF20F27E", "#FFA0FFC5", "#FF28734C", "#FFF0FFF5", "#FFB8F1CE", "#FF557A65"),
        P("Hotline Pink", "#FF160513", "#FF2A0C24", "#FF421439", "#FF662054", "#FF8A2D70", "#FFFF4DB8", "#FFFFB36E", "#FF994A79", "#FFFFF3FB", "#FFFFC9E9", "#FF9B667F"),
        P("Arctic Steel", "#FF071014", "#FF111D24", "#FF1B2A33", "#FF2A3F4C", "#FF385869", "#FF73C7FF", "#FFE1F6FF", "#FF4B6978", "#FFF7FCFF", "#FFC8DCE6", "#FF72838B"),
        P("Copper Desk", "#FF100907", "#FF211410", "#FF33211B", "#FF4C342A", "#FF6A4937", "#FFE78A3A", "#FFFFD094", "#FF805D45", "#FFFFF8F0", "#FFEBCDB6", "#FF8A7668"),
        P("Deep Red", "#FF0F0205", "#FF1D080D", "#FF2F1018", "#FF4A1A27", "#FF692334", "#FFFF315D", "#FFFF9BAB", "#FF80384A", "#FFFFF3F5", "#FFFFC7CF", "#FF8C5E68")
    ];

    private static Palette P(string name, string deep, string panel, string control, string hover, string selected, string accent, string accentLight, string border, string textPrimary, string textSecondary, string textDisabled)
    {
        return new Palette(name, new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["BgDeep"] = deep,
            ["BgPanel"] = panel,
            ["BgControl"] = control,
            ["BgHover"] = hover,
            ["BgSelect"] = selected,
            ["Accent"] = accent,
            ["AccentLight"] = accentLight,
            ["Border"] = border,
            ["TextPrimary"] = textPrimary,
            ["TextSecondary"] = textSecondary,
            ["TextDisabled"] = textDisabled
        });
    }
}
