# Initial amChipper Skin Baseline

Date: 2026-05-25

Source checkout: `E:\Repos\ZLS\amChipper`

## Observed Skin Surface

- `src\amChipper.App\App.xaml` merges `Theme/DarkTheme.xaml` at app startup.
- `src\amChipper.App\Theme\DarkTheme.xaml` owns the current core WPF resources and styles.
- `src\amChipper.App\Services\WindowChromeTheme.cs` applies active WPF resources to native DWM title bar colors.
- `src\amChipper.App\ViewModels\MainViewModel.cs` owns the runtime theme catalog, normalization, application, and persisted settings.
- `src\amChipper.App\MainWindow.xaml` exposes 24 theme choices in the View menu and settings UI.
- Several custom drawing controls still use hard-coded brushes or colors in C# rendering code.

## Current Theme Keys

Core semantic keys observed in active XAML:

- `BgDeep`
- `BgPanel`
- `BgControl`
- `BgHover`
- `BgSelect`
- `Accent`
- `AccentLight`
- `Border`
- `TextPrimary`
- `TextSecondary`
- `TextDisabled`
- `ButtonShine`
- `PanelSheen`
- `SoftGlow`
- `PanelShadow`
- `NeonGlow`
- `FlStudioChrome`
- `FlStudioPanel`
- `FlStudioStrip`
- `FlStudioActive`

The dictionary also defines color keys such as `BgDeepColor`, `AccentColor`, `PlayheadColor`, `GridLineColor`, and `GridBarColor`.

## Existing Theme Names

`MainViewModel.ThemeOptions` currently exposes:

- `FL Grape`
- `Neon Studio`
- `Classic Tracker`
- `Amber CRT`
- `Midnight Pro`
- `Ice Matrix`
- `Magenta Circuit`
- `Carbon Lime`
- `Ruby Wave`
- `Ocean Lab`
- `Steel Mono`
- `Sunset Pop`
- `Cyber Cyan`
- `Toxic Acid`
- `Royal Neon`
- `Graphite Gold`
- `Circuit Blue`
- `Laser Orange`
- `Vapor Glass`
- `Terminal Green`
- `Hotline Pink`
- `Arctic Steel`
- `Copper Desk`
- `Deep Red`

## Current Strengths

- The look is already cohesive and distinctive: dense workstation chrome, glossy buttons, gradient panels, neon accent states, dark tracker surfaces, and custom editor surfaces.
- Many UI surfaces already consume `DynamicResource`, which makes runtime switching possible.
- Native window chrome is already tied to theme resources through `WindowChromeTheme`.
- User preferences already include theme, UI shine, panel shadows, density, scaling, animation mode, transparent windows, high visibility, small scrollbars, and color map options.

## Current Gaps

- Theme ownership is embedded in `MainViewModel` instead of a reusable engine.
- Theme definitions are C# dictionaries rather than external, schema-validated theme packs.
- The token set is too small for a full WPF engine. It lacks component tokens, drawing tokens, typography tokens, density metrics, motion tokens, chrome tokens, and accessibility metadata.
- `DarkTheme.xaml` is monolithic and not separated into token dictionaries, control styles, effects, templates, and app-specific styles.
- Custom rendered controls hard-code many brush and color values. A scan found the largest hard-coded color surfaces in `MainViewModel.cs`, `DarkTheme.xaml`, `BootstrapWindow.xaml`, `MainWindow.xaml`, `NoteGridCanvas.cs`, `TimelineCanvas.cs`, `PatternGridCanvas.cs`, `TrackHeaderPanel.cs`, and `PianoKeysCanvas.cs`.
- Menu theme choices are duplicated in XAML rather than bound from a catalog.
- There is no visual test harness, theme pack validator, CLI, sample gallery, designer, or NuGet packaging boundary.
