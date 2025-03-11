using QuizAppSolver;
using QuizAppSolver.Diodes;
using QuizAppSolver.OpAmps;
using Spectre.Console;

public static class Program {
    public static void Main() {
        AnsiConsole.Markup("Welcome to the [underline orange3]QuizApp Solver[/]!");
        AnsiConsole.WriteLine();
        var topic = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Select the [green]topic[/]")
                .PageSize(10)
                .MoreChoicesText("[grey](Move up and down to reveal more fruits)[/]")
                .AddChoices("Diodes", "Op-Amps"));

        switch (topic) {
            case "Diodes":
                Diodes.Diode();
                break;
            case "Op-Amps":
                OpAmps.OpAmp();
                break;
        }
    }
}
