# Todo Checkpoint Draft

Current goal: finish v0.3.0.0 as the first public release while preserving the committed deployment baseline.

Current todo:

- [completed] Establish isolated worktree and checkpoint records.
- [completed] Capture clean baseline build and test evidence.
- [completed] Add Core graphics document contract and tests using RED/GREEN cycles.
- [completed] Review Core serialization integration boundary and update the graphics format documentation.
- [completed] Verify the Core slice and its full solution compatibility gates.
- [completed] Add the shared WPF graphics renderer and runtime CopperIcon surface.
- [completed] Add the Designer Graphics tab, presets, tools, save/open, zoom, and snapshot history.
- [completed] Add deterministic SVG/XAML/PNG export adapters and initial WPF primitive/media coverage.
- [completed] Expand layer/edit commands with add/remove/reorder/visibility/lock actions.
- [completed] Add headless CLI graphics validation, inspection, and canonical export commands.
- [completed] Expand the documented WPF control behavior wave, preserve forward horizontal scrolling, and add focus visuals.
- [completed] Complete public package metadata, license, governance docs, CI, release checklist, graphics documentation, and release visuals.
- [completed] Verify warning-free build, full tests, theme validation, package contents, and clean-checkout release inputs.

Completed todos: isolated worktree, baseline evidence, Core document model, validator, serializer, additive ThemePack graphics storage, signing participation, and graphics format documentation.

Active slice: public-release-verification.

Evidence refs: worktree branch feature/v0.3-upgrade at C:/Users/admin/.config/aegis/worktrees/CopperSkin/v0.3-upgrade; source main remains dirty but untouched; Release build 0 warnings/0 errors; solution tests 15 Core, 6 CLI, 14 WPF per .NET 8/9/10, 1 Designer test per .NET 8/9/10, and 2 visual tests per .NET 8/9/10; theme validation 24 themes with 0 errors/0 warnings; Core/WPF NuGet packages 0.3.0 created with README, LICENSE, target assets, and XML docs.

Blocked on: no blocker yet.

Next step: commit and push the verified public-release branch, then use the branch-finish workflow for merge/tag/publish decisions.

Resume state: read this checkpoint, re-read 10-intent.md and the v0.3 plan, compare git status, then continue only if the worktree and baseline agree.

Drift check: complete. Work remains inside the approved v0.3 Core/WPF/Designer/CLI ownership boundaries, does not alter the user's main worktree, and retains the release/scrollbar contracts. Deferred limitations remain explicit: resize handles, full text editing, raster brushes, and framework-owned hosted media behavior.
