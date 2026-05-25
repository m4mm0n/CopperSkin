/*
 * ====================================================================================================
 *  Project        : CopperSkin
 *  File           : src\CopperSkin.Wpf\Drawing\DrawingThemeRegistry.cs
 *  Author         : Geir Gustavsen, ZeroLinez Softworx 2024 - 2026
 *  Created        : 2026-05-25 09:34:20 +02:00
 *  Last Modified  : 2026-05-25 11:25:22 +02:00
 *  CRC32          : 66DF8323
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
// CRC32-BODY: 66DF8323
namespace CopperSkin.Wpf.Drawing;

/// <summary>
/// Keeps weak references to drawing surfaces and broadcasts live theme snapshots to them.
/// </summary>
public static class DrawingThemeRegistry
{
    /// <summary>
    /// Tracks registered drawing surfaces without keeping them alive after their WPF owners are gone.
    /// </summary>
    private static readonly List<WeakReference<IThemeAwareDrawingSurface>> Surfaces = new();
    private static DrawingThemeSnapshot _current = DrawingThemeSnapshot.Default;

    /// <summary>
    /// Gets the most recent drawing-theme snapshot applied to registered surfaces.
    /// </summary>
    public static DrawingThemeSnapshot Current => _current;

    /// <summary>
    /// Registers a custom drawing surface for theme updates.
    /// </summary>
    public static void Register(IThemeAwareDrawingSurface surface)
    {
        lock (Surfaces)
        {
            Surfaces.RemoveAll(static reference => !reference.TryGetTarget(out _));
            if (!Surfaces.Any(reference => reference.TryGetTarget(out var existing) && ReferenceEquals(existing, surface)))
                Surfaces.Add(new WeakReference<IThemeAwareDrawingSurface>(surface));
        }

        surface.ApplyTheme(_current);
    }

    /// <summary>
    /// Applies the requested CopperSkin theme, resource set, chrome color, or drawing snapshot.
    /// </summary>
    public static void Apply(DrawingThemeSnapshot snapshot)
    {
        _current = snapshot;
        lock (Surfaces)
        {
            Surfaces.RemoveAll(static reference => !reference.TryGetTarget(out _));
            foreach (var reference in Surfaces)
            {
                if (reference.TryGetTarget(out var surface))
                    surface.ApplyTheme(snapshot);
            }
        }
    }
}
