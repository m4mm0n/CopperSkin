# CopperSkin WPF Theme Engine Plan

## Goal

Build CopperSkin into a complete WPF theme engine extracted from the amChipper custom skin, preserving the epic glossy tracker-studio look while turning it into a reusable, packageable, typed, validated, designer-friendly, runtime-switchable WPF theming system.

The end state is not "a XAML dictionary with colors." The end state is:

- A core theme model with typed semantic, component, drawing, typography, density, motion, effect, and native-chrome tokens.
- A WPF runtime that can install, apply, preview, scope, and hot-reload themes without restarting the app.
- A theme pack format that can ship as loose files, `.cskin` archives, or embedded NuGet resources.
- A kitchen-sink sample and a dedicated designer app for editing and previewing themes.
- A CLI for importing, validating, diffing, exporting, and screenshot-testing theme packs.
- An analyzer/build tool that finds hard-coded colors and missing token usage in WPF apps.
- An amChipper compatibility adapter so the original app can migrate without losing the current look or saved settings.

## Architecture

CopperSkin should be a small suite of projects:

- `src/CopperSkin.Core`: platform-neutral theme model, catalog, validation, serialization, inheritance, contrast checks, token references, and diagnostics.
- `src/CopperSkin.Wpf`: WPF runtime, resource dictionary emission, `ThemeManager`, attached properties, DWM chrome integration, style dictionaries, and custom drawing theme snapshots.
- `src/CopperSkin.Cli`: command-line tools for import, validate, diff, export, pack, unpack, and audit.
- `src/CopperSkin.Designer`: WPF theme studio for editing theme packs with live preview and gallery pages.
- `samples/CopperSkin.SampleKitchenSink`: WPF sample app that exercises every standard style and drawing token.
- `tests/CopperSkin.Core.Tests`: unit tests for parsing, validation, inheritance, contrast, alias resolution, and diagnostics.
- `tests/CopperSkin.Wpf.Tests`: STA WPF tests for resource application, dynamic update, per-window scope, dictionary emission, and DWM service abstraction.
- `tests/CopperSkin.Cli.Tests`: CLI contract tests.
- `tests/CopperSkin.Visual.Tests`: optional visual snapshot tests for the kitchen sink and designer preview.
- `eng`: build scripts, packaging, and verification helpers.
- `themes`: first-party theme packs imported from amChipper.
- `docs`: user docs, migration docs, token reference, and pack authoring guide.

The engine has three layers:

1. Theme data layer: immutable definitions, typed tokens, inheritance, aliases, validation.
2. Runtime layer: `CopperSkinThemeManager` applies resolved snapshots to WPF resources and notifies custom drawing surfaces.
3. Product layer: designer, CLI, sample, analyzers, docs, and migration adapter.

## Tech Stack

- Windows WPF on `.NET 10`.
- C# latest, nullable enabled.
- `.slnx` solution format.
- XAML resource dictionaries for final WPF assets.
- JSON for the first theme pack schema. Avoid YAML until there is a strong reason to add another dependency.
- xUnit for tests.
- STA WPF test helpers in-house unless a test dependency proves worth it.
- NuGet packaging for `CopperSkin.Core` and `CopperSkin.Wpf`.

## Baseline/Authority Refs

- `E:\Repos\ZLS\amChipper\src\amChipper.App\Theme\DarkTheme.xaml`: current resource dictionary and control style source.
- `E:\Repos\ZLS\amChipper\src\amChipper.App\ViewModels\MainViewModel.cs`: current theme list, palette dictionaries, runtime apply logic, and persisted settings.
- `E:\Repos\ZLS\amChipper\src\amChipper.App\Services\WindowChromeTheme.cs`: current native titlebar integration.
- `E:\Repos\ZLS\amChipper\src\amChipper.App\MainWindow.xaml`: current menu and settings UI using theme options.
- `E:\Repos\ZLS\amChipper\src\amChipper.App\Controls\PatternEditor\PatternGridCanvas.cs`, `PianoRoll\NoteGridCanvas.cs`, `SongEditor\TimelineCanvas.cs`, and related custom canvases: drawing-surface migration targets.
- `docs/aegis/baseline/2026-05-25-amchipper-skin-baseline.md`: local CopperSkin baseline from the source inspection.

## Compatibility Boundary

CopperSkin v1 must keep the amChipper skin stable while extracting it.

- Existing resource keys such as `BgDeep`, `Accent`, `FlStudioChrome`, and `SoftGlow` remain available as aliases.
- Existing amChipper saved settings that store theme names like `FL Grape`, `Copper Desk`, or `Deep Red` must still resolve.
- WPF apps must be able to install CopperSkin with `DynamicResource` and get runtime theme switching without rewriting every control.
- The engine must support app-level, window-level, and preview-scope theming.
- Custom drawing controls must get frozen brush snapshots for performance, but still refresh on theme changes.
- Native DWM chrome integration must be optional and Windows-only.
- No hard dependency on amChipper should remain in CopperSkin packages.

## Verification

Main verification commands after each implementation slice:

```powershell
dotnet build .\CopperSkin.slnx -c Release
dotnet test .\CopperSkin.slnx -c Release
dotnet run --project .\src\CopperSkin.Cli\CopperSkin.Cli.csproj -- validate .\themes\amchipper
dotnet run --project .\src\CopperSkin.Cli\CopperSkin.Cli.csproj -- audit .\samples\CopperSkin.SampleKitchenSink
dotnet run --project .\src\CopperSkin.Designer\CopperSkin.Designer.csproj
```

Visual gates:

- Kitchen sink launches and can switch all first-party themes without restart.
- The designer can edit a token and immediately preview the result.
- The amChipper compatibility sample preserves `FL Grape`, `Neon Studio`, and `Copper Desk` visually enough to be recognizable.
- High contrast validation reports real contrast failures and does not block valid themes.

## Plan Basis

Facts:

- `CopperSkin` is currently an empty Git repo with no project files.
- The machine has .NET SDK `10.0.203` and Windows Desktop runtime `10.0.7`.
- amChipper is a WPF `.NET 10` app.
- amChipper already has 24 theme names, runtime palette switching, theme persistence, custom WPF styles, native titlebar coloring, and custom drawing surfaces.
- Many amChipper controls use `DynamicResource`, which gives us a realistic migration path.

