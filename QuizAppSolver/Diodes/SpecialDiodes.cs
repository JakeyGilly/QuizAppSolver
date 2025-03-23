using Spectre.Console;

namespace QuizAppSolver.Diodes;

public class SpecialDiodes {
    public static void ParallelDiodesInOppositeDirections() {
        // only works for 2 diodes
        // meant to be 1 forward biased and 1 reverse biased
        // doesn't support zener diodes past breakdown
        double iS = 0, n = 0, Vcc = 0, R = 0;
        var diodes = new[] { "Forward Biased", "Reverse Biased" };
        new UserInputBuilder()
            .AddVoltageInput("supply", val => Vcc = val.Real)
            .AddResistorInput("", val => R = val)
            .AddCurrentInput("saturation current", val => iS = val.Real)
            .AddNumericInput("ideality factor", val => n = val, "2")
            .Build();
        
        // reverse bias diode is negliable
        foreach (var diode in diodes) {
            if (diode != "Forward Biased") continue;
            double vR;
            var vD = 600e-3;
            while (true) {
                vR = Vcc - vD;
                var I = vR / R;
                var newVD = (n / 40) * Math.Log(1 + I / iS);
                if (Math.Abs(newVD - vD) == 0) {
                    vD = newVD;
                    break;
                }
                vD = newVD;
                if (Console.KeyAvailable) {
                    break; //Stop the loop after a key as been pressed
                }
            }
            var current = vR / R;
            
            AnsiConsole.WriteLine($"The current is {UnitConverter.ConvertToUnit(current)}A.");
            AnsiConsole.WriteLine($"The voltage across the resistor is {UnitConverter.ConvertToUnit(vR)}V.");
            AnsiConsole.WriteLine($"The voltage across the diode is {UnitConverter.ConvertToUnit(vD)}V.");
            bool menu = AnsiConsole.Confirm("Back to the main menu");
            if (menu) Diodes.Diode();
        }
    }
}