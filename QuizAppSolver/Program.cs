using QuizAppSolver;
using Spectre.Console;

public static class Program {
    public static void Main(string[] args) {
        AnsiConsole.Markup("Welcome to the [underline orange3]QuizApp Solver[/]!");
        AnsiConsole.WriteLine();
        var topic = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Select the [green]topic[/]")
                .PageSize(10)
                .MoreChoicesText("[grey](Move up and down to reveal more fruits)[/]")
                .AddChoices(new[] {
                    "Diodes"
                }));

        if (topic == "Diodes") {
            Diodes.Diode();
        }
    }
}
