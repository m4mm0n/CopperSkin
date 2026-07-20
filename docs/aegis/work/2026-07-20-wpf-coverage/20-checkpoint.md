# Todo Checkpoint Draft

- [completed] Audit existing partial-control styles and identify minimum behavior gaps.
- [completed] Add shared validation/focus resources and targeted control-state setters/templates.
- [completed] Add interaction, RTL, theme-switch, and accessibility regression tests.
- [completed] Update coverage/accessibility documentation and verify the full local matrix.

Active slice: partial-control behavior completion.
Evidence refs: release `main` at `f8362f1`; v0.3 CI previously passed; local `main` is dirty and untouched; isolated worktree `zls/complete-wpf-coverage` starts from remote main.
Blocked on: none.
Next: commit, push, run clean-checkout CI, and merge the bounded WPF coverage slice.
Drift check: continue. Scope remains limited to WPF resource behavior, tests, and coverage docs; Core, graphics, package contracts, and user main remain unchanged.
