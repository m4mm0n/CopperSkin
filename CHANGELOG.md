# Changelog

## Unreleased

No unreleased changes.

## v0.3.0.0 - 2026-07-19

### Added

- Added the v0.3 Core graphics document model with versioned JSON serialization, validation, pack persistence, and signature participation.
- Added shared WPF GraphicCanvas, CopperIcon, renderer, and document-space hit testing.
- Added a Designer Graphics tab with icon/paint presets, rectangle/ellipse/line/freehand tools, save/open, zoom, and snapshot undo/redo.
- Added initial v0.3 safe default styling for BulletDecorator, Image, InkPresenter, and MediaElement.
- Added `CopperSkinThemeResources`, a XAML-loadable resource dictionary that lets host apps and Visual Studio designer surfaces render CopperSkin styling before runtime startup executes.
- Added no-dependency B-233 binary-field ECDSA theme-pack signing, key generation, and trusted-public-key verification.
- Added broader implicit WPF standard-control coverage for shell, item, text/document, popup, data-grid, toolbar, resize, and document primitives.
- Added a richer Designer preview tab that exercises forms, collections, data grids, menus, toolbars, document content, calendar, and ink surfaces.
- Added a Designer event log pane backed by QuickLog for theme selection, token apply, validation, export, duplicate, and dialog preview actions.
- Added the first public-release graphics document contract for vector-first icons and basic painting.
- Added the Designer Graphics tab with presets, shape/freehand tools, layers, history, save/open, zoom, and deterministic SVG/XAML/PNG export.
- Added runtime `GraphicCanvas` and `CopperIcon` surfaces with theme-token fill/stroke resolution.
- Added a Windows CI workflow, package metadata, MIT license, security policy, contribution guide, and release checklist.

### Changed

- Changed theme-pack signing metadata from hash-only `signature.sha256` to `signature.eccgf2.*` B-233 ECDSA metadata.
- Updated CLI signing commands to support `keygen`, private-key-backed signing, and optional trusted public-key verification.
- Corrected horizontal scrollbar track direction so thumb movement and left/right commands agree.
- Fixed Designer collection updates to use supported `ObservableCollection` operations.
- Fixed static initialization ordering for the token catalog and B-233 base point; release signing no longer produces an all-zero public key or loops on zero signature coordinates.

- Bumped assembly, package, built-in theme-pack, and compatibility metadata to `0.3.0.0`.
- Enabled warnings-as-errors for the release build.
- Documented control behavior, accessibility expectations, graphics file format, and clean-checkout release gates.

## v0.2.0.0 - 2026-06-04

### Added

- Added CopperSkin-themed QuickLog exception dialogs to the Designer and Sample Kitchen Sink apps.
- Added WPF dispatcher exception handling so UI-thread failures are logged and shown through CopperSkin instead of the default crash dialog.
- Added stock Win32 icon support to `CopperTaskDialog` for application, information, warning, error, question, and shield states.
- Added a canonical theme token catalog covering color, spacing, radius, typography, motion, effects, icons, and adapter preference tokens.
- Added scoped WPF subtree theming through `CopperSkinThemeScope` and a fluent `CopperSkinApp` installation helper.
- Added Windows backdrop selection for themed WPF windows.
- Added theme-pack signing and verification helpers.
- Added CLI commands for token catalog output, migration reports, static galleries, visual baselines, theme diffs, scaffolding, signing, signature verification, and adapter catalog output.
- Added Designer token-catalog browsing beside live token editing and preview.
- Added version metadata for assemblies and release packages.

### Changed

- Bumped package, assembly, theme pack, and documentation examples to `0.2.0.0`.
- Updated release notes, theme format documentation, and README examples for the current fluent install, scoped theme, task dialog, and CLI APIs.
- Expanded validation to resolve inherited themes before checking required tokens and to validate known token types.
- Expanded WPF resource emission to include typed spacing, radius, font, duration, and metric aliases.
- Routed AppDomain and unobserved task exceptions through QuickLog's hook manager with a CopperSkin custom popup.
- Tightened theme-pack signatures so signed metadata changes invalidate verification.
- Restored scoped theme and preview resources from snapshots when a scoped theme is removed or a preview scope is disposed.

### Removed

- Removed release-internal planning notes from public documentation.

### Verified

- Release build completes with 0 warnings and 0 errors.
- Test suite passes in Release configuration.
- Theme validation reports 24 themes, 0 errors, and 0 warnings.
- Source color audit reports 0 findings.

## v0.1.0 - 2026-05-25

### Added

- Initial CopperSkin engine release.
- Added typed theme tokens, theme validation, serialization, archive support, and hard-coded color auditing.
- Added WPF runtime resources, implicit standard-control styles, custom chrome, `CopperWindow`, `CopperMessageBox`, and `CopperTaskDialog`.
- Added Designer, CLI, Sample Kitchen Sink app, built-in amChipper-inspired themes, and reusable NuGet packages.
