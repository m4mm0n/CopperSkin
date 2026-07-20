# v0.3.0.0 release checklist

1. Build from a clean checkout with SDK `10.0.301` or a compatible latest feature release.
2. Run restore, Release build, the complete test matrix, and Core/WPF package creation.
3. Confirm warnings are treated as errors and `git diff --check` is clean.
4. Validate an old signed theme pack and a v0.3 graphics document.
5. Inspect both packages for `README.md`, `LICENSE`, XML docs, correct version, and repository metadata.
6. Review the [control coverage](CONTROL_COVERAGE.md), [graphics format](GRAPHICS_FORMAT.md), and [accessibility notes](ACCESSIBILITY.md).
7. Create a protected `v0.3.x` tag only after the branch is merged.
8. Let GitHub Actions publish packages using the protected `NUGET_API_KEY` secret. Never store signing keys or API keys in the repository.

The workflow publishes only tags beginning with `v0.3.` and uses `--skip-duplicate`, so a rerun is safe after a transient failure.
