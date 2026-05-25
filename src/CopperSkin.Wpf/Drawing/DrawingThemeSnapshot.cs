// copperskin:allow-hardcoded-color-file
using System.Windows.Media;
using CopperSkin.Core.Theming;
using CopperSkin.Wpf.Resources;

namespace CopperSkin.Wpf.Drawing;

public sealed class DrawingThemeSnapshot
{
    public Brush Surface { get; private init; } = Brushes.Black;
    public Brush Panel { get; private init; } = Brushes.Black;
    public Brush GridLine { get; private init; } = Brushes.DimGray;
    public Brush GridBar { get; private init; } = Brushes.Gray;
    public Brush Playhead { get; private init; } = Brushes.Red;
    public Brush TrackerNote { get; private init; } = Brushes.LightBlue;
    public Brush TrackerInstrument { get; private init; } = Brushes.Gold;
    public Brush TrackerVolume { get; private init; } = Brushes.LightGreen;
    public Brush TrackerEffect { get; private init; } = Brushes.Orange;
    public Brush PianoWhite { get; private init; } = Brushes.White;
    public Brush PianoBlack { get; private init; } = Brushes.Black;
    public Pen GridLinePen { get; private init; } = new(Brushes.DimGray, 0.5);
    public Pen PlayheadPen { get; private init; } = new(Brushes.Red, 1.5);

    public static DrawingThemeSnapshot Default => FromTheme(new ThemeResolver().Resolve(BuiltInThemeCatalog.Create(), "FL Grape"));

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

        Brush grid = B("color.editor.grid.line", "#FF252535");
        Brush playhead = B("color.editor.playhead", "#FFFF4444");
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
            PlayheadPen = P(playhead, 1.5)
        };
    }
}
