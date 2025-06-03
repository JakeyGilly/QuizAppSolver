namespace QuizAppSolver.AC.Dividers;

public class Dividers {
    public static void DividersMenu() {
        new UserInputBuilder().AddSelection("Select the [green]Divider type[/]",
            val => {
                Action action = val switch {
                    // "Voltage Divider" => VoltageDivider.VoltageDividerMenu,
                    "Current Divider" => CurrentDivider.CurrentDividerMenu,
                    "Back" => Program.Main,
                    _ => () => { }
                };
                action();
            }, ["Voltage Divider", "Current Divider", "Back"]
        ).Build();
    }
    

}