# CopperSkin 0.1.0

Initial engine release.

## Included

- `CopperSkin.Core` typed token model, theme resolver, validator, JSON serializer, theme archive, and hard-coded color audit
- `CopperSkin.Wpf` WPF runtime with dynamic resources, implicit standard-control styles, custom chrome support, `CopperWindow`, `CopperMessageBox`, and `CopperTaskDialog`
- `CopperSkin.Designer` theme creation tool with live preview, token editing, diagnostics, export, engine logo, and QuickLog logging
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

