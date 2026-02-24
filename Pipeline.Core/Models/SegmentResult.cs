namespace Pipeline.Core.Models;

public sealed record SegmentResult(
    int SegmentIndex,
    double StartM,
    double EndM,
    double DeltaPKPa,
    double PressureDropScore,
    double FlowLossScore,
    double TotalScore
);
