using System.Numerics;
using Spectre.Console;

namespace QuizAppSolver.Phasors;

public class Phasors {
    public static void Phasor() {
        new UserInputBuilder().AddSelection("Select the [green]Phasor type[/]",
            val => {
                Action action = val switch {
                    "Phasor A and B to Phasor" => PhasorAAndBToPhasor,
                    "Time to Phase" => TimeToPhase,
                    "Back" => Program.Main,
                    _ => () => { }
                };
                action();
            }, ["Phasor A and B to Phasor", "Time to Phase", "Back"]
        ).Build();
    }
    
    public static void PhasorAAndBToPhasor() {
        Complex phasorA = new Complex(0, 0);
        Complex phasorB = new Complex(0, 0);
        Complex phasorResult = new Complex(0, 0);
        new UserInputBuilder()
            .AddComplexNumericInput("value of the first phasor", val => phasorA = new Complex(val.Real, val.Imaginary), "1@0")
            .AddComplexNumericInput("value of the second phasor", val => phasorB = new Complex(val.Real, val.Imaginary), "1@0")
            .AddComplexNumericInput("value of the resultant phasor", val => phasorResult = new Complex(val.Real, val.Imaginary), "1@0")
            .Build();
        
        var firstTry = phasorA + phasorB;
        var secondTry = phasorA - phasorB;
        var thirdTry = -phasorB - phasorA;
        var fourthTry = -phasorA + phasorB;
        var magnitudeTolerance = 0.1;
        var angleTolerance = 2;
        var solved = false;

        // ReSharper disable once LoopVariableIsNeverChangedInsideLoop
        while (!solved) {
            if (Math.Abs(firstTry.Magnitude - phasorResult.Magnitude) < magnitudeTolerance &&
                Math.Abs(firstTry.Phase - phasorResult.Phase) < angleTolerance) {
                AnsiConsole.WriteLine("The answer is A+B");
                break;
            }
            if (Math.Abs(secondTry.Magnitude - phasorResult.Magnitude) < magnitudeTolerance && Math.Abs(secondTry.Phase - phasorResult.Phase) < angleTolerance) {
                AnsiConsole.WriteLine("The answer is A-B");
                break;
            }
            if (Math.Abs(thirdTry.Magnitude - phasorResult.Magnitude) < magnitudeTolerance && Math.Abs(thirdTry.Phase - phasorResult.Phase) < angleTolerance) {
                AnsiConsole.WriteLine("The answer is -B-A");
                break;
            }
            if (Math.Abs(fourthTry.Magnitude - phasorResult.Magnitude) < magnitudeTolerance && Math.Abs(fourthTry.Phase - phasorResult.Phase) < angleTolerance) {
                AnsiConsole.WriteLine("The answer is -A+B");
                break;
            }

            AnsiConsole.WriteLine("No valid result found. Increasing tolerance...");
            magnitudeTolerance += 0.1;
            angleTolerance += 2;
        }
        bool menu = AnsiConsole.Confirm("Back to the main menu");
        if (menu) Program.Main();
    }

    public static void TimeToPhase() {
        double timeOfPeakA = 0;
        double frequency = 0;
        new UserInputBuilder()
            .AddFrequencyInput("", val => frequency = val.Real)
            .AddNumericInput("time of the peak of the signal", val => timeOfPeakA = val, "1000u")
            .Build();
        double period = 1 / frequency;
        double timeDifference = period - timeOfPeakA;

        // Normalize to range [0, period)
        timeDifference = (timeDifference + period) % period;

        double phaseFraction = timeDifference / period;
        double phase = phaseFraction * 360;

        // Round to nearest 15°
        phase = Math.Round(phase / 15) * 15;
        AnsiConsole.WriteLine($"The phase is {UnitConverter.ConvertToUnit(phase)} degrees");
        bool menu = AnsiConsole.Confirm("Back to the main menu");
        if (menu) Program.Main();
    }
}