Assumptions:

- CopperSkin should be its own reusable project, not just code moved inside amChipper.
- The first target is WPF on Windows. WinUI, MAUI, Avalonia, and web exporters are future expansion, not v1.
- The original amChipper look is the visual north star, but the engine should support many apps and design systems.

Unknowns:

- Whether the first public package should use `CopperSkin.*` IDs or a different brand.
- Whether the designer app should ship as a standalone release in v1 or stay as developer tooling until v1.1.
- Whether screenshot diffing should use a third-party package or a small in-house bitmap comparison harness.

Ripple Signal Triage:

- Owner expands from `amChipper.App.MainViewModel` and `DarkTheme.xaml` to `CopperSkin.Core` plus `CopperSkin.Wpf`.
- Downstream consumers include amChipper, the CopperSkin designer, sample apps, and any WPF app installing the NuGet package.
- Contract scope includes resource keys, token schema, serialized theme packs, runtime APIs, and visual compatibility.
- Verification scope must include unit tests, STA WPF tests, CLI tests, visual smoke tests, and migration checks.

## File Map

Create:

- `CopperSkin.slnx`
- `Directory.Build.props`
- `.editorconfig`
- `README.md`
- `docs/token-reference.md`
- `docs/theme-pack-format.md`
- `docs/amchipper-migration.md`
- `docs/visual-verification.md`
- `src/CopperSkin.Core/CopperSkin.Core.csproj`
- `src/CopperSkin.Core/Themes/ThemeDefinition.cs`
- `src/CopperSkin.Core/Themes/ThemeCatalog.cs`
- `src/CopperSkin.Core/Themes/ThemePack.cs`
- `src/CopperSkin.Core/Themes/ThemeResolver.cs`
- `src/CopperSkin.Core/Tokens/ThemeTokenKey.cs`
- `src/CopperSkin.Core/Tokens/ThemeTokenValue.cs`
- `src/CopperSkin.Core/Tokens/ResolvedThemeSnapshot.cs`
- `src/CopperSkin.Core/Validation/ThemeValidator.cs`
- `src/CopperSkin.Core/Validation/ContrastValidator.cs`
- `src/CopperSkin.Core/Serialization/ThemeJsonSerializer.cs`
- `src/CopperSkin.Core/Diagnostics/ThemeDiagnostic.cs`
- `src/CopperSkin.Wpf/CopperSkin.Wpf.csproj`
- `src/CopperSkin.Wpf/CopperSkinThemeManager.cs`
- `src/CopperSkin.Wpf/CopperSkinThemeOptions.cs`
- `src/CopperSkin.Wpf/Resources/CopperSkinResourceEmitter.cs`
- `src/CopperSkin.Wpf/Resources/ResourceDictionaryExtensions.cs`
- `src/CopperSkin.Wpf/Chrome/WpfWindowChromeService.cs`
- `src/CopperSkin.Wpf/Drawing/DrawingThemeSnapshot.cs`
- `src/CopperSkin.Wpf/Drawing/IThemeAwareDrawingSurface.cs`
- `src/CopperSkin.Wpf/Themes/Generic.xaml`
- `src/CopperSkin.Wpf/Themes/CopperSkin.Base.xaml`
- `src/CopperSkin.Wpf/Themes/CopperSkin.Controls.xaml`
- `src/CopperSkin.Wpf/Themes/CopperSkin.LegacyAliases.xaml`
- `src/CopperSkin.Cli/CopperSkin.Cli.csproj`
- `src/CopperSkin.Cli/Program.cs`
- `src/CopperSkin.Designer/CopperSkin.Designer.csproj`
- `src/CopperSkin.Designer/App.xaml`
- `src/CopperSkin.Designer/MainWindow.xaml`
- `src/CopperSkin.Designer/ViewModels/DesignerViewModel.cs`
- `samples/CopperSkin.SampleKitchenSink/CopperSkin.SampleKitchenSink.csproj`
- `samples/CopperSkin.SampleKitchenSink/App.xaml`
- `samples/CopperSkin.SampleKitchenSink/MainWindow.xaml`
- `themes/amchipper/theme-pack.json`
- `themes/amchipper/fl-grape.json`
- `themes/amchipper/copper-desk.json`
- `themes/amchipper/*.json` for the remaining amChipper themes.
- `tests/CopperSkin.Core.Tests/CopperSkin.Core.Tests.csproj`
- `tests/CopperSkin.Wpf.Tests/CopperSkin.Wpf.Tests.csproj`
- `tests/CopperSkin.Cli.Tests/CopperSkin.Cli.Tests.csproj`
- `tests/CopperSkin.Visual.Tests/CopperSkin.Visual.Tests.csproj`
- `eng/build.ps1`
- `eng/test.ps1`
- `eng/package.ps1`

Modify later in amChipper after packages exist:

- `E:\Repos\ZLS\amChipper\src\amChipper.App\amChipper.App.csproj`
- `E:\Repos\ZLS\amChipper\src\amChipper.App\App.xaml`
- `E:\Repos\ZLS\amChipper\src\amChipper.App\Theme\DarkTheme.xaml`
- `E:\Repos\ZLS\amChipper\src\amChipper.App\ViewModels\MainViewModel.cs`
- `E:\Repos\ZLS\amChipper\src\amChipper.App\Services\WindowChromeTheme.cs`
- Custom drawing controls under `E:\Repos\ZLS\amChipper\src\amChipper.App\Controls`

## Public API Shape

Target install experience:

```csharp
CopperSkinThemeManager.Install(
    Application.Current,
    CopperSkinThemeCatalog.LoadEmbeddedDefaults(),
    new CopperSkinThemeOptions
    {
        ApplyNativeWindowChrome = true,
        IncludeLegacyAmChipperAliases = true
    });

await CopperSkinThemeManager.Current.ApplyAsync("Copper Desk");
```

Target XAML experience:

```xml
<Application.Resources>
  <ResourceDictionary>
    <ResourceDictionary.MergedDictionaries>
      <ResourceDictionary Source="pack://application:,,,/CopperSkin.Wpf;component/Themes/CopperSkin.Base.xaml"/>
      <ResourceDictionary Source="pack://application:,,,/CopperSkin.Wpf;component/Themes/CopperSkin.Controls.xaml"/>
    </ResourceDictionary.MergedDictionaries>
  </ResourceDictionary>
</Application.Resources>
```

