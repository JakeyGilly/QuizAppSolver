using QuizAppSolver;
using QuizAppSolver.Diodes;
using QuizAppSolver.OpAmps;
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
                    _ => () => { }
                };
                action();
            }, ["Diodes", "Op-Amps"]
        ).Build();
    }
}
