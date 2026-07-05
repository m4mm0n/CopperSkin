# CopperSkin

CopperSkin is a WPF theme platform extracted from the amChipper custom skin and rebuilt as a reusable runtime, designer, CLI, sample app, and release package.

The engine is built around typed theme tokens, dynamic WPF resources, custom window chrome, scoped themes, standard-control styling, themed drawing surfaces, `.cskin` theme packs, signed metadata, static galleries, visual baselines, migration reports, and a custom `.lzhc` release archive command. `mainlogo.png` is the engine logo and is embedded into the designer and sample shell.

## Projects

| Project | Purpose | Target frameworks |
| --- | --- | --- |
| `CopperSkin.Core` | Theme model, validation, serialization, pack archives, hard-coded color audit | `netstandard2.0`, `net7.0`, `net8.0`, `net9.0`, `net10.0` |
| `CopperSkin.Wpf` | Runtime WPF engine, resources, implicit styles, `CopperWindow`, `CopperMessageBox`, `CopperTaskDialog` | `net7.0-windows`, `net8.0-windows`, `net9.0-windows`, `net10.0-windows` |
| `CopperSkin.Cli` | Theme export, validation, token catalog, gallery, baseline, diff, scaffold, signing, migration reports, `.cskin`, `.lzhc` | `net8.0`, `net9.0`, `net10.0` |
| `CopperSkin.Designer` | Theme creation tool with live preview, token editing, diagnostics, and catalog browsing | `net8.0-windows`, `net9.0-windows`, `net10.0-windows` |
| `CopperSkin.SampleKitchenSink` | WPF kitchen-sink preview app | `net8.0-windows`, `net9.0-windows`, `net10.0-windows` |

The core engine stays compatible as far back as `netstandard2.0`. The WPF runtime starts at `net7.0-windows`. Tooling uses `ZLS.QuickLog` and therefore starts at `net8.0`.

## Quick Start

Install the WPF theme runtime in your app startup:

```csharp
using CopperSkin.Core.Theming;
using CopperSkin.Wpf;

CopperSkinApp.Use(Application.Current)
    .Pack(BuiltInThemeCatalog.Create())
    .Theme("Neon Studio")
    .Backdrop(CopperSkinBackdropKind.Mica)
    .Install();
```

For instant Visual Studio designer support, merge the XAML-loadable resources in `App.xaml` too:

```xml
<Application.Resources>
  <ResourceDictionary>
    <ResourceDictionary.MergedDictionaries>
      <copper:CopperSkinThemeResources Theme="FL Grape"/>
    </ResourceDictionary.MergedDictionaries>
  </ResourceDictionary>
</Application.Resources>
```

Use `xmlns:copper="clr-namespace:CopperSkin.Wpf;assembly=CopperSkin.Wpf"`. The startup install call can still switch or replace the active runtime theme.

Use the custom shell controls where you want full window/dialog ownership:

```xml
<controls:CopperWindow
    x:Class="MyApp.MainWindow"
    xmlns:controls="clr-namespace:CopperSkin.Wpf.Controls;assembly=CopperSkin.Wpf"
    Title="My Copper App"
    Width="1100"
    Height="720">
    <Grid />
</controls:CopperWindow>
```

Scope a different theme to any WPF subtree:

```xml
<Border
    xmlns:cs="clr-namespace:CopperSkin.Wpf;assembly=CopperSkin.Wpf"
    cs:CopperSkinThemeScope.Theme="Copper Desk">
    <StackPanel />
</Border>
```

Message and task dialogs are theme-aware:

```csharp
CopperMessageBox.Show(this, "Saved", "Theme pack exported.", "Nice");
new CopperTaskDialog
{
    Title = "Validation",
    Heading = "All Good",
    Text = "No blocking theme errors.",
    Icon = CopperTaskDialogIcon.Information
}.Show(this);
```

## Theme Designer

`CopperSkin.Designer` is the theme creation tool. It includes:

