# CopperSkin Theme Format

CopperSkin themes are token packs. The JSON format is intentionally simple so it can be edited by tools, humans, and build pipelines.

## Theme Pack

```json
{
  "name": "CopperSkin Built-ins",
  "version": "0.2.0.0",
  "metadata": {
    "author": "ZeroLinez Softworx",
    "compatibility.copperskin": "0.2.0.0",
    "signature.algorithm": "ECDSA-GF2M-B233-SHA256",
    "signature.curve": "NIST-B-233",
    "signature.signer": "CopperSkin",
    "signature.createdUtc": "2026-07-03T00:00:00.0000000Z",
    "signature.eccgf2.publicKey": "04...",
    "signature.eccgf2.signature": "..."
  },
  "themes": [
    {
      "name": "Neon Studio",
      "tokens": {
        "color.surface.deep": "#FF171221",
        "color.surface.panel": "#FF241B32",
        "color.accent.primary": "#FFB961FF",
        "color.text.primary": "#FFF9F2FF"
      }
    }
  ]
}
```

Color tokens use `#AARRGGBB`. The validator reports missing required tokens, malformed color values, unknown tokens, and low contrast warnings.

Pack and theme metadata are string dictionaries. The signer writes `signature.algorithm`, `signature.curve`, `signature.signer`, `signature.createdUtc`, `signature.eccgf2.publicKey`, and `signature.eccgf2.signature`. Only `signature.eccgf2.signature` is excluded from the signed payload, so changing signed metadata, tokens, theme names, or the embedded public key invalidates verification. For authenticity, verify with a trusted public key file instead of only trusting the embedded public key.

## Core Token Families

- `color.surface.*`: app, panel, control, hover, and selected backgrounds
- `color.accent.*`: primary and light accent colors
- `color.border.default`: shared chrome/control border
- `color.text.*`: primary, secondary, and disabled text
- `color.status.*`: success, warning, danger
- `color.action.*` and `color.focus.ring`: button states and keyboard focus
- `color.editor.*`: tracker, piano roll, clip, and automation drawing colors
- `spacing.*`: spacing units emitted as numeric and `Thickness` resources
- `metric.radius.*`: corner radii emitted as numeric and `CornerRadius` resources
- `metric.border.*`, `metric.scrollbar.size`, `metric.density.scale`: density and chrome sizing
- `font.*`: UI and mono font families, sizes, and weights
- `motion.*.ms`: transition durations emitted as WPF `Duration` resources
- `effect.*.opacity`: shine, shadow, and glow intensity
- `icon.*`: symbolic icon keys for themed dialog surfaces
- `adapter.*`: integration preferences for common WPF control libraries

Run `CopperSkin.Cli tokens` to print the canonical token catalog with type, default value, and required/recommended status.

## Validation And Tooling

The CLI can validate packs, create starter themes, sign packs, verify signatures, compare token differences, produce static galleries, and write deterministic visual baselines:

```powershell
CopperSkin.Cli validate .\themes\amchipper\theme-pack.json
CopperSkin.Cli tokens
CopperSkin.Cli scaffold .\artifacts\starter-theme
CopperSkin.Cli keygen .\artifacts\signing.private .\artifacts\signing.public
CopperSkin.Cli sign .\themes\amchipper\theme-pack.json .\artifacts\signed-theme-pack.json .\artifacts\signing.private
CopperSkin.Cli verify-signature .\artifacts\signed-theme-pack.json .\artifacts\signing.public
CopperSkin.Cli gallery .\themes\amchipper\theme-pack.json .\artifacts\gallery
CopperSkin.Cli baseline .\themes\amchipper\theme-pack.json .\artifacts\visual-baseline.json
CopperSkin.Cli diff .\themes\amchipper\theme-pack.json .\themes\custom\theme-pack.json
CopperSkin.Cli migrate .\src .\artifacts\migration.md
CopperSkin.Cli adapters
```

Gallery output is plain HTML plus a `visual-baseline.json` manifest. Baselines are deterministic JSON snapshots of resolved theme tokens and pack hashes for visual regression workflows.

## Archives

`.cskin` files are zip-based CopperSkin theme packs. They preserve directory structure and can be created or unpacked with the CLI.

`.lzhc` files are CopperSkin release archives created by the CLI. The current format writes a `CSLZHC1` header followed by Brotli-compressed JSON file entries. It is meant for reproducible release packaging and can be replaced later by a lower-level LZ/Huffman codec without changing the release workflow.
