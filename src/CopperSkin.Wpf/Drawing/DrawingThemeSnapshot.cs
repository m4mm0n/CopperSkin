/*
 * ====================================================================================================
 *  Project        : CopperSkin
 *  File           : src\CopperSkin.Wpf\Drawing\DrawingThemeSnapshot.cs
 *  Author         : Geir Gustavsen, ZeroLinez Softworx 2024 - 2026
 *  Created        : 2026-05-25 09:34:20 +02:00
 *  Last Modified  : 2026-05-25 11:25:22 +02:00
 *  CRC32          : 3A792B13
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
// CRC32-BODY: 3A792B13
// copperskin:allow-hardcoded-color-file
using System.Windows.Media;
using System.Collections.Generic;
using CopperSkin.Core.Theming;
using CopperSkin.Wpf.Resources;

namespace CopperSkin.Wpf.Drawing;

/// <summary>
/// Contains frozen WPF brushes and pens for custom CopperSkin drawing surfaces.
/// </summary>
public sealed class DrawingThemeSnapshot
{
    private IReadOnlyDictionary<string, Brush> TokenBrushes { get; init; } =
        new Dictionary<string, Brush>(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Gets the fallback drawing palette used before an application theme has been applied.
    /// </summary>
    public static DrawingThemeSnapshot Default => FromTheme(new ThemeResolver().Resolve(BuiltInThemeCatalog.Create(), "FL Grape"));

    /// <summary>
    /// Creates frozen WPF brushes and pens from a resolved CopperSkin theme.
    /// </summary>
    public static DrawingThemeSnapshot FromTheme(ResolvedTheme theme)
    {
        Brush B(string key, string fallback)
        {
            var brush = new SolidColorBrush(CopperSkinResourceEmitter.ToColor(theme.Get(key, fallback)));
            brush.Freeze();
            return brush;
        }

        Pen P(Brush brush, double thickness)
        {
            var pen = new Pen(brush, thickness);
            pen.Freeze();
            return pen;
        }

        var tokenBrushes = new Dictionary<string, Brush>(StringComparer.OrdinalIgnoreCase);
        foreach (var pair in theme.Tokens)
        {
            if (!pair.Key.StartsWith("color.", StringComparison.OrdinalIgnoreCase)
                || !pair.Value.StartsWith("#", StringComparison.Ordinal))
                continue;

            try
            {
                tokenBrushes[pair.Key] = B(pair.Key, pair.Value);
            }
            catch (FormatException)
            {
                // Invalid theme values are diagnosed by Core; drawing keeps its safe fallback.
            }
        }

        var grid = B("color.editor.grid.line", "#FF252535");
        var playhead = B("color.editor.playhead", "#FFFF4444");
        return new DrawingThemeSnapshot
        {
            Surface = B("color.surface.deep", "#FF171221"),
            Panel = B("color.surface.panel", "#FF241B32"),
            GridLine = grid,
            GridBar = B("color.editor.grid.bar", "#FF303050"),
            Playhead = playhead,
            TrackerNote = B("color.editor.tracker.note", "#FFA0D0FF"),
            TrackerInstrument = B("color.editor.tracker.instrument", "#FFFFCC44"),
            TrackerVolume = B("color.editor.tracker.volume", "#FF44FF88"),
            TrackerEffect = B("color.editor.tracker.effect", "#FFFF8844"),
            PianoWhite = B("color.editor.piano.white", "#FFF4F5FF"),
            PianoBlack = B("color.editor.piano.black", "#FF05050B"),
            GridLinePen = P(grid, 0.5),
            PlayheadPen = P(playhead, 1.5),
            TokenBrushes = tokenBrushes
        };
    }

    /// <summary>
    /// Resolves a color token for a graphics document, or returns <see langword="null"/> when unavailable.
    /// </summary>
    public Brush? GetTokenBrush(string? token) =>
        !string.IsNullOrWhiteSpace(token) && TokenBrushes.TryGetValue(token, out var brush) ? brush : null;
    /// <summary>
    /// Gets the brush used for stronger tracker bar lines.
    /// </summary>
    public Brush GridBar { get; private init; } = Brushes.Gray;
    /// <summary>
    /// Gets the brush used for fine tracker grid lines.
    /// </summary>
    public Brush GridLine { get; private init; } = Brushes.DimGray;
    /// <summary>
    /// Gets the frozen pen used for thin tracker grid lines.
    /// </summary>
    public Pen GridLinePen { get; private init; } = new(Brushes.DimGray, 0.5);
    /// <summary>
    /// Gets the brush used for panel fills inside drawing surfaces.
    /// </summary>
    public Brush Panel { get; private init; } = Brushes.Black;
    /// <summary>
    /// Gets the brush used for black-key regions in piano-style drawing surfaces.
    /// </summary>
    public Brush PianoBlack { get; private init; } = Brushes.Black;
    /// <summary>
    /// Gets the brush used for white-key regions in piano-style drawing surfaces.
    /// </summary>
    public Brush PianoWhite { get; private init; } = Brushes.White;
    /// <summary>
    /// Gets the brush used for the active playhead marker.
    /// </summary>
    public Brush Playhead { get; private init; } = Brushes.Red;
    /// <summary>
    /// Gets the frozen pen used for the active playhead line.
    /// </summary>
    public Pen PlayheadPen { get; private init; } = new(Brushes.Red, 1.5);
    /// <summary>
    /// Gets the brush used for deep drawing-surface backgrounds.
    /// </summary>
    public Brush Surface { get; private init; } = Brushes.Black;
    /// <summary>
    /// Gets the brush used for tracker effect text.
    /// </summary>
    public Brush TrackerEffect { get; private init; } = Brushes.Orange;
    /// <summary>
    /// Gets the brush used for tracker instrument text.
    /// </summary>
    public Brush TrackerInstrument { get; private init; } = Brushes.Gold;
    /// <summary>
    /// Gets the brush used for tracker note text.
    /// </summary>
    public Brush TrackerNote { get; private init; } = Brushes.LightBlue;
    /// <summary>
    /// Gets the brush used for tracker volume text.
    /// </summary>
    public Brush TrackerVolume { get; private init; } = Brushes.LightGreen;
}
