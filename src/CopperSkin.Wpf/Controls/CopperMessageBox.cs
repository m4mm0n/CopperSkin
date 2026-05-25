using System.Windows;
using CopperSkin.Core.Theming;

namespace CopperSkin.Wpf.Controls;

public static class CopperMessageBox
{
    public static MessageBoxResult Show(Window? owner, string message, string title = "CopperSkin", MessageBoxButton buttons = MessageBoxButton.OK)
    {
        if (Application.Current is null)
            return MessageBox.Show(message, title, buttons);

        var dialog = new CopperDialogWindow(title, message, buttons);
        if (owner is not null)
            dialog.Owner = owner;
        bool? result = dialog.ShowDialog();
        return result == true ? dialog.Result : MessageBoxResult.Cancel;
    }
}
