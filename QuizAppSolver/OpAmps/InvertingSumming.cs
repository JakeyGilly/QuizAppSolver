using QuizAppSolver.Utils;
using Spectre.Console;

namespace QuizAppSolver.OpAmps;

public class InvertingSumming {
    public static void InvertingSummingMenu() {
        new UserInputBuilder().AddSelection("Select the [green]inverting summing amplifier type[/]",
            val => {
                    Action action = val switch {
                        "Voltage" => InvertingSummingVoltage,
                        "Missing One Voltage Source" => InvertingSummingMissingOneSource,
                        "Missing Rf" => InvertingSummingMissingRf,
                        "Missing One Rin" => InvertingSummingMissingOneRin,
                        "Configure Resistors (E12)" => InvertingSummingConfigureResistors,
                        "Back" => OpAmps.OpAmp,
                        _ => () => { }
                    };
                    action();
                }, ["Voltage", "Missing One Voltage Source", "Missing Rf", "Missing One Rin", "Configure Resistors (E12)", "Back"]
            ).Build();
    }
    
    private static void InvertingSummingVoltage() {
        // assume 2 inputs
        bool gainMode = false;
        double feedbackResistor = 0, saturation = 0;
        List<double> inputResistors = [];
        List<double> inputVoltages = [];
        
        new UserInputBuilder()
            .AddSelection("Do you need to calculate the [green]gain[/] or the [green]output voltage[/]?", 
                val => gainMode = val, 
                new Dictionary<bool, string> { { true, "Gain" }, { false, "Output Voltage" } })
            .AddMultipleVoltageInput("input", 2, val => inputVoltages.AddRange(val.Select(v => v.Real)), condition: () => !gainMode)
            .AddMultipleResistorInput("input", 2, val => inputResistors = val)
            .AddResistorInput("feedback", val => feedbackResistor = val)
            .AddVoltageInput("saturation", val => saturation = val.Real, condition: () => !gainMode)
            .Build();

        for (int i = 0; i < 2; i++) {
            var gain = OpAmpUtils.CalculateGain(feedbackResistor, inputResistors[i], true);
            AnsiConsole.WriteLine($"The gain for input {i + 1} is {UnitConverter.ConvertToUnit(gain)}");
        }
        var A = -(1 / inputResistors[0]) * feedbackResistor;
        var B = -(1 / inputResistors[1]) * feedbackResistor;
        AnsiConsole.WriteLine($"The output in terms of the inputs A and B is {A}A + {B}B");
        if (gainMode) {
            var menuGain = AnsiConsole.Confirm("Back to the main menu");
            if (menuGain) OpAmps.OpAmp();
        }

        var resistanceWeightedVoltages = 0.0;
        for (int i = 0; i < 2; i++) {
            resistanceWeightedVoltages += inputVoltages[i] / inputResistors[i];
        }
        var output = -feedbackResistor * resistanceWeightedVoltages;
        output = Math.Clamp(output, -saturation, saturation);
        
        AnsiConsole.WriteLine($"The output voltage is {UnitConverter.ConvertToUnit(output)}V");
        bool menu = AnsiConsole.Confirm("Back to the main menu");
        if (menu) OpAmps.OpAmp();
    }
    
    private static void InvertingSummingMissingOneSource() {
        // assume 2 inputs
        double inputVoltage = 0, feedbackResistor = 0, outputVoltage = 0;
        List<double> inputResistors = [];
        new UserInputBuilder()
            .AddVoltageInput("input", val => inputVoltage = val.Real, postfix: "(the given voltage source)")
            .AddMultipleResistorInput("input", 2, val => inputResistors = val, postfix: "(starting from the given voltage source)")
            .AddResistorInput("feedback", val => feedbackResistor = val)
            .AddVoltageInput("output", val => outputVoltage = val.Real)
            .Build();
        
        var voltageSource = ((-outputVoltage/feedbackResistor) - (inputVoltage/inputResistors[0])) * inputResistors[1];
        for (int i = 0; i < 2; i++) {
            var gain = OpAmpUtils.CalculateGain(feedbackResistor, inputResistors[i], true);
            AnsiConsole.WriteLine($"The gain for input {i + 1} is {UnitConverter.ConvertToUnit(gain)}");
        }
        AnsiConsole.WriteLine($"The missing input voltage is {UnitConverter.ConvertToUnit(voltageSource)}V");
        bool menu = AnsiConsole.Confirm("Back to the main menu");
        if (menu) OpAmps.OpAmp();
    }
    
