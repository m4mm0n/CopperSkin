# Publishing CopperSkin to NuGet

CopperSkin publishes two packages:

- CopperSkin.Core — renderer-neutral theme and graphics primitives.
- CopperSkin.Wpf — WPF resources, controls, dialogs, chrome, and graphics rendering.

The project files produce package version 0.3.1 from assembly/file version 0.3.0.0. The patch version exists because NuGet package versions are immutable; it carries the corrected package README without changing the assembly compatibility version.

## Trusted Publishing setup

CopperSkin uses NuGet Trusted Publishing from GitHub Actions. This avoids storing a long-lived NuGet API key in GitHub.

The NuGet policies for both packages should use:

- NuGet package owner: m4mm0n
- Repository owner: m4mm0n
- Repository: CopperSkin
- Workflow file: ci.yml
- Environment: leave empty

NuGet expects the workflow filename only, not the .github/workflows/ path. The policy is initially active for seven days; a successful login or publish permanently activates it.

The workflow requests GitHub's OIDC token, exchanges it with NuGet using NuGet/login@v1, and receives a short-lived package credential. No NUGET_API_KEY repository secret is required.

## Verify locally before tagging

Run this from a clean Windows checkout:

~~~powershell
dotnet restore .\CopperSkin.slnx
dotnet build .\CopperSkin.slnx --configuration Release --no-restore
dotnet test .\CopperSkin.slnx --configuration Release --no-build

Remove-Item .\artifacts\packages -Recurse -Force -ErrorAction SilentlyContinue
dotnet pack .\src\CopperSkin.Core\CopperSkin.Core.csproj --configuration Release --output .\artifacts\packages
dotnet pack .\src\CopperSkin.Wpf\CopperSkin.Wpf.csproj --configuration Release --output .\artifacts\packages

Get-ChildItem .\artifacts\packages\*.nupkg
~~~

Confirm that both packages are present, have version 0.3.1, and contain the package-specific README.md, LICENSE, XML documentation, and the expected repository URL.

## Tag-driven publishing

After the release commit is on main:

~~~powershell
git switch main
git pull --ff-only origin main
git tag -a v0.3.1.0 -m "CopperSkin 0.3.1.0"
git push origin v0.3.1.0
~~~

GitHub Actions will then:

1. Check out the exact tag.
2. Restore, build, and test the solution.
3. Pack both NuGet packages.
4. Request a short-lived NuGet credential through Trusted Publishing.
5. Push both packages to NuGet.

Monitor the workflow under the repository's **Actions** tab. Once it is green, verify:

- https://www.nuget.org/packages/CopperSkin.Core/0.3.1
- https://www.nuget.org/packages/CopperSkin.Wpf/0.3.1

NuGet indexing can take a few minutes after a successful push.

## If the seven-day policy window expires

Open NuGet.org → your profile → **Trusted Publishing**, edit the policy, and choose **Activate for 7 days** again. Then rerun the tag workflow. A successful OIDC login is enough to permanently activate the policy.

## Manual fallback

If Trusted Publishing is unavailable, use a short-lived, package-scoped NuGet API key only from a secure local environment:

~~~powershell
$packages = Get-ChildItem .\artifacts\packages\*.nupkg
foreach ($package in $packages) {
    dotnet nuget push $package.FullName --api-key $env:NUGET_API_KEY --source https://api.nuget.org/v3/index.json --skip-duplicate
}
~~~

Never commit the key or put it in a workflow file. Trusted Publishing remains the preferred release path.

## Updating a future release

For a new version, update Directory.Build.props, the package project files, CHANGELOG.md, and RELEASE_NOTES.md together. Run the full clean-checkout validation, create a matching vX.Y.Z.0 tag, and use a new NuGet package version. NuGet package versions are normalized to three-part SemVer values even when the assembly version has four parts.