- built-in amChipper theme selection
- live token grid editing
- real standard-control preview tab covering forms, collections, data, documents, menus, toolbars, calendar, and ink surfaces
- browsable canonical token catalog
- instant WPF preview using the real runtime resource emitter
- diagnostics for missing tokens, malformed colors, and low contrast
- `.json` export for theme packs
- themed `CopperTaskDialog` preview
- QuickLog exception hijacking with CopperSkin-themed task dialogs
- QuickLog-backed event logging with an in-studio log pane

Run it from source:

```powershell
dotnet run --project .\src\CopperSkin.Designer\CopperSkin.Designer.csproj -c Release -f net10.0-windows
```

## CLI

```powershell
dotnet run --project .\src\CopperSkin.Cli\CopperSkin.Cli.csproj -c Release -f net10.0 -- list
dotnet run --project .\src\CopperSkin.Cli\CopperSkin.Cli.csproj -c Release -f net10.0 -- tokens
dotnet run --project .\src\CopperSkin.Cli\CopperSkin.Cli.csproj -c Release -f net10.0 -- export-builtins .\themes\amchipper\theme-pack.json
dotnet run --project .\src\CopperSkin.Cli\CopperSkin.Cli.csproj -c Release -f net10.0 -- validate .\themes\amchipper\theme-pack.json
dotnet run --project .\src\CopperSkin.Cli\CopperSkin.Cli.csproj -c Release -f net10.0 -- audit .\src
dotnet run --project .\src\CopperSkin.Cli\CopperSkin.Cli.csproj -c Release -f net10.0 -- migrate .\src .\artifacts\migration.md
dotnet run --project .\src\CopperSkin.Cli\CopperSkin.Cli.csproj -c Release -f net10.0 -- gallery .\themes\amchipper\theme-pack.json .\artifacts\gallery
dotnet run --project .\src\CopperSkin.Cli\CopperSkin.Cli.csproj -c Release -f net10.0 -- baseline .\themes\amchipper\theme-pack.json .\artifacts\visual-baseline.json
dotnet run --project .\src\CopperSkin.Cli\CopperSkin.Cli.csproj -c Release -f net10.0 -- diff .\themes\amchipper\theme-pack.json .\themes\custom\theme-pack.json
dotnet run --project .\src\CopperSkin.Cli\CopperSkin.Cli.csproj -c Release -f net10.0 -- scaffold .\artifacts\starter-theme
dotnet run --project .\src\CopperSkin.Cli\CopperSkin.Cli.csproj -c Release -f net10.0 -- keygen .\artifacts\signing.private .\artifacts\signing.public
dotnet run --project .\src\CopperSkin.Cli\CopperSkin.Cli.csproj -c Release -f net10.0 -- sign .\themes\amchipper\theme-pack.json .\artifacts\signed-theme-pack.json .\artifacts\signing.private
dotnet run --project .\src\CopperSkin.Cli\CopperSkin.Cli.csproj -c Release -f net10.0 -- verify-signature .\artifacts\signed-theme-pack.json .\artifacts\signing.public
dotnet run --project .\src\CopperSkin.Cli\CopperSkin.Cli.csproj -c Release -f net10.0 -- adapters
dotnet run --project .\src\CopperSkin.Cli\CopperSkin.Cli.csproj -c Release -f net10.0 -- pack .\themes\amchipper .\artifacts\CopperSkin.AmChipper.cskin
dotnet run --project .\src\CopperSkin.Cli\CopperSkin.Cli.csproj -c Release -f net10.0 -- lzhc .\Ready2Release .\Ready2Release\CopperSkin.Ready2Release.lzhc
```

## Build And Test

```powershell
dotnet restore .\CopperSkin.slnx
dotnet build .\CopperSkin.slnx -c Release
dotnet test .\CopperSkin.slnx -c Release --no-build
```

## Docs

- [Control Coverage](docs/CONTROL_COVERAGE.md)
- [Theme Format](docs/THEME_FORMAT.md)
- [Changelog](CHANGELOG.md)
