# Evidence Bundle Draft

This record will contain fresh evidence for the isolated v0.3 Core slice.

Required evidence:

- clean worktree status and branch;
- Release build output;
- existing solution test output;
- Core graphics RED evidence;
- Core graphics GREEN evidence;
- format documentation readback;
- final diff and status.

Current evidence:

- Worktree created from commit 6a052e8 on branch feature/v0.3-upgrade.
- Original main worktree was observed dirty and is not being modified by this slice.
- RED: GraphicsDocumentTests initially failed to compile because CopperSkin.Core.Graphics did not exist.
- GREEN: Core graphics tests pass: 15 passed, 0 failed.
- Compatibility fix: replaced ArgumentNullException.ThrowIfNull with netstandard2.0-compatible explicit guards.
- Full Release build: 0 warnings, 0 errors across the solution target frameworks.
- Full solution tests: 15 Core, 5 CLI, 6 WPF on each of .NET 8/9/10, and 2 visual on each of .NET 8/9/10; all passed.
- Added docs/GRAPHICS_FORMAT.md with schema version, element kinds, compatibility, and exclusions.
- WPF renderer/hit testing: 8 tests passed per .NET 8/9/10 before export coverage.
- WPF export/control wave: 11 tests passed per .NET 8/9/10, including SVG/XAML determinism, PNG dimensions, and four new primitive/media resource styles.
- Designer history: 1 test passed per .NET 8/9/10.
- Full solution verification after export wiring: build 0 warnings/0 errors; 15 Core, 5 CLI, 11 WPF per .NET 8/9/10, 1 Designer per .NET 8/9/10, and 2 visual per .NET 8/9/10 all passed.
- Commit 35645f5 Add v0.3 graphics editor foundation was pushed to origin/feature/v0.3-upgrade.
- Layer command slice verified by the full solution gate: add/remove/reorder/visibility/lock UI compiles and Designer history suites remain green.
- CLI graphics slice: 6 CLI tests passed; validate, inspect, and canonical JSON export are covered.
- Full solution verification after CLI wiring: build 0 warnings/0 errors; 15 Core, 6 CLI, 11 WPF per .NET 8/9/10, 1 Designer per .NET 8/9/10, and 2 visual per .NET 8/9/10 all passed.
