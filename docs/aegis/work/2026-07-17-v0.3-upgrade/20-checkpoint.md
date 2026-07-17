# Todo Checkpoint Draft

Current goal: begin v0.3.0.0 implementation while preserving the committed deployment baseline.

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
- [in_progress] Expand behavior-complete WPF control coverage and public package/release integration.

Completed todos: isolated worktree, baseline evidence, Core document model, validator, serializer, additive ThemePack graphics storage, signing participation, and graphics format documentation.

Active slice: editor-and-control-expansion.

Evidence refs: worktree branch feature/v0.3-upgrade at C:/Users/admin/.config/aegis/worktrees/CopperSkin/v0.3-upgrade; source main remains dirty but untouched; implementation commit 35645f5 pushed to origin/feature/v0.3-upgrade; Release build 0 warnings/0 errors; solution tests 15 Core, 5 CLI, 11 WPF per .NET 8/9/10, 1 Designer-history test per .NET 8/9/10, 2 visual per .NET 8/9/10.

Blocked on: no blocker yet.

Next step: close the next documented WPF control behavior gaps with interaction tests, then prepare package/version/CI changes for the public-release gate.

Resume state: read this checkpoint, re-read 10-intent.md and the v0.3 plan, compare git status, then continue only if the worktree and baseline agree.

Drift check: continue. Completed work remains inside the approved v0.3 Core/WPF/Designer/CLI ownership boundaries, does not alter the user's main worktree, and retains the release/scrollbar retirement contracts. Deferred work is still explicit: richer selection transforms, behavior-complete control waves, and public package/CI gates.