Target custom drawing experience:

```csharp
public sealed class PatternGridCanvas : FrameworkElement, IThemeAwareDrawingSurface
{
    private DrawingThemeSnapshot _theme = DrawingThemeSnapshot.Default;

    public void ApplyTheme(DrawingThemeSnapshot theme)
    {
        _theme = theme;
        InvalidateVisual();
    }
}
```

## Task 1 - Initialize the CopperSkin solution

Files:

- Create `CopperSkin.slnx`
- Create `Directory.Build.props`
- Create `.editorconfig`
- Create project folders under `src`, `tests`, `samples`, `themes`, `docs`, and `eng`

Why:

Give the engine its own clean structure before moving logic out of amChipper.

Impact/Compatibility:

No amChipper changes. This is additive.

Steps:

1. Write test:

```powershell
dotnet new xunit -n CopperSkin.Core.Tests -o tests\CopperSkin.Core.Tests -f net10.0
```

2. Verify RED:

```powershell
dotnet test .\tests\CopperSkin.Core.Tests\CopperSkin.Core.Tests.csproj -c Release
```

Expected: the generated test passes, but the solution does not exist yet. This verifies the SDK is usable.

3. Minimal code:

```powershell
dotnet new sln -n CopperSkin
dotnet new classlib -n CopperSkin.Core -o src\CopperSkin.Core -f net10.0
dotnet new classlib -n CopperSkin.Wpf -o src\CopperSkin.Wpf -f net10.0-windows
dotnet new console -n CopperSkin.Cli -o src\CopperSkin.Cli -f net10.0
dotnet new wpf -n CopperSkin.Designer -o src\CopperSkin.Designer -f net10.0-windows
dotnet new wpf -n CopperSkin.SampleKitchenSink -o samples\CopperSkin.SampleKitchenSink -f net10.0-windows
dotnet new xunit -n CopperSkin.Wpf.Tests -o tests\CopperSkin.Wpf.Tests -f net10.0-windows
dotnet new xunit -n CopperSkin.Cli.Tests -o tests\CopperSkin.Cli.Tests -f net10.0
dotnet new xunit -n CopperSkin.Visual.Tests -o tests\CopperSkin.Visual.Tests -f net10.0-windows
dotnet sln .\CopperSkin.slnx add src\CopperSkin.Core\CopperSkin.Core.csproj src\CopperSkin.Wpf\CopperSkin.Wpf.csproj src\CopperSkin.Cli\CopperSkin.Cli.csproj src\CopperSkin.Designer\CopperSkin.Designer.csproj samples\CopperSkin.SampleKitchenSink\CopperSkin.SampleKitchenSink.csproj tests\CopperSkin.Core.Tests\CopperSkin.Core.Tests.csproj tests\CopperSkin.Wpf.Tests\CopperSkin.Wpf.Tests.csproj tests\CopperSkin.Cli.Tests\CopperSkin.Cli.Tests.csproj tests\CopperSkin.Visual.Tests\CopperSkin.Visual.Tests.csproj
dotnet add src\CopperSkin.Wpf\CopperSkin.Wpf.csproj reference src\CopperSkin.Core\CopperSkin.Core.csproj
dotnet add src\CopperSkin.Cli\CopperSkin.Cli.csproj reference src\CopperSkin.Core\CopperSkin.Core.csproj
dotnet add src\CopperSkin.Designer\CopperSkin.Designer.csproj reference src\CopperSkin.Core\CopperSkin.Core.csproj src\CopperSkin.Wpf\CopperSkin.Wpf.csproj
dotnet add samples\CopperSkin.SampleKitchenSink\CopperSkin.SampleKitchenSink.csproj reference src\CopperSkin.Core\CopperSkin.Core.csproj src\CopperSkin.Wpf\CopperSkin.Wpf.csproj
dotnet add tests\CopperSkin.Core.Tests\CopperSkin.Core.Tests.csproj reference src\CopperSkin.Core\CopperSkin.Core.csproj
dotnet add tests\CopperSkin.Wpf.Tests\CopperSkin.Wpf.Tests.csproj reference src\CopperSkin.Wpf\CopperSkin.Wpf.csproj
dotnet add tests\CopperSkin.Cli.Tests\CopperSkin.Cli.Tests.csproj reference src\CopperSkin.Cli\CopperSkin.Cli.csproj
dotnet add tests\CopperSkin.Visual.Tests\CopperSkin.Visual.Tests.csproj reference samples\CopperSkin.SampleKitchenSink\CopperSkin.SampleKitchenSink.csproj
```

Patch `src/CopperSkin.Wpf/CopperSkin.Wpf.csproj` and WPF test/sample projects to include:

```xml
<UseWPF>true</UseWPF>
<Nullable>enable</Nullable>
<ImplicitUsings>enable</ImplicitUsings>
<LangVersion>latest</LangVersion>
```

4. Verify GREEN:

```powershell
dotnet build .\CopperSkin.slnx -c Release
dotnet test .\CopperSkin.slnx -c Release
```

5. Commit:

```powershell
git add .
git commit -m "Initialize CopperSkin solution"
```

## Task 2 - Model typed theme tokens in CopperSkin.Core

Files:

- `src/CopperSkin.Core/Tokens/ThemeTokenKey.cs`
- `src/CopperSkin.Core/Tokens/ThemeTokenValue.cs`
- `src/CopperSkin.Core/Themes/ThemeDefinition.cs`
- `src/CopperSkin.Core/Themes/ThemePack.cs`
- `tests/CopperSkin.Core.Tests/ThemeDefinitionTests.cs`

Why:

The current amChipper theme is string-keyed dictionaries. The engine needs typed contracts before it can validate, emit resources, or support packs.

Impact/Compatibility:

Keep legacy keys as aliases, but make the canonical tokens richer.

Token groups:

