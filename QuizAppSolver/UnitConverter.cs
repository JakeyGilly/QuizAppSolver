using System.Text.RegularExpressions;
using System.Numerics;
using System;

namespace QuizAppSolver;

public class UnitConverter {
    public static Complex ConvertUnits(string input) {
        var polarMatch = Regex.Match(input, @"^(-?\d*\.?\d+)([munpMk]?)?(\d*)?@(-?\d*\.?\d+)$");
        if (polarMatch.Success) {
            double magnitude = ParseComponent(polarMatch.Groups[1].Value, polarMatch.Groups[2].Value, polarMatch.Groups[3].Value);
            double phase = Math.Round(double.Parse(polarMatch.Groups[4].Value) * Math.PI / 180, 15); // Convert to radians
            return Complex.FromPolarCoordinates(magnitude, phase);
        }

        var match = Regex.Match(input, @"^(-?\d*\.?\d+)([munpMk]?)?(\d*)?([+-]\d*\.?\d+)?([munpMk]?)?(\d*)?j?$");
        if (!match.Success) throw new ArgumentException("Invalid input format");

        double real = ParseComponent(match.Groups[1].Value, match.Groups[2].Value, match.Groups[3].Value);
        double imaginary = 0;
        if (match.Groups[4].Success) {
            imaginary = ParseComponent(match.Groups[4].Value, match.Groups[5].Value, match.Groups[6].Value);
        }
        if (input.Contains('j') && !match.Groups[4].Success) {
            return new Complex(0, ParseComponent(match.Groups[1].Value, match.Groups[2].Value, match.Groups[3].Value));
        }
        return new Complex(real, imaginary);
    }

    private static double ParseComponent(string baseValue, string unit, string subscript) {
        double value = double.Parse(baseValue);
        double multiplier = unit switch {
            "m" => 1e-3,
            "u" => 1e-6,
            "n" => 1e-9,
            "p" => 1e-12,
            "k" => 1e3,
            "M" => 1e6,
            _ => 1.0
        };
        if (!string.IsNullOrEmpty(subscript)) {
            if (value < 0) {
                return value * multiplier - double.Parse(subscript) * (multiplier / (Math.Pow(10, subscript.Length)));
            }
            return value * multiplier + double.Parse(subscript) * (multiplier / (Math.Pow(10, subscript.Length)));
        }
        return value * multiplier;
    }

    public static string ConvertToUnit(Complex value, bool polar = false) {
        string[] units = { "p", "n", "µ", "m", "", "k", "M" };
        double[] multipliers = { 1e-12, 1e-9, 1e-6, 1e-3, 1, 1e3, 1e6 };

        string FormatComponent(double component) {
            for (int i = multipliers.Length - 1; i >= 0; i--) {
                if (Math.Abs(component) < multipliers[i]) continue;
                double scaled = component / multipliers[i];
                return scaled.ToString("0.##") + units[i];
            }
            return component.ToString("0.##");
        }
        if (polar) {
            double magnitude = value.Magnitude;
            double phase = value.Phase * 180 / Math.PI; // Convert to degrees
            return $"{FormatComponent(magnitude)}@{FormatComponent(phase)}°";
        }
        
        if (value.Imaginary == 0) return FormatComponent(value.Real);
        if (value.Real == 0) return FormatComponent(value.Imaginary) + "j";
        if (value.Imaginary < 0) return $"{FormatComponent(value.Real)} - {FormatComponent(-value.Imaginary)}j";
        return $"{FormatComponent(value.Real)} + {FormatComponent(value.Imaginary)}j";
    }
}