using Spectre.Console;

namespace QuizAppSolver.AC.PassiveNetworks;

public class R {
    public static void Resistor() {
        double resistance = 0;
        new UserInputBuilder()
            .AddResistorInput("", val => resistance = val)
            .Build();
        
        AnsiConsole.WriteLine($"The result is {UnitConverter.ConvertToUnit(resistance)}");
        bool menu = AnsiConsole.Confirm("Back to the main menu");
        if (menu) Program.Main();
    }
}