- Semantic colors: surface, panel, control, hover, selected, border, text, disabled, accent, accentAlt, warning, danger, success.
- Component colors: button, menu, tab, slider, scrollbar, combo, checkbox, textbox, tooltip, statusbar.
- Drawing colors: gridLine, gridBar, playhead, pianoWhiteKey, pianoBlackKey, trackerNote, trackerInstrument, trackerVolume, trackerEffect, clip, automationLine.
- Typography: uiFont, monoFont, titleFont, baseSize, denseSize, editorSize.
- Metrics: radiusXs, radiusSm, radiusMd, borderThin, scrollbarSize, focusThickness, densityScale.
- Effects: shineOpacity, panelShadowOpacity, glowOpacity, glowRadius, acrylicOpacity.
- Motion: transitionMs, popupAnimation, reduceMotion.
- Chrome: caption, captionText, captionBorder, immersiveDarkMode.

Steps:

1. Write test:

```powershell
dotnet test .\tests\CopperSkin.Core.Tests\CopperSkin.Core.Tests.csproj -c Release --filter ThemeDefinitionTests
```

Expected: no matching tests until `ThemeDefinitionTests` is added.

2. Verify RED:

Add tests asserting:

- Duplicate token keys fail validation.
- Legacy alias `BgDeep` resolves to canonical `color.surface.deep`.
- Color token accepts `#AARRGGBB`.
- Invalid color reports a diagnostic with token path.

Run:

```powershell
dotnet test .\tests\CopperSkin.Core.Tests\CopperSkin.Core.Tests.csproj -c Release --filter ThemeDefinitionTests
```

Expected: compile fails until the model exists.

3. Minimal code:

Create immutable records for:

- `ThemeTokenKey`
- `ThemeTokenValue`
- `ThemeDefinition`
- `ThemePack`

Use no WPF references in Core.

4. Verify GREEN:

```powershell
dotnet test .\tests\CopperSkin.Core.Tests\CopperSkin.Core.Tests.csproj -c Release --filter ThemeDefinitionTests
```

5. Commit:

```powershell
git add .
git commit -m "Add typed theme token model"
```

## Task 3 - Import the amChipper theme catalog

Files:

- `themes/amchipper/theme-pack.json`
- `themes/amchipper/fl-grape.json`
- `themes/amchipper/neon-studio.json`
- `themes/amchipper/classic-tracker.json`
- Remaining amChipper theme JSON files.
- `src/CopperSkin.Core/Import/AmChipperThemeImporter.cs`
- `tests/CopperSkin.Core.Tests/AmChipperThemeImporterTests.cs`

Why:

The first engine proof is importing the actual amChipper skin and all 24 named variants.

Impact/Compatibility:

The exact theme names stay stable for amChipper settings compatibility.

Steps:

1. Write test:

```powershell
dotnet test .\tests\CopperSkin.Core.Tests\CopperSkin.Core.Tests.csproj -c Release --filter AmChipperThemeImporterTests
```

2. Verify RED:

Test expectations:

- Import returns exactly 24 themes.
- `FL Grape`, `Neon Studio`, `Copper Desk`, and `Deep Red` resolve.
- Legacy keys `BgDeep`, `BgPanel`, `Accent`, and `TextPrimary` map to canonical tokens.
- Each theme has at least the current 10 palette values from `MainViewModel.ApplyTheme`.

3. Minimal code:

Build a one-time importer that can read a compact JSON transcription of the amChipper dictionaries. Do not parse C# source at runtime. Source parsing can be an optional tool later.

4. Verify GREEN:

```powershell
dotnet test .\tests\CopperSkin.Core.Tests\CopperSkin.Core.Tests.csproj -c Release --filter AmChipperThemeImporterTests
```

5. Commit:

```powershell
git add .
git commit -m "Import amChipper theme catalog"
```

## Task 4 - Build theme resolution, inheritance, and aliases

Files:

- `src/CopperSkin.Core/Themes/ThemeResolver.cs`
- `src/CopperSkin.Core/Tokens/ResolvedThemeSnapshot.cs`
- `src/CopperSkin.Core/Tokens/LegacyTokenAliases.cs`
- `tests/CopperSkin.Core.Tests/ThemeResolverTests.cs`

Why:

Themes need base inheritance, aliases, derived brushes, and stable snapshots.

Impact/Compatibility:

Existing amChipper keys can be provided as aliases while new apps use canonical names.

Steps:

1. Write test:

```powershell
dotnet test .\tests\CopperSkin.Core.Tests\CopperSkin.Core.Tests.csproj -c Release --filter ThemeResolverTests
```

2. Verify RED:

Tests assert:

- A child theme inherits missing tokens from base.
- Aliases resolve to the same value as canonical keys.
- Missing required tokens produce diagnostics.
- Resolved snapshots are immutable.

3. Minimal code:

Implement `ThemeResolver.Resolve(ThemePack pack, string themeId)` returning `ResolvedThemeSnapshot`.

4. Verify GREEN:

```powershell
dotnet test .\tests\CopperSkin.Core.Tests\CopperSkin.Core.Tests.csproj -c Release --filter ThemeResolverTests
```

5. Commit:

```powershell
git add .
git commit -m "Resolve inherited themes and aliases"
```

## Task 5 - Validate accessibility, contrast, and token completeness

Files:

- `src/CopperSkin.Core/Validation/ThemeValidator.cs`
- `src/CopperSkin.Core/Validation/ContrastValidator.cs`
- `src/CopperSkin.Core/Diagnostics/ThemeDiagnostic.cs`
- `tests/CopperSkin.Core.Tests/ThemeValidatorTests.cs`

Why:

An epic theme engine should keep wild themes usable. The editor must warn when colors look cool but fail actual readability.

Impact/Compatibility:

Validation should warn by default, not block legacy amChipper imports unless the pack asks for strict mode.

Steps:

1. Write test:

```powershell
dotnet test .\tests\CopperSkin.Core.Tests\CopperSkin.Core.Tests.csproj -c Release --filter ThemeValidatorTests
```

2. Verify RED:

Tests assert:

- Missing semantic text tokens are errors.
- Low text contrast is a warning in compatibility mode.
- Invalid color string is an error.
- Unknown token is a warning with a suggested canonical name if alias exists.

3. Minimal code:

Implement WCAG-style relative luminance and contrast checks in Core with no WPF dependency.

4. Verify GREEN:

```powershell
dotnet test .\tests\CopperSkin.Core.Tests\CopperSkin.Core.Tests.csproj -c Release --filter ThemeValidatorTests
```

5. Commit:

```powershell
git add .
git commit -m "Validate theme accessibility and completeness"
```

