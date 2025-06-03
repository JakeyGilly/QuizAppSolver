using QuizAppSolver.Diodes;
using QuizAppSolver.OpAmps;
using QuizAppSolver;
using QuizAppSolver.AC.Dividers;
using QuizAppSolver.AC.PassiveNetworks;
using QuizAppSolver.OpAmpBandwidths;
using QuizAppSolver.Phasors;
using Spectre.Console;

public static class Program {
    public static void Main() {
        AnsiConsole.Markup("Welcome to the [underline orange3]QuizApp Solver[/]!");
        AnsiConsole.WriteLine();
        new UserInputBuilder().AddSelection("Select the [green]topic[/]",
            val => {
                Action action = val switch {
                    "Diodes" => Diodes.Diode,
                    "Op-Amps" => OpAmps.OpAmp,
                    "Phasors" => Phasors.Phasor,
                    "Passive Networks" => PassiveNetworks.PassiveNetwork,
                    "Dividers" => Dividers.DividersMenu,
                    "Op-Amp Bandwidths" => OpAmpBandwidths.OpAmpBandwithsMenu,
                    _ => () => { }
                };
                action();
            }, ["Diodes", "Op-Amps", "Phasors", "Passive Networks", "Dividers", "Op-Amp Bandwidths"]
        ).Build();
    }
}
