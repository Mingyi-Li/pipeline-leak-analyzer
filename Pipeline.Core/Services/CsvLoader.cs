using System.Globalization;
using Pipeline.Core.Models;

namespace Pipeline.Core.Services;

public static class CsvLoader
{
    // Expected CSV header: PositionM,PressureKPa
    public static List<SensorReading> LoadPositionPressureCsv(string csvText)
    {
        var readings = new List<SensorReading>();

        using var sr = new StringReader(csvText);
        string? line;
        bool firstLine = true;

        while ((line = sr.ReadLine()) is not null)
        {
            line = line.Trim();
            if (line.Length == 0) continue;

            if (firstLine)
            {
                firstLine = false;
                // allow header or no header
                if (line.StartsWith("Position", StringComparison.OrdinalIgnoreCase))
                    continue;
            }

            var parts = line.Split(',', StringSplitOptions.TrimEntries);
            if (parts.Length < 2) continue;

            if (!double.TryParse(parts[0], NumberStyles.Float, CultureInfo.InvariantCulture, out var pos))
                continue;
            if (!double.TryParse(parts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out var p))
                continue;

            readings.Add(new SensorReading(pos, p));
        }

        // Ensure sorted by position (important)
        readings.Sort((a, b) => a.PositionM.CompareTo(b.PositionM));
        return readings;
    }
}
