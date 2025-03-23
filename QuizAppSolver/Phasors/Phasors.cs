using System.Numerics;
using Spectre.Console;

namespace QuizAppSolver.Phasors;

public class Phasors {
    public static void Phasor() {
        new UserInputBuilder().AddSelection("Select the [green]Phasor type[/]",
            val => {
                Action action = val switch {
                    "Add/Subtract" => AddPhasors,
                    "Phasor A and B to Phasor" => PhasorAAndBToPhasor,
                    "Radians to Degrees" => RadiansToDegrees,
                    "Back" => Program.Main,
                    _ => () => { }
                };
                action();
            }, ["Add/Subtract", "Phasor A and B to Phasor", "Radians to Degrees", "Back"]
        ).Build();
    }

    public static void AddPhasors() {
        Complex phasor1 = new Complex(0, 0);
        Complex phasor2 = new Complex(0, 0);
        bool add = true;
        new UserInputBuilder()
            .AddComplexNumericInput("value of the first phasor", val => phasor1 = new Complex(val.Real, val.Imaginary), "1@0")
            .AddComplexNumericInput("value of the second phasor", val => phasor2 = new Complex(val.Real, val.Imaginary), "1@0")
            .AddSelection("Select the [green]operation[/]",
                val => {
                    add = val == "Add";
                }, ["Add", "Subtract", "Back"]
            ).Build();
        
        var result = add ? phasor1 + phasor2 : phasor1 - phasor2;
        AnsiConsole.WriteLine($"The result is {UnitConverter.ConvertToUnit(result, true)}");
        bool menu = AnsiConsole.Confirm("Back to the main menu");
        if (menu) Program.Main();
    }
    
    public static void RadiansToDegrees() {
        double radians = 0;
        new UserInputBuilder()
            .AddNumericInput("radians", val => radians = val)
            .Build();
        
        var degrees = radians * 180 / Math.PI;
        AnsiConsole.WriteLine($"The result is {UnitConverter.ConvertToUnit(degrees)}");
        bool menu = AnsiConsole.Confirm("Back to the main menu");
        if (menu) Program.Main();
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
}