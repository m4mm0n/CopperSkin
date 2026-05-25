using System.Globalization;
using System.Windows;
using System.Windows.Media;
using CopperSkin.Wpf.Drawing;

namespace CopperSkin.Wpf.Controls;

public sealed class TrackerPreviewControl : FrameworkElement, IThemeAwareDrawingSurface
{
    private DrawingThemeSnapshot _theme = DrawingThemeSnapshot.Default;

    public TrackerPreviewControl()
    {
        MinHeight = 180;
        Loaded += (_, _) => DrawingThemeRegistry.Register(this);
    }

    public void ApplyTheme(DrawingThemeSnapshot theme)
    {
        _theme = theme;
        InvalidateVisual();
    }

    protected override void OnRender(DrawingContext dc)
    {
        double w = ActualWidth;
        double h = ActualHeight;
        dc.DrawRectangle(_theme.Surface, null, new Rect(0, 0, w, h));

        double rowHeight = 18;
        int rows = Math.Max(1, (int)(h / rowHeight));
        for (int row = 0; row < rows; row++)
        {
            Brush background = row % 4 == 0 ? _theme.GridBar : _theme.Panel;
            dc.DrawRectangle(background, null, new Rect(0, row * rowHeight, w, rowHeight - 1));
            dc.DrawLine(_theme.GridLinePen, new Point(0, row * rowHeight), new Point(w, row * rowHeight));
            DrawText(dc, row.ToString("X2", CultureInfo.InvariantCulture), 8, row * rowHeight + 2, _theme.TrackerInstrument);
            DrawText(dc, row % 3 == 0 ? "C-5 01 v40 A04" : row % 3 == 1 ? "D#5 02 v32 B00" : "--- -- --- ...", 64, row * rowHeight + 2, row % 3 == 2 ? _theme.GridLine : _theme.TrackerNote);
        }

        dc.DrawLine(_theme.PlayheadPen, new Point(0, rowHeight * 4), new Point(w, rowHeight * 4));
    }

    private static void DrawText(DrawingContext dc, string text, double x, double y, Brush brush)
    {
        var formatted = new FormattedText(
            text,
            CultureInfo.CurrentCulture,
            FlowDirection.LeftToRight,
            new Typeface("Consolas"),
            12,
            brush,
            1.0);
        dc.DrawText(formatted, new Point(x, y));
    }
}
