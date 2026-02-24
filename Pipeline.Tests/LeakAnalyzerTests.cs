using Pipeline.Core.Models;
using Pipeline.Core.Services;

namespace Pipeline.Tests;

public class LeakAnalyzerTests
{
    [Fact]
    public void Analyze_PicksSegmentWithLargestPressureDrop()
    {
        var readings = new List<SensorReading>
        {
            new(0, 520),
            new(100, 519),
            new(200, 490), // big drop happens 100-200
            new(300, 489),
        };

        var result = LeakAnalyzer.Analyze(readings, flowInLps: 200, flowOutLps: 198);

        Assert.Equal(100, result.BestSegment.StartM);
        Assert.Equal(200, result.BestSegment.EndM);
        Assert.True(result.Confidence >= 0);
    }

    [Fact]
    public void Analyze_FlowLossNeverNegative()
    {
        var readings = new List<SensorReading>
        {
            new(0, 520),
            new(100, 519),
        };

        var result = LeakAnalyzer.Analyze(readings, flowInLps: 200, flowOutLps: 210);
        Assert.Equal(0.0, result.FlowLossRatio);
    }
}