## Task 6 - Emit WPF ResourceDictionaries

Files:

- `src/CopperSkin.Wpf/Resources/CopperSkinResourceEmitter.cs`
- `src/CopperSkin.Wpf/Resources/ResourceDictionaryExtensions.cs`
- `src/CopperSkin.Wpf/Themes/CopperSkin.Base.xaml`
- `src/CopperSkin.Wpf/Themes/CopperSkin.LegacyAliases.xaml`
- `tests/CopperSkin.Wpf.Tests/ResourceEmitterTests.cs`

Why:

WPF apps consume resources. The engine must turn typed snapshots into live WPF resources, gradients, effects, and aliases.

Impact/Compatibility:

Use mutable brushes for live updates, but frozen drawing snapshots for custom canvases.

Steps:

1. Write test:

```powershell
dotnet test .\tests\CopperSkin.Wpf.Tests\CopperSkin.Wpf.Tests.csproj -c Release --filter ResourceEmitterTests
```

2. Verify RED:

Tests assert:

- `BgDeep`, `Accent`, `TextPrimary`, and canonical keys exist after emission.
- `FlStudioChrome`, `FlStudioPanel`, `FlStudioStrip`, and `FlStudioActive` are generated from base tokens.
- Applying a new snapshot mutates existing unfrozen brushes where possible.
- Frozen or missing resources are replaced safely.

3. Minimal code:

Implement `CopperSkinResourceEmitter.Apply(ResourceDictionary target, ResolvedThemeSnapshot snapshot, CopperSkinThemeOptions options)`.

4. Verify GREEN:

```powershell
dotnet test .\tests\CopperSkin.Wpf.Tests\CopperSkin.Wpf.Tests.csproj -c Release --filter ResourceEmitterTests
```

5. Commit:

```powershell
git add .
git commit -m "Emit WPF resources from theme snapshots"
```

## Task 7 - Implement the runtime ThemeManager

Files:

- `src/CopperSkin.Wpf/CopperSkinThemeManager.cs`
- `src/CopperSkin.Wpf/CopperSkinThemeOptions.cs`
- `src/CopperSkin.Wpf/CopperSkinThemeChangedEventArgs.cs`
- `tests/CopperSkin.Wpf.Tests/ThemeManagerTests.cs`

Why:

Apps need one obvious way to install CopperSkin, switch themes, preview themes, and observe changes.

Impact/Compatibility:

Theme switching must work with existing `DynamicResource` consumers and should not require app restart.

Steps:

1. Write test:

```powershell
dotnet test .\tests\CopperSkin.Wpf.Tests\CopperSkin.Wpf.Tests.csproj -c Release --filter ThemeManagerTests
```

2. Verify RED:

Tests assert:

- `Install(Application, ThemeCatalog, options)` creates a singleton manager.
- `ApplyAsync("Copper Desk")` updates application resources.
- `BeginPreviewScope(window, "Neon Studio")` updates only that window and restores on dispose.
- `ThemeChanged` fires once per completed apply.
- Unknown theme returns a diagnostic and leaves the active theme unchanged.

3. Minimal code:

Implement manager over Core resolver and WPF emitter.

4. Verify GREEN:

```powershell
dotnet test .\tests\CopperSkin.Wpf.Tests\CopperSkin.Wpf.Tests.csproj -c Release --filter ThemeManagerTests
```

5. Commit:

```powershell
git add .
git commit -m "Add WPF theme manager"
```

## Task 8 - Port the custom control style dictionary

Files:

- `src/CopperSkin.Wpf/Themes/CopperSkin.Controls.xaml`
- `src/CopperSkin.Wpf/Themes/Generic.xaml`
- `tests/CopperSkin.Wpf.Tests/ControlDictionaryTests.cs`

Why:

The soul of the amChipper skin is not just colors. It is the glossy buttons, sliders, combo boxes, menus, tabs, status bar, shadows, and dense workstation panels.

Impact/Compatibility:

Port styles as reusable defaults, but move app-specific names like `FlStudioPanel` behind CopperSkin aliases.

Steps:

1. Write test:

```powershell
dotnet test .\tests\CopperSkin.Wpf.Tests\CopperSkin.Wpf.Tests.csproj -c Release --filter ControlDictionaryTests
```

2. Verify RED:

Tests assert the dictionary loads and contains styles for:

- `Window`
- `TextBlock`
- `Button`
- `ToggleButton`
- `TextBox`
- `ProgressBar`
- `Slider`
- `ComboBox`
- `CheckBox`
- `GridSplitter`
- `ScrollBar`
- `Menu`
- `MenuItem`
- `Separator`
- `TabControl`
- `TabItem`
- `StatusBar`
- `ToolTip`

3. Minimal code:

Split the current `DarkTheme.xaml` into reusable base resources and control styles. Replace app-specific hard-coded colors inside templates with semantic or component tokens.

4. Verify GREEN:

```powershell
dotnet test .\tests\CopperSkin.Wpf.Tests\CopperSkin.Wpf.Tests.csproj -c Release --filter ControlDictionaryTests
```

5. Commit:

```powershell
git add .
git commit -m "Port CopperSkin WPF control styles"
```

## Task 9 - Theme custom drawing surfaces

Files:

- `src/CopperSkin.Wpf/Drawing/DrawingThemeSnapshot.cs`
- `src/CopperSkin.Wpf/Drawing/IThemeAwareDrawingSurface.cs`
- `src/CopperSkin.Wpf/Drawing/DrawingThemeRegistry.cs`
- `tests/CopperSkin.Wpf.Tests/DrawingThemeTests.cs`

Why:

amChipper has custom rendered editors. A real engine must theme these too, not only ordinary XAML controls.

Impact/Compatibility:

Existing canvases can keep high-performance frozen brushes while refreshing when the theme changes.

Steps:

1. Write test:

```powershell
dotnet test .\tests\CopperSkin.Wpf.Tests\CopperSkin.Wpf.Tests.csproj -c Release --filter DrawingThemeTests
```

2. Verify RED:

Tests assert:

- Drawing snapshot produces frozen brushes and pens.
- Registry tracks `IThemeAwareDrawingSurface` controls weakly.
- Theme change calls `ApplyTheme` on live surfaces.
- Disposed or unloaded surfaces are not leaked.

