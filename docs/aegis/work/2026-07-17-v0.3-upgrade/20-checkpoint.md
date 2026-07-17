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
- [in_progress] Expand layer/edit commands and behavior-complete WPF control coverage.

Completed todos: isolated worktree, baseline evidence, Core document model, validator, serializer, additive ThemePack graphics storage, signing participation, and graphics format documentation.

Active slice: editor-and-control-expansion.

Evidence refs: worktree branch feature/v0.3-upgrade at C:/Users/admin/.config/aegis/worktrees/CopperSkin/v0.3-upgrade; source main remains dirty but untouched; implementation commit 35645f5 pushed to origin/feature/v0.3-upgrade; Release build 0 warnings/0 errors; solution tests 15 Core, 5 CLI, 11 WPF per .NET 8/9/10, 1 Designer-history test per .NET 8/9/10, 2 visual per .NET 8/9/10.

Blocked on: no blocker yet.

Next step: add explicit layer selection/reordering/visibility/lock commands and close the next documented WPF control behavior gaps with interaction tests.

Resume state: read this checkpoint, re-read 10-intent.md and the v0.3 plan, compare git status, then continue only if the worktree and baseline agree.

Drift check: continue. Completed work remains inside the approved v0.3 Core/WPF/Designer ownership boundaries, does not alter the user's main worktree, and retains the release/scrollbar retirement contracts. Deferred work is still explicit: full layer commands, richer editing, CLI graphics commands, and behavior-complete control waves.
