# CopperSkin Graphics Format

CopperSkin 0.3 introduces a renderer-neutral graphics document for icons and basic painting. The model lives in CopperSkin.Core.Graphics and is intentionally independent of WPF so it can be validated, signed, inspected, and serialized by Core and CLI tooling.

## Schema

The first public schema is version 1. A document contains an id, name, document type, logical canvas width and height, background color, and ordered layers. Layers contain ordered elements. Elements use one explicit discriminator: path, line, rectangle, ellipse, polygon, freehand, or text.

Colors are ARGB channel objects. A style may also name a CopperSkin theme token for runtime color resolution. Geometry uses document-space doubles; WPF renderers are responsible for device-pixel conversion and DPI handling.

Example:

~~~json
{
  "schemaVersion": 1,
  "id": "sample-icon",
  "name": "Sample Icon",
  "documentType": "Icon",
  "width": 24,
  "height": 24,
  "background": { "a": 0, "r": 0, "g": 0, "b": 0 },
  "layers": [
    {
      "id": "foreground",
      "name": "Foreground",
      "isVisible": true,
      "isLocked": false,
      "elements": [
        {
          "id": "body",
          "kind": "Ellipse",
          "geometry": { "bounds": { "x": 2, "y": 2, "width": 20, "height": 20 }, "points": [], "path": [] },
          "style": {
            "fill": { "a": 255, "r": 185, "g": 97, "b": 31 },
            "fillToken": "color.accent.primary",
            "stroke": { "a": 0, "r": 0, "g": 0, "b": 0 },
            "strokeWidth": 0,
            "opacity": 1
          },
          "transform": { "x": 0, "y": 0, "scaleX": 1, "scaleY": 1, "rotation": 0 }
        }
      ]
    }
  ]
}
~~~

Existing packs without a graphics property remain valid and do not gain an empty graphics section when written. When graphics are present, they participate in the deterministic theme-pack signature payload.

Readers reject a future schema version with a GraphicSchemaException. Unsupported element kinds, invalid dimensions, duplicate ids, malformed geometry, and invalid style ranges are reported by GraphicDocumentValidator without mutating the document.

The v0.3 scope intentionally excludes filters, gradients, animation, collaboration, and a full raster brush engine. SVG/XAML export is vector-oriented; PNG export is a WPF rendering adapter.
