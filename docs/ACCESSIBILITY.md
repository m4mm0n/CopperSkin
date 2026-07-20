# Accessibility and control behavior

CopperSkin keeps WPF's routed events, automation peers, keyboard navigation, and validation system as the behavior owners. Styles provide visual state resources and templates; they do not replace application semantics. The v0.3 target-control wave now has regression coverage for the visual and state contract around those framework behaviors.

The v0.3 support target includes:

| Surface | Expected behavior |
| --- | --- |
| Text input and `RichTextBox` | keyboard focus, caret/selection, read-only state, validation visuals |
| `DatePicker` and `Calendar` | keyboard date navigation, disabled state, focus and validation visuals; popup ownership remains WPF |
| `DataGrid` | row/cell selection, keyboard traversal, headers, validation wiring, virtualization, and scrolling |
| `TabControl` | selected-tab keyboard navigation, focus scopes, disabled tabs |
| `ToolBarOverflowPanel` | overflow keyboard traversal and focus cycling |
| Scrollbars | left/right commands and thumb movement use the same forward horizontal direction |
| Graphics | `CopperIcon.AccessibleName` is the runtime name; editor actions are exposed as labeled controls |

Applications remain responsible for meaningful names, descriptions, contrast choices, and media alternatives. `MediaElement` and external hosted content are intentionally not treated as fully owned by the theme.
