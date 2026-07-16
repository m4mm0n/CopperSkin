# Goal

Make CopperSkin publishable as a dependable WPF library release: reproducible builds, complete test gates, documented package usage, protected signing, and an auditable release artifact.

# Architecture

`CopperSkin.Core` owns typed theme data, validation, serialization, archives, and signing. `CopperSkin.Wpf` owns resource emission, styles, controls, chrome, and scoped runtime themes. `CopperSkin.Cli` owns authoring and release operations. The Designer and Kitchen Sink are consumers and visual references, not alternate runtime owners.

# Tech Stack

- C# and .NET SDK 10.0.301
- WPF targets: `net7.0-windows`, `net8.0-windows`, `net9.0-windows`, `net10.0-windows`
- Core targets: `netstandard2.0`, `net7.0`, `net8.0`, `net9.0`, `net10.0`
- xUnit, Microsoft.NET.Test.Sdk, coverlet collector
- NuGet packaging, `.cskin` theme packs, `.lzhc` release archives

# Baseline/Authority Refs

- `CopperSkin.slnx`
- `Directory.Build.props`
- `README.md`
- `RELEASE_NOTES.md`
- `docs/THEME_FORMAT.md`
- `docs/CONTROL_COVERAGE.md`

Baseline usage: the current source and test projects are the release authority; no CI workflow, Git remote, license file, or package-publishing credential configuration is present in this workspace.

# Compatibility Boundary

The release must preserve the existing public namespaces and the current target-framework matrix. Horizontal scrollbar behavior must remain correct for both `FlowDirection.LeftToRight` and existing vertical-scroll usage. Existing signed theme-pack metadata remains readable; any signing-format change requires an explicit migration and compatibility test.

# Verification

The implementation baseline is verified with:

```powershell
dotnet build .\CopperSkin.slnx --configuration Release --no-restore
dotnet test .\CopperSkin.slnx --configuration Release --no-build
```

The release plan is complete only when the same commands run in clean CI, package validation passes, and a signed pack can be verified with a trusted public key.

## Requirement Ready Check

- Requirement source: repository request to fix defects, document deployment readiness, and prepare public repository documentation.
- Acceptance: zero build warnings/errors, passing unit/WPF/visual tests, corrected horizontal scroll direction, README examples/assets, and an actionable deployment plan.
- Open release decisions: Git hosting remote, package feed, supported release branch, signing-key custody, and public license-file policy.
- Decision: ready for implementation and documentation; release publication remains an external decision.

## Current evidence

- Release build: 0 warnings, 0 errors across the solution target frameworks.
- Core tests: 9 passed.
- CLI tests: 5 passed.
- WPF tests: 6 passed on each of .NET 8, 9, and 10.
- Visual smoke tests: 2 passed on each of .NET 8, 9, and 10.
- Horizontal scrollbar regression: reproduced before the template change and passing after it on .NET 8, 9, and 10.

## Deployment workstream

### 1. Repository and governance

- Initialize or connect the Git repository and protect the release branch.
- Add `LICENSE`, `CONTRIBUTING.md`, `SECURITY.md`, and a release issue template.
- Add a CI workflow that runs restore, build, all tests, package validation, and the CLI audit on Windows.
- Add a pinned SDK policy (`global.json`) once the supported SDK line is selected.

### 2. Quality gates

- Change `TreatWarningsAsErrors` to `true` in CI first, then make it the repository default after third-party warning compatibility is confirmed.
- Keep the target matrix explicit and test both the lowest supported and current SDK/runtime combinations.
- Add WPF visual baselines for horizontal/vertical scrolling, DPI scaling, keyboard navigation, RTL layout, and theme switching.
- Add coverage thresholds for Core and CLI paths that affect pack validation and signing.

### 3. Packaging and versioning

- Centralize package version, informational version, repository URL, tags, license expression, and icon/readme metadata.
- Produce `CopperSkin.Core` and `CopperSkin.Wpf` packages with symbols and source link data.
- Validate package contents, dependency groups, XML docs, README rendering, and the `netstandard2.0` compatibility contract.
- Generate a release manifest containing package hashes, archive hashes, target frameworks, and the exact commit.

### 4. Signing and supply-chain controls

- Keep private signing keys outside the repository and CI logs; use a protected secret or external signing service.
- Verify the public key and signature in CI before publishing a theme pack.
- Add tamper and wrong-key tests to the release gate.
- Never publish `artifacts/`, `Ready2Release/`, or generated secrets unless a release job explicitly produces them.

### 5. Documentation and release operations

- Keep the root README as the short public entry point and link to `docs/THEME_FORMAT.md`, control coverage, and this plan.
- Publish a versioned changelog and release notes with compatibility and upgrade notes.
- Add a clean-room release rehearsal from a fresh checkout before the first public tag.
- Publish packages and archives only after CI, signature verification, and manual sample smoke checks pass.

## Risks and rollback

- WPF behavior can vary by Windows theme, DPI, and input device; retain visual smoke coverage and a manual sample check.
- Signing is a public compatibility surface; do not rotate algorithms or metadata keys without a migration path.
- The workspace currently has no `.git` directory or remote, so upload/push cannot be verified here. Once a remote is supplied, push only after the clean-room release rehearsal.

## Retirement track

- Retired: the all-orientations `IsDirectionReversed="True"` behavior in the shared scrollbar template.
- Retained: existing vertical template behavior, public signing metadata names, and the current target-framework matrix.
- Trigger for future cleanup: remove legacy aliases or release-internal artifacts only after a compatibility scan and an explicit versioned migration decision.
