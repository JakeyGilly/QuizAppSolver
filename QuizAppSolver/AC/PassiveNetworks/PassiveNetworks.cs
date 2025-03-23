namespace QuizAppSolver.AC.PassiveNetworks;

public class PassiveNetworks {
    public static void PassiveNetwork() {
        new UserInputBuilder().AddSelection("Select the [green]Passive Network type[/]",
            val => {
                Action action = val switch {
                    "R" => R.Resistor,
                    "RC and RL" => RCandRl.RCandRlMenu,
                    // "RLC" => RLC.RLCMenu,
                    "Back" => Program.Main,
                    _ => () => { }
                };
                action();
            }, ["R", "RC and RL", "RLC", "Back"]
        ).Build();
    }
    
    
    
    
    
}