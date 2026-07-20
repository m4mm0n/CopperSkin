# Accessibility and control behavior

CopperSkin keeps WPF's routed events, automation peers, keyboard navigation, and validation system as the behavior owners. Styles provide visual state resources and templates; they do not replace application semantics.

The v0.3 support target includes:

| Surface | Expected behavior |
| --- | --- |
| Text input and `RichTextBox` | keyboard focus, caret/selection, read-only state, validation visuals |
| `DatePicker` and `Calendar` | popup focus return, keyboard date navigation, disabled dates |
| `DataGrid` | row/cell selection, keyboard traversal, headers, validation and scrolling |
| `TabControl` | selected-tab keyboard navigation, focus scopes, disabled tabs |
| `ToolBarOverflowPanel` | overflow keyboard access and focus return |
| Scrollbars | left/right commands and thumb movement use the same forward horizontal direction |
| Graphics | `CopperIcon.AccessibleName` is the runtime name; editor actions are exposed as labeled controls |

Applications remain responsible for meaningful names, descriptions, contrast choices, and media alternatives. `MediaElement` and external hosted content are intentionally not treated as fully owned by the theme.
