namespace Pipeline.Core.Models;

public sealed record LeakAnalysisResult(
    SegmentResult BestSegment,
    double PredictedLeakLocationM,
    double Confidence,
    IReadOnlyList<SegmentResult> RankedSegments,
    double FlowLossRatio
);
