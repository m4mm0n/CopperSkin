# CopperSkin Control Coverage

CopperSkin ships an implicit WPF style dictionary plus dedicated owner controls. The goal is that a normal WPF app can merge CopperSkin once and immediately get a coherent skin across the standard surface.

## Owner Controls

- `CopperWindow`: custom WPF window base with themed background, DWM title-bar tinting, and theme-change handling.
- `CopperMessageBox`: themed replacement flow for simple message boxes.
- `CopperTaskDialog`: themed task dialog flow for richer prompts.
- `TrackerPreviewControl`: drawing-surface sample for theme-aware custom renderers.
- `CopperSkinThemeScope`: attached-property theme scope for any WPF subtree.

## Styled WPF Controls

The runtime resource dictionary includes implicit styles for:

- shell and text: `Window`, `NavigationWindow`, `Page`, `UserControl`, `ContentControl`, `HeaderedContentControl`, `ItemsControl`, `HeaderedItemsControl`, `TextBlock`, `AccessText`, `TextElement`, `Label`
- buttons: `Button`, `RepeatButton`, `ToggleButton`
- input: `TextBoxBase`, `TextBox`, `PasswordBox`, `RichTextBox`, `ComboBox`, `ComboBoxItem`, `DatePicker`, `DatePickerTextBox`
- choice: `CheckBox`, `RadioButton`, `Slider`
- lists and trees: `ListBox`, `ListBoxItem`, `ListView`, `GridViewColumnHeader`, `TreeView`, `TreeViewItem`
- data: `DataGrid`, `DataGridRow`, `DataGridColumnHeader`, `DataGridColumnHeadersPresenter`, `DataGridRowHeader`, `DataGridCell`
- menus and popups: `Menu`, `MenuItem`, `ContextMenu`, `Popup`, `Separator`
- layout and grouping: `TabControl`, `TabItem`, `GroupBox`, `Expander`, `GridSplitter`, `ResizeGrip`, `ScrollViewer`, `ScrollBar`, `Thumb`
- status and tools: `StatusBar`, `StatusBarItem`, `ToolBar`, `ToolBarPanel`, `ToolBarTray`, `ToolBarOverflowPanel`, `ToolTip`
- date/document/canvas surfaces: `Calendar`, `CalendarItem`, `CalendarDayButton`, `CalendarButton`, `InkCanvas`, `DocumentViewer`, `FlowDocument`, `Paragraph`, `Section`, `Table`, `List`, `ListItem`, `Hyperlink`, `FlowDocumentScrollViewer`, `FlowDocumentReader`, `FlowDocumentPageViewer`, `Frame`

Panels such as `Grid`, `StackPanel`, `DockPanel`, `Canvas`, `WrapPanel`, `UniformGrid`, and `VirtualizingStackPanel` are layout primitives rather than lookful controls. CopperSkin skins their child controls, borders, scrollbars, splitters, and owner shells while leaving layout measurement behavior untouched.

## Extension Points

Applications can register custom drawing surfaces through `DrawingThemeRegistry`. Registered surfaces receive frozen brush snapshots every time the active theme changes.

Applications can also install their own resources after CopperSkin and override any token key, brush, gradient, or effect by using normal WPF resource precedence.