3. Minimal code:

Implement drawing token snapshot for tracker grids, piano roll, song timeline, automation lanes, meters, and analyzers.

4. Verify GREEN:

```powershell
dotnet test .\tests\CopperSkin.Wpf.Tests\CopperSkin.Wpf.Tests.csproj -c Release --filter DrawingThemeTests
```

5. Commit:

```powershell
git add .
git commit -m "Add drawing surface theme snapshots"
```

## Task 10 - Add native window chrome integration

Files:

- `src/CopperSkin.Wpf/Chrome/WpfWindowChromeService.cs`
- `src/CopperSkin.Wpf/Chrome/IWindowChromeService.cs`
- `tests/CopperSkin.Wpf.Tests/WindowChromeServiceTests.cs`

Why:

amChipper already colors the native titlebar. CopperSkin should make this a first-class optional feature.

Impact/Compatibility:

Windows-only implementation, no failure on non-Windows or unsupported handles.

Steps:

1. Write test:

```powershell
dotnet test .\tests\CopperSkin.Wpf.Tests\CopperSkin.Wpf.Tests.csproj -c Release --filter WindowChromeServiceTests
```

2. Verify RED:

Tests assert:

- Service no-ops when disabled.
- Service maps theme chrome tokens to caption, border, and text colors.
- Non-Windows guard returns success without P/Invoke.
- Failed DWM call reports a warning diagnostic, not an app crash.

3. Minimal code:

Move the pattern from amChipper `WindowChromeTheme` into a testable service with a small P/Invoke boundary.

4. Verify GREEN:

```powershell
dotnet test .\tests\CopperSkin.Wpf.Tests\CopperSkin.Wpf.Tests.csproj -c Release --filter WindowChromeServiceTests
```

5. Commit:

```powershell
git add .
git commit -m "Add optional native window chrome theming"
```

## Task 11 - Build the kitchen-sink sample

Files:

- `samples/CopperSkin.SampleKitchenSink/App.xaml`
- `samples/CopperSkin.SampleKitchenSink/MainWindow.xaml`
- `samples/CopperSkin.SampleKitchenSink/MainWindow.xaml.cs`
- `samples/CopperSkin.SampleKitchenSink/ThemeGalleryViewModel.cs`

Why:

Every theme engine needs a brutal preview surface that exposes weak controls immediately.

Impact/Compatibility:

This sample becomes the visual smoke test before migrating amChipper.

Sample pages:

- Controls
- Menus and popups
- Forms
- Data grid/list density
- Sliders and transport controls
- Tabs and toolbars
- Tracker grid mock
- Piano roll mock
- Analyzer/meter mock
- Dialogs/tooltips
- High visibility and reduced motion toggles

Steps:

1. Write test:

```powershell
dotnet test .\tests\CopperSkin.Visual.Tests\CopperSkin.Visual.Tests.csproj -c Release --filter KitchenSinkLaunchTests
```

2. Verify RED:

Test expects the sample app to load its main window and find a theme selector.

3. Minimal code:

Build a real WPF window using CopperSkin install/apply APIs. Bind the theme selector to catalog themes.

4. Verify GREEN:

```powershell
dotnet test .\tests\CopperSkin.Visual.Tests\CopperSkin.Visual.Tests.csproj -c Release --filter KitchenSinkLaunchTests
dotnet run --project .\samples\CopperSkin.SampleKitchenSink\CopperSkin.SampleKitchenSink.csproj
```

5. Commit:

```powershell
git add .
git commit -m "Add CopperSkin kitchen sink sample"
```

## Task 12 - Build the CopperSkin Designer app

Files:

- `src/CopperSkin.Designer/MainWindow.xaml`
- `src/CopperSkin.Designer/ViewModels/DesignerViewModel.cs`
- `src/CopperSkin.Designer/ViewModels/TokenEditorViewModel.cs`
- `src/CopperSkin.Designer/Views/TokenEditorView.xaml`
- `src/CopperSkin.Designer/Views/PreviewGalleryView.xaml`
- `src/CopperSkin.Designer/Views/DiagnosticsView.xaml`

Why:

The engine becomes far more powerful when themes can be edited live instead of by hand-editing JSON.

Impact/Compatibility:

Designer uses the same Core and WPF runtime as real apps, so it continuously tests the engine.

Features:

- Open theme pack.
- Edit token values.
- Live preview with preview scope.
- Contrast and missing-token diagnostics.
- Theme diff against base theme.
- Export `.cskin` pack.
- Import amChipper palette pack.
- Toggle shine, shadows, density, high visibility, and reduced motion.

Steps:

1. Write test:

```powershell
dotnet test .\tests\CopperSkin.Visual.Tests\CopperSkin.Visual.Tests.csproj -c Release --filter DesignerLaunchTests
```

2. Verify RED:

Test expects designer to load and expose token editor, preview, and diagnostics areas.

3. Minimal code:

Implement one-window designer with a left theme list, center preview, right token inspector, and bottom diagnostics panel.

4. Verify GREEN:

```powershell
dotnet test .\tests\CopperSkin.Visual.Tests\CopperSkin.Visual.Tests.csproj -c Release --filter DesignerLaunchTests
dotnet run --project .\src\CopperSkin.Designer\CopperSkin.Designer.csproj
```

5. Commit:

```powershell
git add .
git commit -m "Add CopperSkin designer app"
```

## Task 13 - Build the CLI

Files:

- `src/CopperSkin.Cli/Program.cs`
- `src/CopperSkin.Cli/Commands/ValidateCommand.cs`
- `src/CopperSkin.Cli/Commands/DiffCommand.cs`
- `src/CopperSkin.Cli/Commands/PackCommand.cs`
- `src/CopperSkin.Cli/Commands/UnpackCommand.cs`
- `src/CopperSkin.Cli/Commands/AuditCommand.cs`
- `tests/CopperSkin.Cli.Tests/*.cs`

Why:

Theme packs need repeatable automation for CI, packaging, and app migration.

Impact/Compatibility:

CLI must work without WPF for Core operations. Audit can scan XAML and C# files for hard-coded colors.

Commands:

