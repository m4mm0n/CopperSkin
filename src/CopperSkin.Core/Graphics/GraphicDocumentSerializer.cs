using System.Text.Json;

namespace CopperSkin.Core.Graphics;

/// <summary>
/// Serializes and deserializes the versioned graphics-document JSON contract.
/// </summary>
public static class GraphicDocumentSerializer
{
    private static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true
    };

    /// <summary>
    /// Serializes a graphics document with deterministic formatting.
    /// </summary>
    public static string Serialize(GraphicDocument document)
    {
        if (document is null)
            throw new ArgumentNullException(nameof(document));
        return JsonSerializer.Serialize(document, Options);
    }

    /// <summary>
    /// Deserializes a graphics document after checking its schema version.
    /// </summary>
    public static GraphicDocument Deserialize(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
            throw new ArgumentException("Graphics JSON is required.", nameof(json));

        using var parsed = JsonDocument.Parse(json);
        if (parsed.RootElement.ValueKind != JsonValueKind.Object)
            throw new JsonException("Graphics JSON must contain an object.");

        var version = ReadSchemaVersion(parsed.RootElement);
        if (version > GraphicDocument.CurrentSchemaVersion)
            throw new GraphicSchemaException(version);

        var document = JsonSerializer.Deserialize<GraphicDocument>(json, Options);
        return document ?? throw new JsonException("Graphics JSON did not contain a document.");
    }

    private static int ReadSchemaVersion(JsonElement root)
    {
        if (root.TryGetProperty("schemaVersion", out var lower) && lower.TryGetInt32(out var lowerValue))
            return lowerValue;
        if (root.TryGetProperty("SchemaVersion", out var upper) && upper.TryGetInt32(out var upperValue))
            return upperValue;
        return GraphicDocument.CurrentSchemaVersion;
    }
}

/// <summary>
/// Reports that a document requires a graphics schema newer than this library supports.
/// </summary>
public sealed class GraphicSchemaException : Exception
{
    /// <summary>
    /// Initializes a schema-version exception.
    /// </summary>
    public GraphicSchemaException(int schemaVersion)
        : base($"Graphics schema version {schemaVersion} is newer than supported version {GraphicDocument.CurrentSchemaVersion}.")
    {
        SchemaVersion = schemaVersion;
    }

    /// <summary>
    /// Gets the unsupported schema version.
    /// </summary>
    public int SchemaVersion { get; }
}