    private static void InvertingSummingMissingRf() {
        // assume 2 inputs
        double outputVoltage = 0;
        List<double> inputVoltages = [];
        List<double> inputResistors = [];
        new UserInputBuilder()
            .AddMultipleVoltageInput("input", 2, val => inputVoltages.AddRange(val.Select(v => v.Real)))
            .AddMultipleResistorInput("input", 2, val => inputResistors = val)
            .AddVoltageInput("output", val => outputVoltage = val.Real)
            .Build();
        
        var feedbackResistor = -outputVoltage/ (inputVoltages[0] / inputResistors[0] + inputVoltages[1] / inputResistors[1]);
        
        for (int i = 0; i < 2; i++) {
            var gain = OpAmpUtils.CalculateGain(feedbackResistor, inputResistors[i], true);
            AnsiConsole.WriteLine($"The gain for input {i + 1} is {UnitConverter.ConvertToUnit(gain)}");
        }
        AnsiConsole.WriteLine($"The missing feedback resistor is {UnitConverter.ConvertToUnit(feedbackResistor)}");
        bool menu = AnsiConsole.Confirm("Back to the main menu");
        if (menu) OpAmps.OpAmp();
    }
    
    private static void InvertingSummingMissingOneRin() {
        // assume 2 inputs
        double inputResistor = 0, feedbackResistor = 0, outputVoltage = 0;
        List<double> inputVoltages = [];
        new UserInputBuilder()
            .AddMultipleVoltageInput("input", 2, val => inputVoltages.AddRange(val.Select(v => v.Real)), postfix: "(starting from the given resistor)")
            .AddResistorInput("input", val => inputResistor = val)
            .AddResistorInput("feedback", val => feedbackResistor = val)        
            .AddVoltageInput("output", val => outputVoltage = val.Real)
            .Build();
        
        var sumOfInputVoltagesDividedByInputResistance = outputVoltage / -feedbackResistor;
        var missingInputResistor = (1/(sumOfInputVoltagesDividedByInputResistance - (inputVoltages[0] / inputResistor))) * inputVoltages[1];
        
        var inputResistors = new[] { inputResistor, missingInputResistor };
        for (int i = 0; i < 2; i++) {
            var gain = OpAmpUtils.CalculateGain(feedbackResistor, inputResistors[i], true);
            AnsiConsole.WriteLine($"The gain for input {i + 1} is {UnitConverter.ConvertToUnit(gain)}");
        }
        AnsiConsole.WriteLine($"The missing input resistor is {UnitConverter.ConvertToUnit(missingInputResistor)}");
        bool menu = AnsiConsole.Confirm("Back to the main menu");
        if (menu) OpAmps.OpAmp();
    }

    private static void InvertingSummingConfigureResistors() {
        // assume 2 inputs
        var e12 = new[] { 1.0, 1.2, 1.5, 1.8, 2.2, 2.7, 3.3, 3.9, 4.7, 5.6, 6.8, 8.2 };
        var scales = new double[] { 1, 10, 100, 1000, 10000, 100000, 1000000, 10000000, 100000000, 1000000000 };
        double outputVoltage = 0;
        List<double> inputVoltages = new();
        new UserInputBuilder()
            .AddMultipleVoltageInput("input", 2, val => inputVoltages.AddRange(val.Select(v => v.Real)))
            .AddVoltageInput("output", val => outputVoltage = val.Real)
            .Build();

        var gain = outputVoltage / inputVoltages[0];

        var closestR1 = 0.0;
        var closestRf = 0.0;
        var closestDiff = double.MaxValue;

        foreach (var scale1 in scales) {
            foreach (var r1 in e12) {
                var scaledR1 = r1 * scale1;
                foreach (var scale2 in scales) {
                    foreach (var rf in e12) {
                        var scaledRf = rf * scale2;
                        var calculatedGain = -scaledRf / scaledR1;
                        if (gain - calculatedGain > 0) continue; // make sure the calculated gain is under the target gain
                        var diff = Math.Abs(calculatedGain - gain);
                        if (diff < closestDiff) {
                            closestDiff = diff;
                            closestR1 = scaledR1;
                            closestRf = scaledRf;
                        }
                    }
                }
            }
        }
        
        var voltageSource1 = inputVoltages[0] * (-closestRf / closestR1);
        var vDiff = Math.Abs(outputVoltage - voltageSource1);
        
        var R2 = (closestRf * inputVoltages[1]) / vDiff;
        
        var closestR2 = 0.0;
        var closestDiffR2 = double.MaxValue;
        
        foreach (var scale in scales) {
            foreach (var r2 in e12) {
                var scaledR2 = r2 * scale;
                if (Math.Abs(scaledR2 - R2) > closestDiffR2) continue;
                closestDiffR2 = Math.Abs(scaledR2 - R2);
                closestR2 = scaledR2;
            }
        }
        
        var output = -closestRf * (inputVoltages[0] / closestR1 + inputVoltages[1] / closestR2);
        
        AnsiConsole.WriteLine($"The gain is {UnitConverter.ConvertToUnit(gain)}");
        AnsiConsole.WriteLine($"The closest R1 is {UnitConverter.ConvertToUnit(closestR1)}");
        AnsiConsole.WriteLine($"The closest R2 is {UnitConverter.ConvertToUnit(closestR2)}");
        AnsiConsole.WriteLine($"The closest Rf is {UnitConverter.ConvertToUnit(closestRf)}");
        AnsiConsole.WriteLine($"The output voltage is {UnitConverter.ConvertToUnit(output)}V");
        bool menu = AnsiConsole.Confirm("Back to the main menu");
        if (menu) OpAmps.OpAmp();
    }
}