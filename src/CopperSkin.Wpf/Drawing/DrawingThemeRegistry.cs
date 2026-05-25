namespace CopperSkin.Wpf.Drawing;

public static class DrawingThemeRegistry
{
    private static readonly List<WeakReference<IThemeAwareDrawingSurface>> Surfaces = new();
    private static DrawingThemeSnapshot _current = DrawingThemeSnapshot.Default;

    public static DrawingThemeSnapshot Current => _current;

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
