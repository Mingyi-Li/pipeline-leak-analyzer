using Pipeline.Core.Models;

namespace Pipeline.Core.Services;

public static class LeakAnalyzer
{
    public static LeakAnalysisResult Analyze(
        IReadOnlyList<SensorReading> readings,
        double flowInLps,
        double flowOutLps,
        double flowLossThresholdRatio = 0.05, // 5% loss => strong signal
        double pressureWeight = 0.7,
        double flowWeight = 0.3)
    {
        if (readings.Count < 2)
            throw new ArgumentException("Need at least 2 sensor readings to form segments.");

        if (flowInLps <= 0)
            throw new ArgumentException("FlowIn must be > 0.");

        if (flowOutLps < 0)
            throw new ArgumentException("FlowOut must be >= 0.");

        // Flow loss ratio
        var rawLoss = (flowInLps - flowOutLps) / flowInLps;
        var flowLossRatio = Math.Max(0.0, rawLoss); // ignore negative "gain"
        var flowLossScore = Clamp01(flowLossRatio / Math.Max(1e-9, flowLossThresholdRatio));

        // Build segments and compute deltaP
        var deltas = new double[readings.Count - 1];
        double maxDelta = 0.0;

        for (int i = 0; i < readings.Count - 1; i++)
        {
            // Pressure drop from i -> i+1 (positive if dropping)
            var d = readings[i].PressureKPa - readings[i + 1].PressureKPa;
            deltas[i] = d;
            if (d > maxDelta) maxDelta = d;
        }

        // If maxDelta <= 0, there is no local drop anywhere
        // We still produce output, but pressure signal is weak.
        if (maxDelta <= 1e-9) maxDelta = 1e-9;

        var results = new List<SegmentResult>(readings.Count - 1);

        for (int i = 0; i < readings.Count - 1; i++)
        {
            var start = readings[i].PositionM;
            var end = readings[i + 1].PositionM;
            var deltaP = deltas[i];

            // Only positive drops contribute. Negative (pressure rises) => 0 pressure score.
            var pressureDropScore = Clamp01(Math.Max(0.0, deltaP) / maxDelta);

            var total = pressureWeight * pressureDropScore + flowWeight * flowLossScore;

            results.Add(new SegmentResult(
                SegmentIndex: i,
                StartM: start,
                EndM: end,
                DeltaPKPa: deltaP,
                PressureDropScore: pressureDropScore,
                FlowLossScore: flowLossScore,
                TotalScore: total
            ));
        }

        // Rank descending by total score
        results.Sort((a, b) => b.TotalScore.CompareTo(a.TotalScore));

        var best = results[0];
        var predicted = (best.StartM + best.EndM) / 2.0;

        var s1 = best.TotalScore;
        var s2 = results.Count > 1 ? results[1].TotalScore : 0.0;
        var confidence = Clamp01((s1 - s2) / Math.Max(1e-9, s1));

        return new LeakAnalysisResult(
            BestSegment: best,
            PredictedLeakLocationM: predicted,
            Confidence: confidence,
            RankedSegments: results,
            FlowLossRatio: flowLossRatio
        );
    }

    private static double Clamp01(double x) => x < 0 ? 0 : (x > 1 ? 1 : x);
}
