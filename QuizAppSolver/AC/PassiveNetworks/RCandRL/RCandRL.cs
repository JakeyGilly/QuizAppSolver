namespace QuizAppSolver.AC.PassiveNetworks.RCandRL;

public class RCandRl {
    public static void RCandRlMenu() {
        new UserInputBuilder().AddSelection("Select the [green]RC type[/]",
            val => {
                Action action = val switch {
                    "Arrange Circuit (2 or 3 Components)" => ArrangeCircuit.ArrangeRLandRC,
                    "Back" => Program.Main,
                    _ => () => { }
                };
                action();
            }, ["Arrange Circuit (2 or 3 Components)", "Back"]
        ).Build();
    }
}