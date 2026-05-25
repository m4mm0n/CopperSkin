/*
 * ====================================================================================================
 *  Project        : CopperSkin
 *  File           : src\CopperSkin.Wpf\Controls\TrackerPreviewControl.cs
 *  Author         : Geir Gustavsen, ZeroLinez Softworx 2024 - 2026
 *  Created        : 2026-05-25 09:36:36 +02:00
 *  Last Modified  : 2026-05-25 11:25:22 +02:00
 *  CRC32          : 02232EBF
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
// CRC32-BODY: 02232EBF
using System.Globalization;
using System.Windows;
using System.Windows.Media;
using CopperSkin.Wpf.Drawing;

namespace CopperSkin.Wpf.Controls;

/// <summary>
/// Renders a tracker-inspired preview surface using the current CopperSkin drawing theme.
/// </summary>
public sealed class TrackerPreviewControl : FrameworkElement, IThemeAwareDrawingSurface
{
    private DrawingThemeSnapshot _theme = DrawingThemeSnapshot.Default;

    /// <summary>
    /// Initializes the tracker preview surface and registers it for live drawing-theme updates.
    /// </summary>
    public TrackerPreviewControl()
    {
        MinHeight = 180;
        Loaded += (_, _) => DrawingThemeRegistry.Register(this);
    }

    /// <summary>
    /// Applies a new drawing snapshot and schedules the preview surface to repaint.
    /// </summary>
    public void ApplyTheme(DrawingThemeSnapshot theme)
    {
        _theme = theme;
        InvalidateVisual();
    }

    /// <summary>
    /// Draws the themed WPF preview surface.
    /// </summary>
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

    /// <summary>
    /// Draws one tracker-style text fragment using the fixed-width preview typeface.
    /// </summary>
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
