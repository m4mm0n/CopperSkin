using CopperSkin.Core.Theming;

namespace CopperSkin.Wpf;

public sealed class CopperSkinThemeChangedEventArgs : EventArgs
{
    public CopperSkinThemeChangedEventArgs(ResolvedTheme theme)
    {
        Theme = theme;
    }

    public ResolvedTheme Theme { get; }
}
