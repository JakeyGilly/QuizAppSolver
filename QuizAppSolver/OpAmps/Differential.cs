using Spectre.Console;

namespace QuizAppSolver.OpAmps;

public class Differential {
    public static void DifferentialMenu() {
        new UserInputBuilder().AddSelection("Select the [green]differential amplifier type[/]",
            val => {
                Action action = val switch {
                    "Voltage" => DifferentialVoltage,
                    "Missing One Voltage Source" => DifferentialMissingOneSource,
                    "Back" => OpAmps.OpAmp,
                    _ => () => { }
                };
                action();
            }, ["Voltage", "Missing One Voltage Source", "Back"]
        ).Build();
    }

    private static void DifferentialVoltage() {
        // assume 2 inputs
        // assume input resistor values are the same
        // assume feedback resistor value is the same
        double feedbackResistor = 0, inputResistor = 0;
        List<double> inputVoltages = [];
        new UserInputBuilder()
            .AddMultipleVoltageInput("input", 2, val => inputVoltages = val)
            .AddResistorInput("input", val => inputResistor = val)
            .AddResistorInput("feedback", val => feedbackResistor = val)
            .Build();
        
        var gain = feedbackResistor / inputResistor;
        var output = gain * (inputVoltages[0] - inputVoltages[1]);
        
        AnsiConsole.WriteLine($"The gain for both inputs is {UnitConverter.ConvertToUnit(gain)}");
        AnsiConsole.WriteLine($"The output voltage is {UnitConverter.ConvertToUnit(output)}V");
        bool menu = AnsiConsole.Confirm("Back to the main menu");
        if (menu) OpAmps.OpAmp();
    }

    private static void DifferentialMissingOneSource() {
        // assume 2 inputs
        // assume input resistor values are the same
        // assume feedback resistor value is the same
        double inputVoltage = 0, feedbackResistor = 0, inputResistor = 0, outputVoltage = 0;
        new UserInputBuilder()
            .AddVoltageInput("input", val => inputVoltage = val, postfix: "(the given voltage source)")
            .AddResistorInput("input", val => inputResistor = val)
            .AddResistorInput("feedback", val => feedbackResistor = val)
            .AddVoltageInput("output", val => outputVoltage = val)
            .Build();
        
        var gain = feedbackResistor / inputResistor;
        var voltageSource = (outputVoltage / (feedbackResistor / inputResistor)) + inputVoltage;
        
        AnsiConsole.WriteLine($"The gain for both inputs is {UnitConverter.ConvertToUnit(gain)}");
        AnsiConsole.WriteLine($"The missing input voltage is {UnitConverter.ConvertToUnit(voltageSource)}V");
        bool menu = AnsiConsole.Confirm("Back to the main menu");
        if (menu) OpAmps.OpAmp();
    }
}