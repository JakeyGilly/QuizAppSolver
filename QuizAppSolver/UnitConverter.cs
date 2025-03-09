using System.Text.RegularExpressions;
using System.Data;

namespace QuizAppSolver;

public class UnitConverter {
    public static double ConvertUnits(string input) {
        var match = Regex.Match(input, @"^-?(\d*\.?\d+)([munpMk]?)(\d*)$");
        if (!match.Success) {
            throw new ArgumentException("Invalid input format");
        }

        double baseValue = double.Parse(match.Groups[1].Value);
        string unit = match.Groups[2].Value;
        string subscript = match.Groups[3].Value;
        double multiplier = unit switch {
            "m" => 1e-3,
            "u" => 1e-6,
            "n" => 1e-9,
            "p" => 1e-12,
            "k" => 1e3,
            "M" => 1e6,
            _ => 1.0
        };

        double result = baseValue * multiplier;
        if (!string.IsNullOrEmpty(subscript)) {
            result += double.Parse(subscript) * Math.Pow(10, Math.Log10(multiplier) - 1);
        }
        if (input.StartsWith("-")) {
            result = -result;
        }
        return result;
    }

    public static string ConvertToUnit(double value) {
        string[] units = { "p", "n", "µ", "m", "", "k", "M" };
        double[] multipliers = { 1e-12, 1e-9, 1e-6, 1e-3, 1, 1e3, 1e6 };

        for (int i = multipliers.Length - 1; i >= 0; i--) {
            if (Math.Abs(value) < multipliers[i]) continue;
            double scaled = value / multipliers[i];
            string formatted = scaled.ToString("0.##");
            return formatted + units[i];
        }

        return value.ToString("0.##");
    }
}