```powershell
copperskin validate .\themes\amchipper
copperskin diff .\themes\amchipper\fl-grape.json .\themes\amchipper\copper-desk.json
copperskin pack .\themes\amchipper -o .\artifacts\amchipper.cskin
copperskin unpack .\artifacts\amchipper.cskin -o .\artifacts\unpacked
copperskin audit E:\Repos\ZLS\amChipper\src\amChipper.App
```

Steps:

1. Write test:

```powershell
dotnet test .\tests\CopperSkin.Cli.Tests\CopperSkin.Cli.Tests.csproj -c Release --filter CliCommandTests
```

2. Verify RED:

Tests assert exit codes and machine-readable output for validate, diff, pack, unpack, and audit.

3. Minimal code:

Implement a small command dispatcher with deterministic text and JSON output options.

4. Verify GREEN:

```powershell
dotnet test .\tests\CopperSkin.Cli.Tests\CopperSkin.Cli.Tests.csproj -c Release --filter CliCommandTests
dotnet run --project .\src\CopperSkin.Cli\CopperSkin.Cli.csproj -- validate .\themes\amchipper
```

5. Commit:

```powershell
git add .
git commit -m "Add CopperSkin CLI"
```

## Task 14 - Add hard-coded color audit and migration diagnostics

Files:

- `src/CopperSkin.Core/Audit/HardCodedColorAudit.cs`
- `src/CopperSkin.Core/Audit/WpfResourceAudit.cs`
- `tests/CopperSkin.Core.Tests/HardCodedColorAuditTests.cs`
- `docs/amchipper-migration.md`

Why:

The amChipper scan showed hard-coded colors in XAML and C# canvases. CopperSkin needs tooling to find those in any WPF app.

Impact/Compatibility:

Audit should advise, not auto-rewrite, unless a later explicit migration command is added.

Steps:

1. Write test:

```powershell
dotnet test .\tests\CopperSkin.Core.Tests\CopperSkin.Core.Tests.csproj -c Release --filter HardCodedColorAuditTests
```

2. Verify RED:

Tests assert:

- XAML hex colors are detected with file and line.
- `Color.FromRgb`, `Color.FromArgb`, and `Brushes.White` are detected in C#.
- Allowlist comments suppress intentional non-theme colors.
- Diagnostics suggest the closest token category.

3. Minimal code:

Implement text-based audit with exact line and column diagnostics. Do not mutate files.

4. Verify GREEN:

```powershell
dotnet test .\tests\CopperSkin.Core.Tests\CopperSkin.Core.Tests.csproj -c Release --filter HardCodedColorAuditTests
dotnet run --project .\src\CopperSkin.Cli\CopperSkin.Cli.csproj -- audit E:\Repos\ZLS\amChipper\src\amChipper.App
```

5. Commit:

```powershell
git add .
git commit -m "Audit WPF apps for hard-coded colors"
```

## Task 15 - Package themes and NuGet artifacts

Files:

- `src/CopperSkin.Core/CopperSkin.Core.csproj`
- `src/CopperSkin.Wpf/CopperSkin.Wpf.csproj`
- `eng/package.ps1`
- `themes/amchipper/theme-pack.json`
- `docs/theme-pack-format.md`

Why:

The engine needs clean distribution: NuGet for code and `.cskin` packs for themes.

Impact/Compatibility:

Keep first-party amChipper themes available both embedded and as loose packs.

Steps:

1. Write test:

```powershell
dotnet test .\tests\CopperSkin.Core.Tests\CopperSkin.Core.Tests.csproj -c Release --filter ThemePackArchiveTests
```

2. Verify RED:

Tests assert `.cskin` archives contain:

- `theme-pack.json`
- Theme JSON files.
- Optional preview metadata.
- Stable pack id and version.

3. Minimal code:

Implement pack/unpack logic in Core or CLI shared services. Add package metadata to csproj files.

4. Verify GREEN:

```powershell
dotnet pack .\src\CopperSkin.Core\CopperSkin.Core.csproj -c Release -o .\artifacts\nuget
dotnet pack .\src\CopperSkin.Wpf\CopperSkin.Wpf.csproj -c Release -o .\artifacts\nuget
dotnet run --project .\src\CopperSkin.Cli\CopperSkin.Cli.csproj -- pack .\themes\amchipper -o .\artifacts\amchipper.cskin
```

5. Commit:

```powershell
git add .
git commit -m "Package CopperSkin engine and theme packs"
```

## Task 16 - Migrate amChipper through a compatibility adapter

Files in amChipper:

- `E:\Repos\ZLS\amChipper\src\amChipper.App\amChipper.App.csproj`
- `E:\Repos\ZLS\amChipper\src\amChipper.App\App.xaml`
- `E:\Repos\ZLS\amChipper\src\amChipper.App\ViewModels\MainViewModel.cs`
- `E:\Repos\ZLS\amChipper\src\amChipper.App\Services\WindowChromeTheme.cs`
- `E:\Repos\ZLS\amChipper\src\amChipper.App\Controls\*\*.cs`
- `E:\Repos\ZLS\amChipper\tests\amChipper.App.Tests\ThemeMigrationTests.cs`

Why:

The proof that CopperSkin is real is replacing the original embedded theme logic without breaking amChipper.

Impact/Compatibility:

This should happen only after CopperSkin packages are passing tests. Keep a local compatibility dictionary until every hard-coded surface is migrated.

Repair Track:

- Root cause: theme logic is embedded in `MainViewModel` and custom controls.
- Canonical owner: `CopperSkin.Core` and `CopperSkin.Wpf`.
- Minimal change: use CopperSkin to load the amChipper theme pack and apply selected theme names.
- Compatibility boundary: keep saved setting names and legacy resource keys.
- Verification: amChipper builds, settings load, and theme switching works.

Retirement Track:

- Retire `MainViewModel.ApplyTheme` dictionary switch after CopperSkin catalog is installed.
- Retire local `WindowChromeTheme` after `WpfWindowChromeService` is used.
- Retire hard-coded canvas brushes incrementally by adopting `DrawingThemeSnapshot`.
- Keep `DarkTheme.xaml` temporarily as a compatibility shim until all resources are package-provided.

Steps:

1. Write test:

```powershell
dotnet test E:\Repos\ZLS\amChipper\amChipper.sln -c Release -p:Platform=x64 --filter Theme
```

2. Verify RED:

Add migration tests that expect CopperSkin to resolve existing theme names. They fail until the package is referenced.

