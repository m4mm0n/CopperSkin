# CopperSkin 0.2.0.0

Release candidate for the reusable CopperSkin engine, designer, CLI, sample app, and release bundle.

## Highlights

- QuickLog exception ownership is enabled in the Designer and Sample Kitchen Sink apps.
- Unhandled WPF dispatcher exceptions are logged through QuickLog and shown with CopperSkin-owned task dialogs.
- AppDomain and unobserved task exceptions use QuickLog's hook manager with a CopperSkin custom popup.
- `CopperTaskDialog` now supports stock Win32 icons for error, warning, information, question, shield, and application states.
- Release-internal planning notes were removed from public docs.
- Package, assembly, theme pack, and release metadata were bumped to `0.2.0.0`.

## Included

- `CopperSkin.Core` typed token model, theme resolver, validator, JSON serializer, theme archive, and hard-coded color audit
- `CopperSkin.Wpf` WPF runtime with dynamic resources, implicit standard-control styles, custom chrome support, `CopperWindow`, `CopperMessageBox`, and `CopperTaskDialog`
- `CopperSkin.Designer` theme creation tool with live preview, token editing, diagnostics, export, engine logo, QuickLog logging, and themed exception dialogs
- `CopperSkin.Cli` commands for theme listing, export, validation, audit, `.cskin`, and `.lzhc`
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
