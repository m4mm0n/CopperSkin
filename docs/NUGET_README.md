# CopperSkin

CopperSkin is a reusable WPF theming platform for desktop applications. It provides runtime theme switching, consistent control styling, custom window chrome, dialogs, graphics rendering, and typed theme-pack tooling.

## Install

```powershell
dotnet add package CopperSkin.Core --version 0.3.1
dotnet add package CopperSkin.Wpf --version 0.3.1
```

Use `CopperSkin.Core` for renderer-neutral theme and graphics documents. Add `CopperSkin.Wpf` for WPF resources, controls, dialogs, window chrome, and rendering.

## Quick start

Merge the CopperSkin resources into `Application.Resources`:

```xml
<Application.Resources>
    <ResourceDictionary>
        <ResourceDictionary.MergedDictionaries>
            <ResourceDictionary Source="pack://application:,,,/CopperSkin.Wpf;component/Themes/CopperSkinThemeResources.xaml" />
        </ResourceDictionary.MergedDictionaries>
    </ResourceDictionary>
</Application.Resources>
```

Then initialize the runtime during application startup:

```csharp
using CopperSkin.Wpf;

CopperSkinApp.Use(Application.Current);
```

## Documentation

- [Getting started](https://github.com/m4mm0n/CopperSkin/blob/main/docs/GETTING_STARTED.md)
- [Control coverage](https://github.com/m4mm0n/CopperSkin/blob/main/docs/CONTROL_COVERAGE.md)
- [Graphics format](https://github.com/m4mm0n/CopperSkin/blob/main/docs/GRAPHICS_FORMAT.md)
- [Theme format](https://github.com/m4mm0n/CopperSkin/blob/main/docs/THEME_FORMAT.md)
- [Complete project README](https://github.com/m4mm0n/CopperSkin#readme)

## Links

- [CopperSkin.Core on NuGet](https://www.nuget.org/packages/CopperSkin.Core)
- [CopperSkin.Wpf on NuGet](https://www.nuget.org/packages/CopperSkin.Wpf)
- [Source repository](https://github.com/m4mm0n/CopperSkin)
- [MIT license](https://github.com/m4mm0n/CopperSkin/blob/main/LICENSE)
