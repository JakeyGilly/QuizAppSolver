using Spectre.Console;

namespace QuizAppSolver.Diodes;

public class ReverseBiased {
    public static void ReverseBiasedMenu() {
        new UserInputBuilder().AddSelection("Select the [green]Reverse Biased Diode type[/]",
            val => {
                Action action = val switch {
                    "Reverse Biased Voltage" => ReverseBiasedVoltage,
                    "Reverse Biased in Parallel with Resistor" => ReverseBiasedInParallelWithResistor,
                    "Back" => Diodes.Diode,
                    _ => () => { }
                };
                action();
            }, ["Reverse Biased Voltage", "Reverse Biased in Parallel with Resistor", "Back"]
        ).Build();
    }
    
    private static void ReverseBiasedVoltage() {
        double iS = 0, Vcc = 0, R = 0;
        new UserInputBuilder()
            .AddCurrentInput("saturation", val => iS = val)
            .AddVoltageInput("supply", val => Vcc = val)
            .AddResistorInput("", val => R = val)
            .Build();

        var current = iS;
        var vR = iS * R;
        var vD = Vcc - vR;

        AnsiConsole.WriteLine($"The current is {UnitConverter.ConvertToUnit(current)}A.");
        AnsiConsole.WriteLine($"The voltage across the resistor is {UnitConverter.ConvertToUnit(vR)}V.");
        AnsiConsole.WriteLine($"The voltage across the diode is {UnitConverter.ConvertToUnit(vD)}V.");
        bool menu = AnsiConsole.Confirm("Back to the main menu");
        if (menu) Diodes.Diode();
    }
    
    private static void ReverseBiasedInParallelWithResistor() {
        double iS = 0, n = 0, Vcc = 0, R = 0, RD = 0;
        new UserInputBuilder()
            .AddVoltageInput("supply", val => Vcc = val)
            .AddResistorInput("", val => R = val)
            .AddResistorInput("diode", val => RD = val, postfix: "(the resistor in parallel with the diode)")
            .AddCurrentInput("saturation", val => iS = val)
            .AddNumericInput("ideality factor", val => n = val, "2")
            .Build();
        
        var vRWithoutDiode = Vcc * RD / (R + RD);
        var current = iS;
        var iRD = vRWithoutDiode / RD;
        var vR = Vcc - vRWithoutDiode;
        
        AnsiConsole.WriteLine($"The current through the diode is {UnitConverter.ConvertToUnit(current)}A.");
        AnsiConsole.WriteLine($"The current through the resistor and diode is {UnitConverter.ConvertToUnit(iRD)}A.");
        AnsiConsole.WriteLine($"The voltage across the resistor in parallel with the diode is {UnitConverter.ConvertToUnit(vRWithoutDiode)}V.");
        AnsiConsole.WriteLine($"The voltage across the resistor is {UnitConverter.ConvertToUnit(vR)}V.");
        bool menu = AnsiConsole.Confirm("Back to the main menu");
        if (menu) Diodes.Diode();
    }
}