using Spectre.Console;

namespace QuizAppSolver.OpAmps;

public class OpAmps {
    public static void OpAmp() {
        new UserInputBuilder().AddSelection("Select the [green]Op-Amp type[/]",
            val => {
                Action action = val switch {
                    "Inverting" => Inverting.InvertingMenu,
                    "Non-Inverting" => NonInverting.NonInvertingMenu,
                    "Inverting-Summing" => InvertingSumming.InvertingSummingMenu,
                    "Non-Inverting Summing" => NonInvertingSumming.NonInvertingSummingMenu,
                    "Differential" => Differential.DifferentialMenu,
                    "Back" => Program.Main,
                    _ => () => { }
                };
                action();
            }, ["Inverting", "Non-Inverting", "Inverting-Summing", "Non-Inverting Summing", "Differential", "Back"]
        ).Build();
    }
}