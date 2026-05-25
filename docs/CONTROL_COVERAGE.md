# CopperSkin Control Coverage

CopperSkin ships an implicit WPF style dictionary plus dedicated owner controls. The goal is that a normal WPF app can merge CopperSkin once and immediately get a coherent skin across the standard surface.

## Owner Controls

- `CopperWindow`: custom WPF window base with themed background, DWM title-bar tinting, and theme-change handling.
- `CopperMessageBox`: themed replacement flow for simple message boxes.
- `CopperTaskDialog`: themed task dialog flow for richer prompts.
- `TrackerPreviewControl`: drawing-surface sample for theme-aware custom renderers.

## Styled WPF Controls

The runtime resource dictionary includes implicit styles for:

- shell and text: `Window`, `Page`-like content via `ContentControl`, `HeaderedContentControl`, `TextBlock`, `Label`
- buttons: `Button`, `RepeatButton`, `ToggleButton`
- input: `TextBox`, `PasswordBox`, `RichTextBox`, `ComboBox`, `ComboBoxItem`, `DatePicker`, `DatePickerTextBox`
- choice: `CheckBox`, `RadioButton`, `Slider`
- lists and trees: `ListBox`, `ListBoxItem`, `ListView`, `GridViewColumnHeader`, `TreeView`, `TreeViewItem`
- data: `DataGrid`, `DataGridColumnHeader`, `DataGridCell`
- menus: `Menu`, `MenuItem`, `ContextMenu`, `Separator`
- layout and grouping: `TabControl`, `TabItem`, `GroupBox`, `Expander`, `GridSplitter`, `ScrollViewer`, `ScrollBar`, `Thumb`
- status and tools: `StatusBar`, `StatusBarItem`, `ToolBar`, `ToolBarTray`, `ToolBarOverflowPanel`, `ToolTip`
- date/document/canvas surfaces: `Calendar`, `CalendarItem`, `CalendarDayButton`, `CalendarButton`, `InkCanvas`, `DocumentViewer`, `FlowDocumentScrollViewer`, `FlowDocumentReader`, `FlowDocumentPageViewer`, `Frame`

## Extension Points

Applications can register custom drawing surfaces through `DrawingThemeRegistry`. Registered surfaces receive frozen brush snapshots every time the active theme changes.

Applications can also install their own resources after CopperSkin and override any token key, brush, gradient, or effect by using normal WPF resource precedence.

