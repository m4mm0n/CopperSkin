/*
 * ====================================================================================================
 *  Project        : CopperSkin
 *  File           : src\CopperSkin.Wpf\Controls\CopperMessageBox.cs
 *  Author         : Geir Gustavsen, ZeroLinez Softworx 2024 - 2026
 *  Created        : 2026-05-25 09:34:20 +02:00
 *  Last Modified  : 2026-05-25 11:25:22 +02:00
 *  CRC32          : 30B5ABAD
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
// CRC32-BODY: 30B5ABAD
using System.Windows;
using CopperSkin.Core.Theming;

namespace CopperSkin.Wpf.Controls;

/// <summary>
/// Provides a CopperSkin-themed MessageBox replacement.
/// </summary>
public static class CopperMessageBox
{
    /// <summary>
    /// Shows the themed dialog or window interaction and returns the selected result.
    /// </summary>
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
