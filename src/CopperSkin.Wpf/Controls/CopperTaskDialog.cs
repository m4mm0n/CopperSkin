/*
 * ====================================================================================================
 *  Project        : CopperSkin
 *  File           : src\CopperSkin.Wpf\Controls\CopperTaskDialog.cs
 *  Author         : Geir Gustavsen, ZeroLinez Softworx 2024 - 2026
 *  Created        : 2026-05-25 09:34:20 +02:00
 *  Last Modified  : 2026-05-25 11:25:22 +02:00
 *  CRC32          : 173CFFE2
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
// CRC32-BODY: 173CFFE2
using System.Windows;

namespace CopperSkin.Wpf.Controls;

/// <summary>
/// Represents a CopperSkin-themed task dialog request with heading, body text, and configurable buttons.
/// </summary>
public sealed class CopperTaskDialog
{
    /// <summary>
    /// Gets or sets the dialog window title.
    /// </summary>
    public string Title { get; set; } = "CopperSkin Task";
    /// <summary>
    /// Gets or sets the prominent heading shown above the task dialog body.
    /// </summary>
    public string Heading { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the main explanatory task dialog text.
    /// </summary>
    public string Text { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the primary action button caption.
    /// </summary>
    public string PrimaryButtonText { get; set; } = "OK";
    /// <summary>
    /// Gets or sets the secondary action button caption.
    /// </summary>
    public string SecondaryButtonText { get; set; } = "Cancel";
    /// <summary>
    /// Gets or sets whether the dialog includes a secondary cancel-style action.
    /// </summary>
    public bool ShowSecondaryButton { get; set; }

    /// <summary>
    /// Shows the themed dialog or window interaction and returns the selected result.
    /// </summary>
    public bool Show(Window? owner = null)
    {
        var dialog = new CopperDialogWindow(
            Title,
            string.IsNullOrWhiteSpace(Heading) ? Text : $"{Heading}\n\n{Text}",
            ShowSecondaryButton ? MessageBoxButton.OKCancel : MessageBoxButton.OK,
            PrimaryButtonText,
            SecondaryButtonText);
        if (owner is not null)
            dialog.Owner = owner;
        return dialog.ShowDialog() == true && dialog.Result == MessageBoxResult.OK;
    }
}
