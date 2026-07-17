using CopperSkin.Core.Graphics;
using CopperSkin.Designer.Graphics;

namespace CopperSkin.Designer.Tests;

/// <summary>
/// Verifies deterministic Designer graphics history behavior.
/// </summary>
public sealed class GraphicsEditorTests
{
    /// <summary>Verifies undo and redo restore serialized document snapshots.</summary>
    [Fact]
    public void HistoryRestoresUndoAndRedoSnapshots()
    {
        var document = new GraphicDocument
        {
            Id = "history",
            Name = "History",
            Width = 64,
            Height = 64,
            Layers = [new GraphicLayer { Id = "layer", Name = "Layer" }]
        };
        var history = new GraphicsHistory();
        history.Record(document);
        document.Layers[0].Elements.Add(new GraphicElement { Id = "rectangle", Kind = GraphicElementKind.Rectangle, Geometry = new GraphicGeometry { Bounds = new GraphicRect(1, 1, 10, 10) } });

        var undone = history.Undo(document);

        Assert.NotNull(undone);
        Assert.Empty(undone!.Layers[0].Elements);
        Assert.True(history.CanRedo);

        var redone = history.Redo(undone);

        Assert.NotNull(redone);
        Assert.Single(redone!.Layers[0].Elements);
    }
}
