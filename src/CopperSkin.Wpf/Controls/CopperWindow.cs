using System.Windows;

namespace CopperSkin.Wpf.Controls;

public class CopperWindow : Window
{
    public CopperWindow()
    {
        Loaded += (_, _) => CopperSkinThemeManager.Current?.AttachWindow(this);
    }
}
