using Spectre.Console;

namespace QuizAppSolver.Diodes;

public class Zener {
    public static void ZenerDiodeMenu() {
        new UserInputBuilder().AddSelection("Select the [green]Zener Diode type[/]",
            val => {
                Action action = val switch {
                    "Reverse Biased" => ZenerDiodeReverseBiased,
                    "Reverse Biased Parallel with Resistor" => ZenerDiodeReverseBiasedParallelWithResistor,
                    "Back" => Diodes.Diode,
                    _ => () => { }
                };
                action();
            }, ["Reverse Biased", "Reverse Biased Parallel with Resistor", "Back"]
        ).Build();
    }

    private static void ZenerDiodeReverseBiased() {
        double iBZ = 0, vBZ = 0, Vcc = 0, rDyn = 0, R = 0;
        new UserInputBuilder()
            .AddVoltageInput("supply", val => Vcc = val)
            .AddResistorInput("", val => R = val)
            .AddCurrentInput("Zener breakdown voltage current rating", val => iBZ = val, useSuffix: false)
            .AddVoltageInput("Zener breakdown", val => vBZ = val)
            .AddResistorInput("dynamic resistance", val => rDyn = val, useSuffix: false)
            .Build();
        
        if (Vcc < vBZ) {
            AnsiConsole.WriteLine("The Zener diode is not in breakdown. Use Reverse Biased instead.");
            return;
        }

        double current = (Vcc - vBZ + (rDyn * iBZ)) / (R + rDyn);
        double vR = current * R;
        double vZ = Vcc - vR;

        AnsiConsole.WriteLine($"The current is {UnitConverter.ConvertToUnit(current)}A.");
        AnsiConsole.WriteLine($"The voltage across the resistor is {UnitConverter.ConvertToUnit(vR)}V.");
        AnsiConsole.WriteLine($"The voltage across the Zener diode is {UnitConverter.ConvertToUnit(vZ)}V.");
        bool menu = AnsiConsole.Confirm("Back to the main menu");
        if (menu) Diodes.Diode();
    }
    
    private static void ZenerDiodeReverseBiasedParallelWithResistor() {
        double iBZ = 0, vBZ = 0, Vcc = 0, rDyn = 0, R = 0, RZ = 0;
        new UserInputBuilder()
            .AddVoltageInput("supply", val => Vcc = val)
            .AddResistorInput("", val => R = val)
            .AddResistorInput("diode", val => RZ = val, postfix: "(the resistor in parallel with the diode)")
            .AddCurrentInput("Zener breakdown voltage current rating", val => iBZ = val, useSuffix: false)
            .AddVoltageInput("Zener breakdown", val => vBZ = val)
            .AddResistorInput("dynamic resistance", val => rDyn = val, useSuffix: false)
            .Build();
        
        var vRDWithoutZener = Vcc * RZ / (R + RZ);
        if (vRDWithoutZener < vBZ) {
            AnsiConsole.WriteLine("The Zener diode is not in breakdown. Use Reverse Biased instead.");
            return;
        }

        double vR, iR, iRD, iZ;
        var vZ = vBZ;
        while (true) {
            vR = Vcc - vZ;
            iR = vR / R;
            iRD = vZ / RZ;
            iZ = iR - iRD;

            var newVZ = rDyn * (iZ - iBZ) + vBZ;
            if (Math.Abs(newVZ - vZ) == 0) {
                vZ = newVZ;
                break;
            }
            vZ = newVZ;
            if (Console.KeyAvailable) {
                break; //Stop the loop after a key as been pressed
            }
        }
        
        vR = Vcc - vZ;
        iR = vR / R;
        iRD = vZ / RZ;
        iZ = iR - iRD;
        
        AnsiConsole.WriteLine($"The current through the Zener diode is {UnitConverter.ConvertToUnit(iZ)}A.");
        AnsiConsole.WriteLine($"The current through the resistor is {UnitConverter.ConvertToUnit(iR)}A.");
        AnsiConsole.WriteLine($"The current through the resistor in parallel with the diode is {UnitConverter.ConvertToUnit(iRD)}A.");
        AnsiConsole.WriteLine($"The voltage across the resistor is {UnitConverter.ConvertToUnit(vR)}V.");
        AnsiConsole.WriteLine($"The voltage across the Zener diode is {UnitConverter.ConvertToUnit(vZ)}V.");
        bool menu = AnsiConsole.Confirm("Back to the main menu");
        if (menu) Diodes.Diode();
    }
}