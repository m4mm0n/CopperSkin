/*
 * ====================================================================================================
 *  Project        : CopperSkin
 *  File           : src\CopperSkin.Wpf\Controls\CopperRadialProgress.cs
 *  Author         : Geir Gustavsen, ZeroLinez Softworx 2024 - 2026
 *  Created        : 2026-06-28 01:31:17 +02:00
 *  Last Modified  : 2026-07-03 09:59:32 +02:00
 *  CRC32          : F2B79739
 *
 *  Description    :
 *                   Renders a theme-aware circular progress indicator for long-running CopperSkin workflows.
 *
 *  License        :
 *                   MIT
 *                   https://opensource.org/licenses/MIT
 *
 *  Notes          :
 *                   WPF theme engine extracted from the amChipper custom skin.
 * ====================================================================================================
 */
// CRC32-BODY: F2B79739

using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace CopperSkin.Wpf.Controls;

/// <summary>
/// Renders a theme-aware circular progress indicator for long-running CopperSkin workflows.
/// </summary>
public sealed class CopperRadialProgress : Control
{
    /// <summary>Identifies the <see cref="Minimum" /> dependency property.</summary>
    public static readonly DependencyProperty MinimumProperty =
        DependencyProperty.Register(nameof(Minimum), typeof(double), typeof(CopperRadialProgress),
            new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsRender));

    /// <summary>Identifies the <see cref="Maximum" /> dependency property.</summary>
    public static readonly DependencyProperty MaximumProperty =
        DependencyProperty.Register(nameof(Maximum), typeof(double), typeof(CopperRadialProgress),
            new FrameworkPropertyMetadata(100d, FrameworkPropertyMetadataOptions.AffectsRender));

    /// <summary>Identifies the <see cref="Value" /> dependency property.</summary>
    public static readonly DependencyProperty ValueProperty =
        DependencyProperty.Register(nameof(Value), typeof(double), typeof(CopperRadialProgress),
            new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsRender));

    /// <summary>Identifies the <see cref="Caption" /> dependency property.</summary>
    public static readonly DependencyProperty CaptionProperty =
        DependencyProperty.Register(nameof(Caption), typeof(string), typeof(CopperRadialProgress),
            new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.AffectsRender));

    /// <summary>Gets or sets the lower bound used to calculate progress.</summary>
    public double Minimum
    {
        get => (double)GetValue(MinimumProperty);
        set => SetValue(MinimumProperty, value);
    }

    /// <summary>Gets or sets the upper bound used to calculate progress.</summary>
    public double Maximum
    {
        get => (double)GetValue(MaximumProperty);
        set => SetValue(MaximumProperty, value);
    }

    /// <summary>Gets or sets the current progress value.</summary>
    public double Value
    {
        get => (double)GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    /// <summary>Gets or sets the short label rendered below the percentage.</summary>
    public string Caption
    {
        get => (string)GetValue(CaptionProperty);
        set => SetValue(CaptionProperty, value);
    }

    static CopperRadialProgress()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(CopperRadialProgress), new FrameworkPropertyMetadata(typeof(CopperRadialProgress)));
    }

    /// <inheritdoc />
    protected override void OnRender(DrawingContext drawingContext)
    {
        base.OnRender(drawingContext);

        var size = Math.Min(ActualWidth, ActualHeight);
        if (size <= 0)
            return;

        var center = new Point(ActualWidth / 2d, ActualHeight / 2d);
        var thickness = Math.Max(8d, size * 0.075d);
        var radius = (size - thickness) / 2d;
        var trackBrush = GetBrush("color.surface.hover", SystemColors.ControlDarkBrush);
        var accentBrush = GetBrush("color.status.danger", SystemColors.HighlightBrush);
        var textBrush = GetBrush("color.text.primary", SystemColors.ControlTextBrush);
        var secondaryBrush = GetBrush("color.text.secondary", SystemColors.GrayTextBrush);

        drawingContext.DrawEllipse(null, new Pen(trackBrush, thickness), center, radius, radius);

        var range = Maximum - Minimum;
        var progress = range <= 0 ? 0 : Math.Clamp((Value - Minimum) / range, 0d, 1d);
        if (progress > 0)
            DrawArc(drawingContext, center, radius, thickness, progress, accentBrush);

        var percentText = FormattableString.Invariant($"{Math.Round(progress * 100d):0}%");
        DrawCenteredText(drawingContext, percentText, size * 0.22d, FontWeights.Bold, textBrush, center.Y - (size * 0.06d));

        if (!string.IsNullOrWhiteSpace(Caption))
            DrawCenteredText(drawingContext, Caption.ToUpperInvariant(), size * 0.052d, FontWeights.SemiBold, secondaryBrush, center.Y + (size * 0.14d));
    }

    private static void DrawArc(DrawingContext drawingContext, Point center, double radius, double thickness, double progress, Brush brush)
    {
        var startAngle = -90d;
        var endAngle = startAngle + (360d * progress);
        var start = PointOnCircle(center, radius, startAngle);
        var end = PointOnCircle(center, radius, endAngle);
        var geometry = new StreamGeometry();

        using (var context = geometry.Open())
        {
            context.BeginFigure(start, false, false);
            context.ArcTo(end, new Size(radius, radius), 0d, progress > 0.5d, SweepDirection.Clockwise, true, false);
        }

        geometry.Freeze();
        drawingContext.DrawGeometry(null, new Pen(brush, thickness) { StartLineCap = PenLineCap.Flat, EndLineCap = PenLineCap.Flat }, geometry);
    }

    private static Point PointOnCircle(Point center, double radius, double angleDegrees)
    {
        var angle = angleDegrees * Math.PI / 180d;
        return new Point(center.X + radius * Math.Cos(angle), center.Y + radius * Math.Sin(angle));
    }

    private void DrawCenteredText(DrawingContext drawingContext, string text, double size, FontWeight weight, Brush brush, double baselineCenterY)
    {
        var formatted = new FormattedText(
            text,
            CultureInfo.CurrentUICulture,
            FlowDirection.LeftToRight,
            new Typeface(FontFamily, FontStyles.Normal, weight, FontStretches.Normal),
            Math.Max(8d, size),
            brush,
            VisualTreeHelper.GetDpi(this).PixelsPerDip);

        drawingContext.DrawText(formatted, new Point((ActualWidth - formatted.Width) / 2d, baselineCenterY - formatted.Height / 2d));
    }

    private Brush GetBrush(string resourceKey, Brush fallback)
    {
        return TryFindResource(resourceKey) as Brush ?? fallback;
    }
}
