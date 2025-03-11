namespace QuizAppSolver.Diodes;

public class Diodes {
    public static void Diode() {
        new UserInputBuilder().AddSelection("Select the [green]Diode type[/]",
            val => {
                Action action = val switch {
                    "Forward Biased" => ForwardBiased.ForwardBiasedMenu,
                    "Reverse Biased" => ReverseBiased.ReverseBiasedMenu,
                    "Zener Diode Reverse Biased" => Zener.ZenerDiodeMenu,
                    "Parallel Diodes in Opposite Directions" => SpecialDiodes.ParallelDiodesInOppositeDirections,
                    "Back" => Program.Main,
                    _ => () => { }
                };
                action();
            }, ["Forward Biased", "Reverse Biased", "Zener Diode Reverse Biased", "Parallel Diodes in Opposite Directions", "Back"]
        ).Build();
    }
}