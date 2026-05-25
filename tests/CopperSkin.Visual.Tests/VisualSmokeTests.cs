using CopperSkin.SampleKitchenSink;

namespace CopperSkin.Visual.Tests;

public sealed class VisualSmokeTests
{
    [Fact]
    public void SampleRowsAreAvailable()
    {
        var row = new ControlRow("Window", "DWM aware");

        Assert.Equal("Window", row.Name);
        Assert.Equal("DWM aware", row.State);
    }
}
