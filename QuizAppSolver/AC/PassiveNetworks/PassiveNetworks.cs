using QuizAppSolver.AC.PassiveNetworks.RCandRL;

namespace QuizAppSolver.AC.PassiveNetworks;

public class PassiveNetworks {
    public static void PassiveNetwork() {
        new UserInputBuilder().AddSelection("Select the [green]Passive Network type[/]",
            val => {
                Action action = val switch {
                    "RC and RL" => RCandRl.RCandRlMenu,
                    // "RLC" => RLC.RLCMenu,
                    "Back" => Program.Main,
                    _ => () => { }
                };
                action();
            }, ["RC and RL", "Back"]
        ).Build();
    }
}