3. Minimal code:

Reference the local CopperSkin projects or packages, install CopperSkin at app startup, bind theme options from the CopperSkin catalog, and forward selected theme changes to `CopperSkinThemeManager`.

4. Verify GREEN:

```powershell
dotnet build E:\Repos\ZLS\amChipper\amChipper.sln -c Release -p:Platform=x64
dotnet test E:\Repos\ZLS\amChipper\amChipper.sln -c Release -p:Platform=x64
```

5. Commit:

```powershell
git -C E:\Repos\ZLS\amChipper add .
git -C E:\Repos\ZLS\amChipper commit -m "Migrate amChipper theme runtime to CopperSkin"
```

## Task 17 - Add visual verification and screenshots

Files:

- `tests/CopperSkin.Visual.Tests/ScreenshotHarness.cs`
- `tests/CopperSkin.Visual.Tests/KitchenSinkScreenshotTests.cs`
- `tests/CopperSkin.Visual.Tests/AmChipperCompatibilityVisualTests.cs`
- `docs/visual-verification.md`

Why:

Theme regressions are visual. Tests need to catch blank windows, unreadable text, missing resources, and broken layout.

Impact/Compatibility:

Use tolerances. Do not make fragile pixel-perfect tests for every gradient.

Steps:

1. Write test:

```powershell
dotnet test .\tests\CopperSkin.Visual.Tests\CopperSkin.Visual.Tests.csproj -c Release --filter Screenshot
```

2. Verify RED:

Tests fail until harness can launch windows and capture bitmaps.

3. Minimal code:

Implement screenshot capture on STA thread and simple assertions:

- Nonblank image.
- Minimum contrast samples.
- No missing-resource text.
- Theme changes alter expected regions.
- Main content fits at common desktop sizes.

4. Verify GREEN:

```powershell
dotnet test .\tests\CopperSkin.Visual.Tests\CopperSkin.Visual.Tests.csproj -c Release --filter Screenshot
```

5. Commit:

```powershell
git add .
git commit -m "Add CopperSkin visual verification"
```

## Task 18 - Write docs and first release checklist

Files:

- `README.md`
- `docs/getting-started.md`
- `docs/token-reference.md`
- `docs/theme-pack-format.md`
- `docs/amchipper-migration.md`
- `docs/designer-guide.md`
- `docs/release-checklist.md`

Why:

The engine should be usable by another WPF app without asking us how the magic works.

Impact/Compatibility:

Docs become part of the package contract.

Steps:

1. Write test:

```powershell
dotnet run --project .\src\CopperSkin.Cli\CopperSkin.Cli.csproj -- validate .\themes\amchipper
```

2. Verify RED:

Run README commands before writing docs; they should fail if referenced files or commands do not exist.

3. Minimal code:

Write docs with exact install, apply, theme pack, designer, CLI, migration, and troubleshooting steps.

4. Verify GREEN:

```powershell
dotnet build .\CopperSkin.slnx -c Release
dotnet test .\CopperSkin.slnx -c Release
dotnet run --project .\src\CopperSkin.Cli\CopperSkin.Cli.csproj -- validate .\themes\amchipper
dotnet pack .\src\CopperSkin.Core\CopperSkin.Core.csproj -c Release -o .\artifacts\nuget
dotnet pack .\src\CopperSkin.Wpf\CopperSkin.Wpf.csproj -c Release -o .\artifacts\nuget
```

5. Commit:

```powershell
git add .
git commit -m "Document CopperSkin theme engine"
```

## Risks

- WPF resource replacement can fail subtly when resources are frozen, app-level resources shadow window resources, or controls use `StaticResource` instead of `DynamicResource`.
- Visual tests can become brittle if they assert too many exact pixels.
- Custom drawing surfaces can leak if registration keeps strong references.
- Theme packs can become too flexible and hard to validate if the schema grows without discipline.
- amChipper can drift while CopperSkin is being built. Before migration, rerun the audit against the current checkout.
- If the first designer tries to become a full IDE, it can slow the engine. Keep designer v1 focused on theme editing, preview, diagnostics, and packaging.

## Rollback

- CopperSkin project creation is additive.
- amChipper migration should be a separate branch and can be rolled back by removing package references and restoring the existing `DarkTheme.xaml`, `ApplyTheme`, and `WindowChromeTheme` ownership.
- Keep the amChipper theme pack generated from source evidence so the visual baseline can be re-applied even if runtime code changes.

## Retirement

Retire these old owners only when replacement gates pass:

- `MainViewModel.ApplyTheme`: retire after CopperSkin catalog resolves all current theme names and applies all legacy keys.
- `WindowChromeTheme`: retire after `WpfWindowChromeService` applies caption, border, and text colors in amChipper.
- Monolithic `DarkTheme.xaml`: retire after CopperSkin base/control dictionaries cover all current resources.
- Hard-coded C# brush constants in custom canvases: retire per control after each implements `IThemeAwareDrawingSurface`.
- Duplicated theme menu items in `MainWindow.xaml`: retire after the menu binds to catalog-provided theme options.

## Completion Gates

CopperSkin v1 is complete when:

- A blank WPF app can install CopperSkin and switch themes at runtime.
- The kitchen-sink sample shows all standard controls styled.
- The designer edits and previews theme tokens live.
- The CLI validates, audits, packs, unpacks, and diffs theme packs.
- All 24 amChipper themes are available as first-party packs.
- amChipper can consume CopperSkin without losing current saved theme names.
- Custom drawing surfaces can use theme snapshots without hard-coded colors.
- `dotnet build`, `dotnet test`, CLI validation, and visual smoke tests pass.

## Execution Order

1. Initialize solution.
2. Build Core token model.
3. Import amChipper themes.
4. Resolve themes and aliases.
5. Validate themes.
6. Emit WPF resources.
7. Add runtime manager.
8. Port control styles.
9. Theme drawing surfaces.
10. Add native chrome.
11. Add kitchen sink.
12. Add designer.
13. Add CLI.
14. Add audits.
15. Package.
16. Migrate amChipper.
17. Add visual verification.
18. Document and release.

The plan is intentionally engine-first. The designer and amChipper migration come after the runtime is testable, because otherwise we risk rebuilding the same one-off skin logic in a prettier box.
