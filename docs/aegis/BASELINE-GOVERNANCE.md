# Baseline Governance

CopperSkin plans must be grounded in source evidence from the current checkout and must preserve compatibility boundaries explicitly.

## Rules

- Treat `E:\Repos\ZLS\amChipper` as the seed implementation until the first CopperSkin packages exist.
- Keep the existing amChipper resource keys working during migration: `BgDeep`, `BgPanel`, `BgControl`, `BgHover`, `BgSelect`, `Accent`, `AccentLight`, `Border`, `TextPrimary`, `TextSecondary`, `TextDisabled`, `ButtonShine`, `PanelSheen`, `SoftGlow`, `PanelShadow`, `NeonGlow`, `FlStudioChrome`, `FlStudioPanel`, `FlStudioStrip`, and `FlStudioActive`.
- New CopperSkin APIs must be typed and testable. XAML-only magic is acceptable only as package output, not as the core owner.
- Runtime theme switching must be reversible, per-window capable, and safe for existing `DynamicResource` consumers.
- Every implementation slice must have automated verification or a documented manual visual gate.
