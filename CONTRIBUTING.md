# Contributing to CopperSkin

CopperSkin is a Windows/WPF library with a renderer-neutral Core. Contributions should preserve that boundary: Core must remain free of WPF references, while WPF-specific rendering and resource dictionaries belong in `CopperSkin.Wpf`.

Before opening a pull request:

1. Restore, build, test, and pack from a clean checkout.
2. Add or update tests for behavior changes, including keyboard/focus behavior for controls and deterministic output for serialization/export changes.
3. Update the relevant format, coverage, or release documentation.
4. Run `git diff --check` and confirm the repository contains no generated artifacts or private keys.

Use focused commits and explain compatibility impact. Do not commit signing keys, generated packages, user settings, or machine-specific paths.
