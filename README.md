# CopperSkin

![CopperSkin logo](mainlogo.png)

![Build](https://img.shields.io/badge/build-.NET%208%20%7C%209%20%7C%2010-512bd4)
![License](https://img.shields.io/badge/license-MIT-2ea44f)
![Release](https://img.shields.io/badge/release-0.3.0-2ea44f)

CopperSkin is a reusable WPF theming platform for desktop applications that need runtime theme switching, consistent standard-control styling, custom window chrome, scoped themes, icon authoring, portable theme packs, and deterministic validation.

The `0.3.0` release is the first public release. It includes the reusable Core and WPF libraries, the Designer, command-line tooling, the sample application, the graphics document format, and the release pipeline.

![CopperSkin architecture](https://raw.githubusercontent.com/m4mm0n/CopperSkin/main/docs/assets/copperskin-architecture.svg)

## Contents

- [Install](#install)
- [First application](#first-application)
- [Theme switching](#theme-switching)
- [WPF resources and scoped themes](#wpf-resources-and-scoped-themes)
- [Windows, dialogs, and standard controls](#windows-dialogs-and-standard-controls)
- [Graphics editor and icons](#graphics-editor-and-icons)
- [Theme packs and CLI tooling](#theme-packs-and-cli-tooling)
- [Build and test from source](#build-and-test-from-source)
- [Packages and compatibility](#packages-and-compatibility)
- [Documentation](#documentation)

## What is included

| Project | Purpose | Target frameworks |
| --- | --- | --- |
| `CopperSkin.Core` | Theme model, validation, JSON, archives, signatures, graphics documents, and audits | `netstandard2.0`, `net7.0`, `net8.0`, `net9.0`, `net10.0` |
| `CopperSkin.Wpf` | Runtime resources, implicit styles, scoped themes, chrome, dialogs, and graphics rendering | `net7.0-windows`, `net8.0-windows`, `net9.0-windows`, `net10.0-windows` |
| `CopperSkin.Cli` | Theme export, validation, galleries, baselines, diffs, signatures, and packaging | `net8.0`, `net9.0`, `net10.0` |
| `CopperSkin.Designer` | Live theme preview, token editing, graphics authoring, diagnostics, and export | `net8.0-windows`, `net9.0-windows`, `net10.0-windows` |
| `CopperSkin.SampleKitchenSink` | Runnable visual reference application | `net8.0-windows`, `net9.0-windows`, `net10.0-windows` |

## Install

For a WPF application, install both runtime packages:

```powershell
dotnet add package CopperSkin.Core --version 0.3.0
dotnet add package CopperSkin.Wpf --version 0.3.0
```

`CopperSkin.Core` is renderer-neutral and can be used by services, build tools, and theme-pack pipelines. `CopperSkin.Wpf` adds the WPF resource dictionaries and controls.

## First application

Install CopperSkin during application startup. The built-in catalog supplies the included themes:

```csharp
using System.Windows;
using CopperSkin.Core.Theming;
using CopperSkin.Wpf;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        CopperSkinApp.Use(this)
            .Pack(BuiltInThemeCatalog.Create())
            .Theme("Neon Studio")
            .Backdrop(CopperSkinBackdropKind.Mica)
            .Install();
    }
}
```

The same setup can be expressed in `App.xaml` when the resource dictionary should be available before the first window is created:

```xml
<Application
    x:Class="MyApp.App"
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:copper="clr-namespace:CopperSkin.Wpf;assembly=CopperSkin.Wpf">
  <Application.Resources>
    <ResourceDictionary>
      <ResourceDictionary.MergedDictionaries>
        <copper:CopperSkinThemeResources Theme="Neon Studio" />
      </ResourceDictionary.MergedDictionaries>
    </ResourceDictionary>
  </Application.Resources>
</Application>
```

## Theme switching

Keep a reference to the theme manager when the user can change themes at runtime:

```csharp
using CopperSkin.Core.Theming;
using CopperSkin.Wpf;

var manager = CopperSkinApp.Use(Application.Current)
    .Pack(BuiltInThemeCatalog.Create())
    .Theme("Copper Desk")
    .Install();

manager.ApplyTheme("FL Grape");
```

Themes are token-based. Controls consume dynamic resources, so changing the active theme updates the application without rebuilding the visual tree. Use the built-in catalog as a starting point or load a validated `ThemePack` from JSON.

## WPF resources and scoped themes

Apply a different theme to a subtree with `CopperSkinThemeScope`:

```xml
<Border
    xmlns:cs="clr-namespace:CopperSkin.Wpf;assembly=CopperSkin.Wpf"
    cs:CopperSkinThemeScope.Theme="Copper Desk"
    Padding="16">
  <StackPanel>
    <TextBlock Text="This subtree has its own theme." />
    <Button Content="Continue" />
  </StackPanel>
</Border>
```

The `0.3.0` control wave covers common WPF input, navigation, data, text, layout, and dialog surfaces with shared focus, validation, disabled/read-only, keyboard-navigation, RTL, and theme-switch behavior. See [control coverage](docs/CONTROL_COVERAGE.md) for the exact support boundary.

## Windows, dialogs, and standard controls

Use `CopperWindow` when CopperSkin should own the application chrome:

```xml
<controls:CopperWindow
    x:Class="MyApp.MainWindow"
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:controls="clr-namespace:CopperSkin.Wpf.Controls;assembly=CopperSkin.Wpf"
    Title="My Copper App"
    Width="1100"
    Height="720">
  <Grid>
    <StackPanel Margin="24">
      <TextBlock Text="CopperSkin" FontSize="26" />
      <TextBox Margin="0,16,0,0" />
      <Button Content="Save" Margin="0,12,0,0" />
    </StackPanel>
  </Grid>
</controls:CopperWindow>
```

Show themed dialogs from code:

```csharp
using CopperSkin.Wpf.Controls;

CopperMessageBox.Show(this, "Saved", "The theme pack was exported.", "Nice");

new CopperTaskDialog
{
    Title = "Validation",
    Heading = "All good",
    Text = "No blocking theme errors were found.",
    Icon = CopperTaskDialogIcon.Information
}.Show(this);
```

## Graphics editor and icons

Run the Designer to create vector-first icons and basic paint documents with layers, visibility and lock controls, rectangle/ellipse/line/freehand tools, zoom, undo/redo, save/open, and SVG/XAML/PNG export.

![CopperSkin graphics editor workflow](https://raw.githubusercontent.com/m4mm0n/CopperSkin/main/docs/assets/copperskin-graphics-editor.svg)

The document model is renderer-neutral, so an icon can be authored once, validated in a build, rendered in WPF, and embedded in a signed theme pack:

```csharp
using CopperSkin.Core.Graphics;

var icon = new GraphicDocument
{
    Id = "status.ok",
    Name = "Status OK",
    DocumentType = GraphicDocumentType.Icon,
    Width = 24,
    Height = 24,
    Layers =
    [
        new GraphicLayer
        {
            Id = "mark",
            Name = "Mark",
            Elements =
            [
                new GraphicElement
                {
                    Id = "circle",
                    Kind = GraphicElementKind.Ellipse,
                    Geometry = new GraphicGeometry
                    {
                        Bounds = new GraphicRect(2, 2, 20, 20)
                    },
                    Style = new GraphicStyle
                    {
                        FillToken = "color.status.success"
                    }
                }
            ]
        }
    ]
};

var json = GraphicDocumentSerializer.Serialize(icon);
var diagnostics = GraphicDocumentValidator.Validate(icon);
```

Render the same document in WPF:

```xml
<copper:CopperIcon
    Document="{Binding StatusIcon}"
    AccessibleName="Success"
    Width="24"
    Height="24"
    xmlns:copper="clr-namespace:CopperSkin.Wpf.Controls;assembly=CopperSkin.Wpf" />
```

![CopperSkin release flow](https://raw.githubusercontent.com/m4mm0n/CopperSkin/main/docs/assets/copperskin-release-flow.svg)

## Theme packs and CLI tooling

Run the Designer:

```powershell
dotnet run --project .\src\CopperSkin.Designer\CopperSkin.Designer.csproj -c Release -f net10.0-windows
```

Export and validate built-in themes:

```powershell
$cli = ".\src\CopperSkin.Cli\CopperSkin.Cli.csproj"
dotnet run --project $cli -c Release -f net10.0 -- list
dotnet run --project $cli -c Release -f net10.0 -- export-builtins .\artifacts\theme-pack.json
dotnet run --project $cli -c Release -f net10.0 -- validate .\artifacts\theme-pack.json
dotnet run --project $cli -c Release -f net10.0 -- gallery .\artifacts\theme-pack.json .\artifacts\gallery
dotnet run --project $cli -c Release -f net10.0 -- baseline .\artifacts\theme-pack.json .\artifacts\visual-baseline.json
```

Validate a graphics document in a headless build:

```powershell
dotnet run --project $cli -c Release -f net10.0 -- graphics validate .\status.ok.cgraphic
dotnet run --project $cli -c Release -f net10.0 -- graphics inspect .\status.ok.cgraphic
dotnet run --project $cli -c Release -f net10.0 -- graphics export .\status.ok.cgraphic .\artifacts\status.ok.json
```

Create and verify a signed theme pack. Keep the private key outside source control:

```powershell
dotnet run --project $cli -c Release -f net10.0 -- keygen .\artifacts\signing.private .\artifacts\signing.public
dotnet run --project $cli -c Release -f net10.0 -- sign .\artifacts\theme-pack.json .\artifacts\signed-theme-pack.json .\artifacts\signing.private
dotnet run --project $cli -c Release -f net10.0 -- verify-signature .\artifacts\signed-theme-pack.json .\artifacts\signing.public
```

Read [the graphics format](docs/GRAPHICS_FORMAT.md) and [the theme format](docs/THEME_FORMAT.md) before building a custom pack.

## Build and test from source

Requirements:

- Windows for WPF projects and visual tests.
- .NET SDK `10.0.301` or a compatible SDK with the .NET 8, 9, and 10 targeting packs.

From the repository root:

```powershell
dotnet restore .\CopperSkin.slnx
dotnet build .\CopperSkin.slnx --configuration Release --no-restore
dotnet test .\CopperSkin.slnx --configuration Release --no-build
dotnet pack .\src\CopperSkin.Core\CopperSkin.Core.csproj --configuration Release --output .\artifacts\packages
dotnet pack .\src\CopperSkin.Wpf\CopperSkin.Wpf.csproj --configuration Release --output .\artifacts\packages
```

The Windows CI workflow runs the same restore, warnings-as-errors build, multi-target test matrix, and package checks. Release tags beginning with `v0.3.` additionally publish through NuGet Trusted Publishing using GitHub OIDC; see [NuGet publishing](docs/NUGET_PUBLISHING.md).

## Packages and compatibility

| Package | Use it when |
| --- | --- |
| `CopperSkin.Core` | You need theme models, validation, serialization, signatures, or graphics documents without WPF. |
| `CopperSkin.Wpf` | You need WPF resources, controls, window chrome, dialogs, or graphics rendering. |

Package version `0.3.0` contains assembly/file version `0.3.0.0`. Existing theme packs without graphics remain readable. The graphics schema is versioned independently from the assembly version.

## Documentation

- [Getting started](docs/GETTING_STARTED.md)
- [Graphics format](docs/GRAPHICS_FORMAT.md)
- [Accessibility and control behavior](docs/ACCESSIBILITY.md)
- [Theme format](docs/THEME_FORMAT.md)
- [Control coverage](docs/CONTROL_COVERAGE.md)
- [Release checklist](docs/RELEASE_CHECKLIST.md)
- [Release notes](RELEASE_NOTES.md)
- [Changelog](CHANGELOG.md)

## License

CopperSkin is released under the [MIT License](LICENSE).
