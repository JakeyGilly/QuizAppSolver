using System.Numerics;
using Spectre.Console;

namespace QuizAppSolver.Diodes;

public class ForwardBiased {
    public static void ForwardBiasedMenu() {
        new UserInputBuilder().AddSelection("Select the [green]Forward Biased Diode type[/]",
            val => {
                Action action = val switch {
                    "Forward Biased Voltage" => ForwardBiasedVoltage,
                    "Parallel Forward Biased" => ParallelForwardBiased,
                    "Series Forward Biased" => SeriesForwardBiased,
                    "Forward Biased in Parallel with Resistor" => ForwardBiasedInParallelWithResistor,
                    "Back" => Diodes.Diode,
                    _ => () => { }
                };
                action();
            }, ["Forward Biased Voltage", "Parallel Forward Biased", "Series Forward Biased", "Forward Biased in Parallel with Resistor", "Back"]
        ).Build();
    }
    
    private static void ForwardBiasedVoltage() {
        double iS = 0, n = 0, Vcc = 0, R = 0;
        new UserInputBuilder()
            .AddVoltageInput("supply", val => Vcc = val.Real)
            .AddResistorInput("", val => R = val)
            .AddCurrentInput("saturation", val => iS = val.Real)
            .AddNumericInput("ideality factor", val => n = val, "2")
            .Build();

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
    
    private static void ParallelForwardBiased() {
        double num = 0, iS = 0, n = 0, Vcc = 0, R = 0;
        new UserInputBuilder()
            .AddNumericInput("number of diodes", val => num = val, "2")
            .AddVoltageInput("supply", val => Vcc = val.Real)
            .AddResistorInput("", val => R = val)
            .AddCurrentInput("saturation", val => iS = val.Real)
            .AddNumericInput("ideality factor", val => n = val, "2")
            .Build();
        
        double vR;
        double iD;
        var vD = 600e-3;
        while (true) {
            vR = Vcc - vD;
            iD = vR / (R * num);
            var newVD = (n / 40) * Math.Log(1 + iD / iS);
            if (Math.Abs(newVD - vD) == 0) {
                vD = newVD;
                break;
            }
            vD = newVD;
            if (Console.KeyAvailable) {
                break; //Stop the loop after a key as been pressed
            }
        }
        vR = Vcc - vD;
        iD = vR / (R * num);
        var iR = iD * num;
        
        AnsiConsole.WriteLine($"The current through each diode is {UnitConverter.ConvertToUnit(iD)}A.");
        AnsiConsole.WriteLine($"The current through the resistor is {UnitConverter.ConvertToUnit(iR)}A.");
        AnsiConsole.WriteLine($"The voltage across the resistor is {UnitConverter.ConvertToUnit(vR)}V.");
        AnsiConsole.WriteLine($"The voltage across the diodes is {UnitConverter.ConvertToUnit(vD)}V.");
        bool menu = AnsiConsole.Confirm("Back to the main menu");
        if (menu) Diodes.Diode();
    }
    
    private static void SeriesForwardBiased() {
        double num = 0, iS = 0, n = 0, Vcc = 0, R = 0;
        new UserInputBuilder()
            .AddNumericInput("number of diodes", val => num = val, "2")
            .AddVoltageInput("supply", val => Vcc = val.Real)
            .AddResistorInput("", val => R = val)
            .AddCurrentInput("saturation", val => iS = val.Real)
            .AddNumericInput("ideality factor", val => n = val, "2")
            .Build();

        double I;
        double vR;
        var vD = 600e-3;
        while (true) {
            vR = Vcc - (vD*num);
            I = vR / R;
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
        
        vR = Vcc - (vD*num);
        I = vR / R;
        
        AnsiConsole.WriteLine($"The current is {UnitConverter.ConvertToUnit(I)}A.");
        AnsiConsole.WriteLine($"The voltage across the resistor is {UnitConverter.ConvertToUnit(vR)}V.");
        AnsiConsole.WriteLine($"The voltage across each diode is {UnitConverter.ConvertToUnit(vD)}V.");
        bool menu = AnsiConsole.Confirm("Back to the main menu");
        if (menu) Diodes.Diode();
    }
    
    private static void ForwardBiasedInParallelWithResistor() {
        double iS = 0, n = 0, Vcc = 0, R = 0, RD = 0;
        new UserInputBuilder()
            .AddVoltageInput("supply", val => Vcc = val.Real)
            .AddResistorInput("", val => R = val)
            .AddResistorInput("diode", val => RD = val, postfix: "(the resistor in parallel with the diode)")
            .AddCurrentInput("saturation", val => iS = val.Real)
            .AddNumericInput("ideality factor", val => n = val, "2")
            .Build();
        
        double vR;
        double iR;
        double iRD;
        double iD;
        var vD = 600e-3;
        while (true) {
            vR = Vcc - vD;
            iR = vR / R;
            iRD = vD / RD;
            iD = iR - iRD;
            var newVD = (n / 40) * Math.Log(1 + iD / iS);
            if (Math.Abs(newVD - vD) == 0) {
                vD = newVD;
                break;
            }
            vD = newVD;
            if (Console.KeyAvailable) {
                break; //Stop the loop after a key as been pressed
            }
        }
        
        vR = Vcc - vD;
        iR = vR / R;
        iRD = vD / RD;
        iD = iR - iRD;
        
        AnsiConsole.WriteLine($"The current through the diode is {UnitConverter.ConvertToUnit(iD)}A.");
        AnsiConsole.WriteLine($"The current through the resistor is {UnitConverter.ConvertToUnit(iR)}A.");
        AnsiConsole.WriteLine($"The current through the resistor in parallel with the diode is {UnitConverter.ConvertToUnit(iRD)}A.");
        AnsiConsole.WriteLine($"The voltage across the resistor is {UnitConverter.ConvertToUnit(vR)}V.");
        AnsiConsole.WriteLine($"The voltage across the diode is {UnitConverter.ConvertToUnit(vD)}V.");
        bool menu = AnsiConsole.Confirm("Back to the main menu");
        if (menu) Diodes.Diode();
    }
}