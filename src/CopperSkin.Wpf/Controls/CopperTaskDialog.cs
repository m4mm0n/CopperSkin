using System.Windows;

namespace CopperSkin.Wpf.Controls;

public sealed class CopperTaskDialog
{
    public string Title { get; set; } = "CopperSkin Task";
    public string Heading { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public string PrimaryButtonText { get; set; } = "OK";
    public string SecondaryButtonText { get; set; } = "Cancel";
    public bool ShowSecondaryButton { get; set; }

    public bool Show(Window? owner = null)
    {
        var dialog = new CopperDialogWindow(Title, string.IsNullOrWhiteSpace(Heading) ? Text : $"{Heading}\n\n{Text}", ShowSecondaryButton ? MessageBoxButton.OKCancel : MessageBoxButton.OK)
        {
            PrimaryButtonText = PrimaryButtonText,
            SecondaryButtonText = SecondaryButtonText
        };
        if (owner is not null)
            dialog.Owner = owner;
        return dialog.ShowDialog() == true && dialog.Result == MessageBoxResult.OK;
    }
}
