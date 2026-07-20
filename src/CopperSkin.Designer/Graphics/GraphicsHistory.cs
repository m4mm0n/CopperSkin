using CopperSkin.Core.Graphics;

namespace CopperSkin.Designer.Graphics;

/// <summary>
/// Stores serialized document snapshots for deterministic graphics undo and redo.
/// </summary>
public sealed class GraphicsHistory
{
    private readonly List<string> _redo = new();
    private readonly List<string> _undo = new();

    /// <summary>
    /// Gets whether an undo operation is available.
    /// </summary>
    public bool CanUndo => _undo.Count > 0;

    /// <summary>
    /// Gets whether a redo operation is available.
    /// </summary>
    public bool CanRedo => _redo.Count > 0;

    /// <summary>
    /// Records a pre-edit snapshot and clears the redo branch.
    /// </summary>
    public void Record(GraphicDocument before)
    {
        if (before is null)
            throw new ArgumentNullException(nameof(before));

        _undo.Add(GraphicDocumentSerializer.Serialize(before));
        _redo.Clear();
    }

    /// <summary>
    /// Restores the most recent pre-edit snapshot and records the current document for redo.
    /// </summary>
    public GraphicDocument? Undo(GraphicDocument current)
    {
        if (!CanUndo)
            return null;

        _redo.Add(GraphicDocumentSerializer.Serialize(current));
        var snapshot = _undo[^1];
        _undo.RemoveAt(_undo.Count - 1);
        return GraphicDocumentSerializer.Deserialize(snapshot);
    }

    /// <summary>
    /// Restores the most recent redo snapshot and records the current document for undo.
    /// </summary>
    public GraphicDocument? Redo(GraphicDocument current)
    {
        if (!CanRedo)
            return null;

        _undo.Add(GraphicDocumentSerializer.Serialize(current));
        var snapshot = _redo[^1];
        _redo.RemoveAt(_redo.Count - 1);
        return GraphicDocumentSerializer.Deserialize(snapshot);
    }

    /// <summary>
    /// Removes all history snapshots, normally after opening or creating a document.
    /// </summary>
    public void Clear()
    {
        _undo.Clear();
        _redo.Clear();
    }
}
