# Task Intent Draft

Requested outcome: begin executing the v0.3.0.0 first-public-release plan for CopperSkin.

Scope for this execution slice:

- Establish the public graphics contract in Core.
- Implement the first renderer-neutral graphics document types and validation/serialization tests.
- Preserve the existing WPF scrollbar fix and release baseline.
- Leave the user's existing uncommitted edits in the original main worktree untouched.

Non-goals for this slice:

- Do not implement the complete Designer UI yet.
- Do not split or replace WPF resource dictionaries yet.
- Do not change package versioning, licensing, CI, or release credentials until the Core contract is verified.

Baseline refs acknowledged:

- docs/aegis/plans/2026-07-16-v0.3.0-public-release.md
- docs/aegis/plans/2026-07-16-library-deployment.md
- Directory.Build.props
- src/CopperSkin.Core/CopperSkin.Core.csproj
- src/CopperSkin.Wpf/Drawing/IThemeAwareDrawingSurface.cs
- docs/THEME_FORMAT.md
- docs/CONTROL_COVERAGE.md

Baseline usage:

~~~powershell
git status --short --branch
dotnet build .\CopperSkin.slnx --configuration Release --no-restore
dotnet test .\CopperSkin.slnx --configuration Release --no-build
~~~

Risk hints: the Core project targets netstandard2.0 and newer frameworks; the new model must remain WPF-free, deterministic, and compatible with existing theme-pack serialization. A public schema mistake is more expensive than a small first slice, so the schema will be kept explicit and tested before adding renderers.

Stop conditions: stop and report if the clean worktree baseline fails, if the existing serializer contract cannot accommodate an additive graphics section without a migration decision, or if the model requires WPF dependencies.
