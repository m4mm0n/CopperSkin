# Changelog

## v0.2.0.0 - 2026-06-04

### Added

- Added CopperSkin-themed QuickLog exception dialogs to the Designer and Sample Kitchen Sink apps.
- Added WPF dispatcher exception handling so UI-thread failures are logged and shown through CopperSkin instead of the default crash dialog.
- Added stock Win32 icon support to `CopperTaskDialog` for application, information, warning, error, question, and shield states.
- Added version metadata for assemblies and release packages.

### Changed

- Bumped package, assembly, theme pack, and documentation examples to `0.2.0.0`.
- Updated release notes and README examples for the current task dialog API.
- Routed AppDomain and unobserved task exceptions through QuickLog's hook manager with a CopperSkin custom popup.

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
