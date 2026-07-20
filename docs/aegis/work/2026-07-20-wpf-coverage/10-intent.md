# WPF coverage completion slice

Requested outcome: close the documented v0.3 partial WPF-control wave before declaring the release finished.

## Goal and boundary

Improve behavior-complete theming for `DataGrid`, `RichTextBox`, `DatePicker`/`Calendar`, `TabControl`, `ToolBarOverflowPanel`, validation visuals, focus visuals, keyboard navigation, disabled/read-only states, RTL layout, high-DPI-safe sizing, and theme switching. Preserve WPF ownership of routed events, automation peers, validation semantics, hosted visual trees, `WebBrowser`, and OS-owned media.

## Baseline refs

- `src/CopperSkin.Wpf/Themes/CopperSkin.Controls.xaml`
- `tests/CopperSkin.Wpf.Tests/WpfThemeTests.cs`
- `docs/CONTROL_COVERAGE.md`
- `docs/ACCESSIBILITY.md`
- `docs/aegis/plans/2026-07-16-v0.3.0-public-release.md`
- `Directory.Build.props`

## Success evidence

- Resource dictionary loads and custom templates apply for the target controls.
- Tests exercise focus, keyboard-navigation properties, validation error templates, disabled/read-only states, RTL flow direction, and theme switching.
- Full Release build/test matrix passes with 0 warnings and 0 errors.
- Clean-checkout CI and package validation pass.

## Non-goals

Full WPF framework parity, custom ownership of `WebBrowser`, external visual trees, OS-hosted media, application-specific data validation rules, and rewriting the existing dictionary into unrelated architecture are outside this slice.
