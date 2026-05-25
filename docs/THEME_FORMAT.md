# CopperSkin Theme Format

CopperSkin themes are token packs. The JSON format is intentionally simple so it can be edited by tools, humans, and build pipelines.

## Theme Pack

```json
{
  "name": "CopperSkin Built-ins",
  "version": "0.1.0",
  "themes": [
    {
      "name": "Neon Tape",
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

## Core Token Families

- `color.surface.*`: app, panel, control, hover, and selected backgrounds
- `color.accent.*`: primary and light accent colors
- `color.border.default`: shared chrome/control border
- `color.text.*`: primary, secondary, and disabled text
- `color.status.*`: success, warning, danger
- `effect.glow.*`: glow color and opacity
- `drawing.*`: tracker/grid/waveform/custom drawing colors

## Archives

`.cskin` files are zip-based CopperSkin theme packs. They preserve directory structure and can be created or unpacked with the CLI.

`.lzhc` files are CopperSkin release archives created by the CLI. The current format writes a `CSLZHC1` header followed by Brotli-compressed JSON file entries. It is meant for reproducible release packaging and can be replaced later by a lower-level LZ/Huffman codec without changing the release workflow.

