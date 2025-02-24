using Spectre.Console;

namespace QuizAppSolver;

public class Diodes {
    public static void Diode() {
        var diode = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Select the [green]Diode type[/]")
                .PageSize(10)
                .MoreChoicesText("[grey](Move up and down to reveal more options)[/]")
                .AddChoices(new[] {
                    "Forward Biased",
                    "Reverse Biased",
                    "Zener Diode Reverse Biased",
                    "Iterator",
                }));
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
                Iterator();
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
        
        var vD = 600e-3;
        while (true) {
            var newVD = (n / 40) * Math.Log(1 + (Vcc - vD) / (R * iS));
            if (Math.Abs(newVD - vD) == 0) {
                vD = newVD;
                break;
            }
            vD = newVD;
        }
        var iR = (Vcc - vD) / R;
        var vR = iR * R;
        
        AnsiConsole.WriteLine($"The current is {UnitConverter.ConvertToUnit(iR)}A.");
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
        
        double current = (Vcc - vBZ + (rDyn * iBZ)) / (R + rDyn);
        double vR = current * R;
        double vZ = Vcc - vR;

        AnsiConsole.WriteLine($"The current is {UnitConverter.ConvertToUnit(current)}A.");
        AnsiConsole.WriteLine($"The voltage across the resistor is {UnitConverter.ConvertToUnit(vR)}V.");
        AnsiConsole.WriteLine($"The voltage across the Zener diode is {UnitConverter.ConvertToUnit(vZ)}V.");
    }

    private static void Iterator() {
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
        
        var vD = 600e-3;
        while (true) {
            var newVD = (n / 40) * Math.Log(1 + (Vcc - vD) / (R * iS));
            if (Math.Abs(newVD - vD) == 0) {
                vD = newVD;
                break;
            }
            vD = newVD;
        }
        
        var vR = (Vcc - vD);
        var current = vR / R;
        
        AnsiConsole.WriteLine($"The current is {UnitConverter.ConvertToUnit(current)}A.");
        AnsiConsole.WriteLine($"The voltage across the resistor is {UnitConverter.ConvertToUnit(vR)}V.");
        AnsiConsole.WriteLine($"The voltage across the diode is {UnitConverter.ConvertToUnit(vD)}V.");
    }
}