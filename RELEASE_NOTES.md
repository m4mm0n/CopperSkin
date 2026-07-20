# CopperSkin 0.3.1.0

Documentation patch release for the first public release.

- NuGet packages are version `0.3.1`.
- NuGet package pages now use a package-specific README with absolute documentation links.
- The repository README links directly to the current NuGet package pages.
- Assembly, file, and theme compatibility versions remain `0.3.0.0`.

See the [0.3.0.0 release notes](https://github.com/m4mm0n/CopperSkin/blob/main/RELEASE_NOTES.md) for the complete feature release details.

---

# CopperSkin 0.3.0.0

First official public release of the reusable CopperSkin engine, designer, CLI, sample app, and release packages.

## Highlights

- QuickLog exception ownership is enabled in the Designer and Sample Kitchen Sink apps.
- Unhandled WPF dispatcher exceptions are logged through QuickLog and shown with CopperSkin-owned task dialogs.
- AppDomain and unobserved task exceptions use QuickLog's hook manager with a CopperSkin custom popup.
- `CopperTaskDialog` now supports stock Win32 icons for error, warning, information, question, shield, and application states.
- Scoped themes can be applied to individual WPF subtrees with `CopperSkinThemeScope`.
- The runtime now includes a canonical token catalog for colors, spacing, radii, typography, motion, effects, icons, and adapter hints.
- The CLI now creates static theme galleries, deterministic visual baselines, starter theme scaffolds, token diffs, migration reports, and signed theme packs.
- The Designer includes a browsable token catalog beside live token editing and preview.
- Theme-pack signing now uses no-dependency B-233 binary-field ECDSA metadata with reusable key generation and trusted public-key verification.
- Standard WPF styling coverage now includes broader shell, item, document, popup, data-grid, toolbar, resize, and text surfaces.
- The Designer now includes a vector-first Graphics tab for icon and basic-paint authoring with layers, history, zoom, save/open, and export.
- Core graphics documents can be validated, inspected, serialized, embedded in theme packs, and rendered by WPF `GraphicCanvas` or `CopperIcon`.
- SVG, DrawingImage XAML, and PNG export adapters are deterministic and covered by WPF tests.
- Repository release metadata, MIT licensing, Windows CI, package validation, and tag-gated NuGet publishing are included.
- The WPF partial-control wave now covers focus and validation visuals, keyboard scope, RTL-preserving state, virtualization, disabled/read-only states, and theme switching for `DataGrid`, `RichTextBox`, `DatePicker`, `Calendar`, `TabControl`, and `ToolBarOverflowPanel`.
- The Designer preview now includes a full standard-control tab and an in-studio QuickLog-backed event log.
- The corrected horizontal scrollbar direction is covered by regression tests and documented in the accessibility contract.
- Package, assembly, theme pack, and release metadata are `0.3.0.0`.

## Included

- `CopperSkin.Core` typed token model, token catalog, theme resolver, validator, JSON serializer, B-233 ECDSA signature helper, theme archive, and hard-coded color audit
- `CopperSkin.Wpf` WPF runtime with dynamic resources, scoped themes, Windows backdrop selection, broad implicit standard-control styles, custom chrome support, `CopperWindow`, `CopperMessageBox`, and `CopperTaskDialog`
- `CopperSkin.Designer` theme creation tool with live preview, standard-control preview, token editing, catalog browsing, diagnostics, event log, export, engine logo, QuickLog logging, and themed exception dialogs
- `CopperSkin.Cli` commands for theme listing, token catalog output, export, validation, audit, migration reports, gallery, baseline, diff, scaffold, key generation, signing, signature verification, adapters, `.cskin`, and `.lzhc`
- `CopperSkin.SampleKitchenSink` WPF preview app
- built-in amChipper-inspired theme pack with 24 themes
- NuGet packages for the reusable engine libraries

## Compatibility

- Core: `netstandard2.0`, `net7.0`, `net8.0`, `net9.0`, `net10.0`
- WPF runtime: `net7.0-windows`, `net8.0-windows`, `net9.0-windows`, `net10.0-windows`
- Tools and sample apps: `net8.0`, `net9.0`, `net10.0`

## Validation

- `dotnet build .\CopperSkin.slnx -c Release`
- `dotnet test .\CopperSkin.slnx -c Release --no-build`
- `CopperSkin.Cli validate .\themes\amchipper\theme-pack.json`
- `CopperSkin.Cli audit .\src`
