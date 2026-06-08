/*
 * ====================================================================================================
 *  Project        : CopperSkin
 *  File           : src\CopperSkin.Wpf\CopperSkinThemeScope.cs
 *  Author         : Geir Gustavsen, ZeroLinez Softworx 2024 - 2026
 *  Created        : 2026-06-08 00:00:00 +02:00
 *  Last Modified  : 2026-06-08 19:36:14 +02:00
 *  CRC32          : 07E81C94
 *
 *  Description    :
 *                   Provides attached-property scoped theming for WPF subtrees.
 *
 *  License        :
 *                   MIT
 *                   https://opensource.org/licenses/MIT
 *
 *  Notes          :
 *                   WPF theme engine extracted from the amChipper custom skin.
 * ====================================================================================================
 */
// CRC32-BODY: 07E81C94
using System.Windows;

namespace CopperSkin.Wpf;

/// <summary>
/// Provides XAML-friendly scoped theming for individual WPF subtrees.
/// </summary>
public static class CopperSkinThemeScope
{
    /// <summary>
    /// Identifies the attached theme name/id property.
    /// </summary>
    public static readonly DependencyProperty ThemeProperty = DependencyProperty.RegisterAttached(
        "Theme",
        typeof(string),
        typeof(CopperSkinThemeScope),
        new PropertyMetadata(null, OnThemeChanged));

    private static readonly DependencyProperty PreviousResourcesProperty = DependencyProperty.RegisterAttached(
        "PreviousResources",
        typeof(CopperSkinThemeManager.ResourceSnapshot),
        typeof(CopperSkinThemeScope),
        new PropertyMetadata(null));

    /// <summary>
    /// Sets the scoped CopperSkin theme name or id on a WPF element.
    /// </summary>
    public static void SetTheme(DependencyObject element, string? value) => element.SetValue(ThemeProperty, value);

    /// <summary>
    /// Gets the scoped CopperSkin theme name or id from a WPF element.
    /// </summary>
    public static string? GetTheme(DependencyObject element) => (string?)element.GetValue(ThemeProperty);

    private static void OnThemeChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
    {
        if (dependencyObject is not FrameworkElement element)
            return;

        Apply(element, e.NewValue as string);
        element.Loaded -= ElementLoaded;
        if (element.IsLoaded)
            return;

        if (!string.IsNullOrWhiteSpace(e.NewValue as string))
            element.Loaded += ElementLoaded;
    }

    private static void ElementLoaded(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement element)
        {
            element.Loaded -= ElementLoaded;
            Apply(element, GetTheme(element));
        }
    }

    private static void Apply(FrameworkElement element, string? theme)
    {
        if (string.IsNullOrWhiteSpace(theme))
        {
            Restore(element);
            return;
        }

        if (element.GetValue(PreviousResourcesProperty) is not CopperSkinThemeManager.ResourceSnapshot)
            element.SetValue(PreviousResourcesProperty, CopperSkinThemeManager.ResourceSnapshot.Capture(element.Resources));

        CopperSkinThemeManager.Current?.ApplyTo(element, theme);
    }

    private static void Restore(FrameworkElement element)
    {
        if (element.GetValue(PreviousResourcesProperty) is not CopperSkinThemeManager.ResourceSnapshot snapshot)
            return;

        snapshot.Restore(element.Resources);
        element.ClearValue(PreviousResourcesProperty);
    }
}
