# Getting started

## Runtime application

Install the `CopperSkin.Core` and `CopperSkin.Wpf` `0.3.0` NuGet packages (assembly version `0.3.0.0`). Merge `CopperSkinThemeResources` into application resources, then call `CopperSkinApp.Use(Application.Current)` during startup. The [README](../README.md) contains the smallest complete startup and `CopperWindow` examples.

## Create a graphic

Run the Designer on Windows:

```powershell
dotnet run --project .\src\CopperSkin.Designer\CopperSkin.Designer.csproj -c Release -f net10.0-windows
```

Open the Graphics tab, choose an icon preset or Paint, select a layer, draw with a tool, and use Save/Open to persist a `.cgraphic` JSON document. Export SVG/XAML for vector consumers or PNG for raster consumers.

## Validate in CI

Use the CLI without WPF dependencies:

```powershell
dotnet run --project .\src\CopperSkin.Cli\CopperSkin.Cli.csproj -c Release -f net10.0 -- graphics validate .\assets\status.ok.cgraphic
```

The command returns exit code `0` for a valid document, `2` for validation diagnostics, and `3` for an unsupported command or renderer-dependent export request.
