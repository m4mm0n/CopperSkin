# Publishing CopperSkin to NuGet

CopperSkin publishes two packages:

- CopperSkin.Core — renderer-neutral theme and graphics primitives.
- CopperSkin.Wpf — WPF resources, controls, dialogs, chrome, and graphics rendering.

The project files currently produce package version 0.3.0 from assembly/file version 0.3.0.0.

## One-time NuGet setup

1. Create or sign in to the NuGet.org owner account for the packages.
2. Create an API key with push permission limited to CopperSkin.Core and CopperSkin.Wpf.
3. Set the source to https://api.nuget.org/v3/index.json.
4. In the GitHub repository settings, open **Secrets and variables → Actions**.
5. Add an Actions secret named NUGET_API_KEY.
6. Keep the key out of source control, local scripts, issue comments, and release notes.

The workflow in .github/workflows/ci.yml publishes only when a tag beginning with v0.3. is pushed. The verification job must pass first, and duplicate uploads are ignored.

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

Confirm that both packages are present, have version 0.3.0, and contain README.md, LICENSE, XML documentation, and the expected repository URL.

## Tag-driven publishing

After the release commit is on main:

~~~powershell
git switch main
git pull --ff-only origin main
git tag -a v0.3.0.0 -m "CopperSkin 0.3.0.0"
git push origin v0.3.0.0
~~~

GitHub Actions will then:

1. Check out the exact tag.
2. Restore, build, and test the solution.
3. Pack both NuGet packages.
4. Push both packages using NUGET_API_KEY.

Monitor the workflow under the repository's **Actions** tab. Once it is green, verify the package pages:

- https://www.nuget.org/packages/CopperSkin.Core/0.3.0
- https://www.nuget.org/packages/CopperSkin.Wpf/0.3.0

NuGet indexing can take a few minutes after a successful push.

## Manual publishing fallback

Use this only when the GitHub Actions publish job is unavailable:

~~~powershell
$packages = Get-ChildItem .\artifacts\packages\*.nupkg
foreach ($package in $packages) {
    dotnet nuget push $package.FullName --api-key $env:NUGET_API_KEY --source https://api.nuget.org/v3/index.json --skip-duplicate
}
~~~

Set NUGET_API_KEY only in the current process or an approved secret store. Do not put the value directly in the command history or a committed script.

## Updating a future release

For a new version, update Directory.Build.props, the package project files, CHANGELOG.md, and RELEASE_NOTES.md together. Run the full clean-checkout validation, create a matching vX.Y.Z.0 tag, and use a new NuGet package version. NuGet package versions are normalized to three-part SemVer values even when the assembly version has four parts.
