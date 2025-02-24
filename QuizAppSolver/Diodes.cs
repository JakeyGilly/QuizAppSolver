using Spectre.Console;

namespace QuizAppSolver;

public class Diodes {
    public static void Diode() {
        var diode = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Select the [green]Diode type[/]")
                .PageSize(10)
                .MoreChoicesText("[grey](Move up and down to reveal more options)[/]")
                .AddChoices("Forward Biased", "Reverse Biased", "Zener Diode Reverse Biased", "Iterator", "Multiple Diodes"));
        switch (diode) {
            case "Forward Biased":
                ForwardBiased();
                break;
            case "Reverse Biased":
                ReverseBiased();
                break;
            case "Zener Diode Reverse Biased":
                ZenerDiodeReverseBiased();
                break;
            case "Iterator":
                ForwardBiased();
                break;
            case "Multiple Diodes":
                MultipleDiodes();
                break;
        }
    }

    private static void ForwardBiased() {
        var iSInput = AnsiConsole.Prompt(
            new TextPrompt<string>("Enter the [green]saturation current[/]")
                .PromptStyle("grey")
                .DefaultValue("2n")
                .DefaultValueStyle("grey"));
        var nInput = AnsiConsole.Prompt(
            new TextPrompt<string>("Enter the [green]ideality factor[/]")
                .PromptStyle("grey")
                .DefaultValue("2")
                .DefaultValueStyle("grey"));
        var VccInput = AnsiConsole.Prompt(
            new TextPrompt<string>("Enter the [green]supply voltage[/]")
                .PromptStyle("grey")
                .DefaultValue("12")
                .DefaultValueStyle("grey"));
        var RInput = AnsiConsole.Prompt(
            new TextPrompt<string>("Enter the [green]resistance of the resistor[/]")
                .PromptStyle("grey")
                .DefaultValue("1k")
                .DefaultValueStyle("grey"));

        var iS = UnitConverter.ConvertUnits(iSInput);
        var n = UnitConverter.ConvertUnits(nInput);
        var Vcc = UnitConverter.ConvertUnits(VccInput);
        var R = UnitConverter.ConvertUnits(RInput);

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
    }

    private static void ReverseBiased() {
        var iSInput = AnsiConsole.Prompt(
            new TextPrompt<string>("Enter the [green]saturation current[/]")
                .PromptStyle("grey")
                .DefaultValue("2n")
                .DefaultValueStyle("grey"));
        var VccInput = AnsiConsole.Prompt(
            new TextPrompt<string>("Enter the [green]supply voltage[/]")
                .PromptStyle("grey")
                .DefaultValue("12")
                .DefaultValueStyle("grey"));
        var RInput = AnsiConsole.Prompt(
            new TextPrompt<string>("Enter the [green]resistance of the resistor[/]")
                .PromptStyle("grey")
                .DefaultValue("1k")
                .DefaultValueStyle("grey"));

        var iS = UnitConverter.ConvertUnits(iSInput);
        var Vcc = UnitConverter.ConvertUnits(VccInput);
        var R = UnitConverter.ConvertUnits(RInput);

        var current = iS;
        var vR = iS * R;
        var vD = Vcc - vR;

        AnsiConsole.WriteLine($"The current is {UnitConverter.ConvertToUnit(current)}A.");
        AnsiConsole.WriteLine($"The voltage across the resistor is {UnitConverter.ConvertToUnit(vR)}V.");
        AnsiConsole.WriteLine($"The voltage across the diode is {UnitConverter.ConvertToUnit(vD)}V.");
    }

    private static void ZenerDiodeReverseBiased() {
        var rDynInput = AnsiConsole.Prompt(
            new TextPrompt<string>("Enter the [green]dynamic resistance[/]")
                .PromptStyle("grey")
                .DefaultValue("40")
                .DefaultValueStyle("grey"));
        var iBZInput = AnsiConsole.Prompt(
            new TextPrompt<string>("Enter the [green]Zener breakdown voltage current rating[/]")
                .PromptStyle("grey")
                .DefaultValue("5m")
                .DefaultValueStyle("grey"));
        var vBZInput = AnsiConsole.Prompt(
            new TextPrompt<string>("Enter the [green]Zener breakdown voltage[/]")
                .PromptStyle("grey")
                .DefaultValue("3.3")
                .DefaultValueStyle("grey"));
        var VccInput = AnsiConsole.Prompt(
            new TextPrompt<string>("Enter the [green]supply voltage[/]")
                .PromptStyle("grey")
                .DefaultValue("12")
                .DefaultValueStyle("grey"));
        var RInput = AnsiConsole.Prompt(
            new TextPrompt<string>("Enter the [green]resistance of the resistor[/]")
                .PromptStyle("grey")
                .DefaultValue("1k")
                .DefaultValueStyle("grey"));
        
        var iBZ = UnitConverter.ConvertUnits(iBZInput);
        var vBZ = UnitConverter.ConvertUnits(vBZInput);
        var Vcc = UnitConverter.ConvertUnits(VccInput);
        var rDyn = UnitConverter.ConvertUnits(rDynInput);
        var R = UnitConverter.ConvertUnits(RInput);
        
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
    }

    private static void MultipleDiodes() {
        var diode = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Select the [green]Multiple Diode type[/]")
                .PageSize(10)
                .MoreChoicesText("[grey](Move up and down to reveal more options)[/]")
                .AddChoices("Parallel Forward Biased", "Series Forward Biased", "Forward Biased in Parallel with Resistor", "Reverse Biased in Parallel with Resistor", "Zener Diode Reverse Biased in Parallel with Resistor", "Back"));
        switch (diode) {
            case "Parallel Forward Biased":
                ParallelForwardBiased();
                break;
            case "Series Forward Biased":
                SeriesForwardBiased();
                break;
            case "Forward Biased in Parallel with Resistor":
                ForwardBiasedInParallelWithResistor();
                break;
            case "Reverse Biased in Parallel with Resistor":
                ReverseBiasedInParallelWithResistor();
                break;
            case "Zener Diode Reverse Biased in Parallel with Resistor":
                ZenerDiodeReverseBiasedParallelWithResistor();
                break;
            case "Back":
                Diode();
                break;
        }
    }

    private static void ParallelForwardBiased() {
        var numInput = AnsiConsole.Prompt(
            new TextPrompt<string>("Enter the [green]number of diodes[/]")
                .PromptStyle("grey")
                .DefaultValue("2")
                .DefaultValueStyle("grey"));
        var iSInput = AnsiConsole.Prompt(
            new TextPrompt<string>("Enter the [green]saturation current[/]")
                .PromptStyle("grey")
                .DefaultValue("2n")
                .DefaultValueStyle("grey"));
        var nInput = AnsiConsole.Prompt(
            new TextPrompt<string>("Enter the [green]ideality factor[/]")
                .PromptStyle("grey")
                .DefaultValue("2")
                .DefaultValueStyle("grey"));
        var VccInput = AnsiConsole.Prompt(
            new TextPrompt<string>("Enter the [green]supply voltage[/]")
                .PromptStyle("grey")
                .DefaultValue("12")
                .DefaultValueStyle("grey"));
        var RInput = AnsiConsole.Prompt(
            new TextPrompt<string>("Enter the [green]resistance of the resistor[/]")
                .PromptStyle("grey")
                .DefaultValue("1k")
                .DefaultValueStyle("grey"));

        var num = UnitConverter.ConvertUnits(numInput);
        var iS = UnitConverter.ConvertUnits(iSInput);
        var n = UnitConverter.ConvertUnits(nInput);
        var Vcc = UnitConverter.ConvertUnits(VccInput);
        var R = UnitConverter.ConvertUnits(RInput);
        
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
    }

    private static void SeriesForwardBiased() {
        var numInput = AnsiConsole.Prompt(
            new TextPrompt<string>("Enter the [green]number of diodes[/]")
                .PromptStyle("grey")
                .DefaultValue("2")
                .DefaultValueStyle("grey"));
        var iSInput = AnsiConsole.Prompt(
            new TextPrompt<string>("Enter the [green]saturation current[/]")
                .PromptStyle("grey")
                .DefaultValue("2n")
                .DefaultValueStyle("grey"));
        var nInput = AnsiConsole.Prompt(
            new TextPrompt<string>("Enter the [green]ideality factor[/]")
                .PromptStyle("grey")
                .DefaultValue("2")
                .DefaultValueStyle("grey"));
        var VccInput = AnsiConsole.Prompt(
            new TextPrompt<string>("Enter the [green]supply voltage[/]")
                .PromptStyle("grey")
                .DefaultValue("12")
                .DefaultValueStyle("grey"));
        var RInput = AnsiConsole.Prompt(
            new TextPrompt<string>("Enter the [green]resistance of the resistor[/]")
                .PromptStyle("grey")
                .DefaultValue("1k")
                .DefaultValueStyle("grey"));

        var num = UnitConverter.ConvertUnits(numInput);
        var iS = UnitConverter.ConvertUnits(iSInput);
        var n = UnitConverter.ConvertUnits(nInput);
        var Vcc = UnitConverter.ConvertUnits(VccInput);
        var R = UnitConverter.ConvertUnits(RInput);

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
    }

    private static void ForwardBiasedInParallelWithResistor() {
        var iSInput = AnsiConsole.Prompt(
            new TextPrompt<string>("Enter the [green]saturation current[/]")
                .PromptStyle("grey")
                .DefaultValue("2n")
                .DefaultValueStyle("grey"));
        var nInput = AnsiConsole.Prompt(
            new TextPrompt<string>("Enter the [green]ideality factor[/]")
                .PromptStyle("grey")
                .DefaultValue("2")
                .DefaultValueStyle("grey"));
        var VccInput = AnsiConsole.Prompt(
            new TextPrompt<string>("Enter the [green]supply voltage[/]")
                .PromptStyle("grey")
                .DefaultValue("12")
                .DefaultValueStyle("grey"));
        var RInput = AnsiConsole.Prompt(
            new TextPrompt<string>("Enter the [green]resistance of the resistor[/]")
                .PromptStyle("grey")
                .DefaultValue("1k")
                .DefaultValueStyle("grey"));
        var RDInput = AnsiConsole.Prompt(
            new TextPrompt<string>("Enter the [green]resistance of the resistor in parallel with the diode[/]")
                .PromptStyle("grey")
                .DefaultValue("1k")
                .DefaultValueStyle("grey"));
        
        var iS = UnitConverter.ConvertUnits(iSInput);
        var n = UnitConverter.ConvertUnits(nInput);
        var Vcc = UnitConverter.ConvertUnits(VccInput);
        var R = UnitConverter.ConvertUnits(RInput);
        var RD = UnitConverter.ConvertUnits(RDInput);
        
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
    }

    private static void ReverseBiasedInParallelWithResistor() {
        var iSInput = AnsiConsole.Prompt(
            new TextPrompt<string>("Enter the [green]saturation current[/]")
                .PromptStyle("grey")
                .DefaultValue("2n")
                .DefaultValueStyle("grey"));
        var nInput = AnsiConsole.Prompt(
            new TextPrompt<string>("Enter the [green]ideality factor[/]")
                .PromptStyle("grey")
                .DefaultValue("2")
                .DefaultValueStyle("grey"));
        var VccInput = AnsiConsole.Prompt(
            new TextPrompt<string>("Enter the [green]supply voltage[/]")
                .PromptStyle("grey")
                .DefaultValue("12")
                .DefaultValueStyle("grey"));
        var RInput = AnsiConsole.Prompt(
            new TextPrompt<string>("Enter the [green]resistance of the resistor[/]")
                .PromptStyle("grey")
                .DefaultValue("1k")
                .DefaultValueStyle("grey"));
        var RDInput = AnsiConsole.Prompt(
            new TextPrompt<string>("Enter the [green]resistance of the resistor in parallel with the diode[/]")
                .PromptStyle("grey")
                .DefaultValue("1k")
                .DefaultValueStyle("grey"));
        
        var iS = UnitConverter.ConvertUnits(iSInput);
        var n = UnitConverter.ConvertUnits(nInput);
        var Vcc = UnitConverter.ConvertUnits(VccInput);
        var R = UnitConverter.ConvertUnits(RInput);
        var RD = UnitConverter.ConvertUnits(RDInput);
        
        var vRWithoutDiode = Vcc * RD / (R + RD);
        var current = iS;
        var iRD = vRWithoutDiode / RD;
        var vR = Vcc - vRWithoutDiode;
        
        AnsiConsole.WriteLine($"The current through the diode is {UnitConverter.ConvertToUnit(current)}A.");
        AnsiConsole.WriteLine($"The current through the resistors is {UnitConverter.ConvertToUnit(iRD)}A.");
        AnsiConsole.WriteLine($"The voltage across the resistor in parallel with the diode is {UnitConverter.ConvertToUnit(vRWithoutDiode)}V.");
        AnsiConsole.WriteLine($"The voltage across the resistor is {UnitConverter.ConvertToUnit(vR)}V.");
    }
    
    private static void ZenerDiodeReverseBiasedParallelWithResistor() {
        var rDynInput = AnsiConsole.Prompt(
            new TextPrompt<string>("Enter the [green]dynamic resistance[/]")
                .PromptStyle("grey")
                .DefaultValue("40")
                .DefaultValueStyle("grey"));
        var iBZInput = AnsiConsole.Prompt(
            new TextPrompt<string>("Enter the [green]Zener breakdown voltage current rating[/]")
                .PromptStyle("grey")
                .DefaultValue("5m")
                .DefaultValueStyle("grey"));
        var vBZInput = AnsiConsole.Prompt(
            new TextPrompt<string>("Enter the [green]Zener breakdown voltage[/]")
                .PromptStyle("grey")
                .DefaultValue("3.3")
                .DefaultValueStyle("grey"));
        var VccInput = AnsiConsole.Prompt(
            new TextPrompt<string>("Enter the [green]supply voltage[/]")
                .PromptStyle("grey")
                .DefaultValue("12")
                .DefaultValueStyle("grey"));
        var RInput = AnsiConsole.Prompt(
            new TextPrompt<string>("Enter the [green]resistance of the resistor[/]")
                .PromptStyle("grey")
                .DefaultValue("1k")
                .DefaultValueStyle("grey"));
        var RDInput = AnsiConsole.Prompt(
            new TextPrompt<string>("Enter the [green]resistance of the resistor in parallel with the diode[/]")
                .PromptStyle("grey")
                .DefaultValue("1k")
                .DefaultValueStyle("grey"));
        
        var iBZ = UnitConverter.ConvertUnits(iBZInput);
        var vBZ = UnitConverter.ConvertUnits(vBZInput);
        var Vcc = UnitConverter.ConvertUnits(VccInput);
        var rDyn = UnitConverter.ConvertUnits(rDynInput);
        var R = UnitConverter.ConvertUnits(RInput);
        var RZ = UnitConverter.ConvertUnits(RDInput);
        
        var vRDWithoutZener = Vcc * RZ / (R + RZ);
        if (vRDWithoutZener < vBZ) {
            AnsiConsole.WriteLine("The Zener diode is not in breakdown. Use Reverse Biased instead.");
            return;
        }

        double vR;
        double iR;
        double iRD;
        double iZ;
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
    }
}