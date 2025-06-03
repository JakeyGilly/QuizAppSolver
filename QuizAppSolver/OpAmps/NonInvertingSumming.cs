using QuizAppSolver.Utils;
using Spectre.Console;

namespace QuizAppSolver.OpAmps;

public class NonInvertingSumming {
    public static void NonInvertingSummingMenu() {
        new UserInputBuilder().AddSelection("Select the [green]non-inverting amplifier type[/]",
            val => {
                    Action action = val switch {
                        "Voltage" => NonInvertingSummingVoltage,
                        "Missing One Voltage Source" => NonInvertingSummingMissingSource,
                        "Missing Rf" => NonInvertingSummingMissingRf,
                        "Missing Rg" => NonInvertingSummingMissingRg,
                        "Configure Resistors (E12)" => NonInvertingSummingConfigureResistors,
                        "Back" => OpAmps.OpAmp,
                        _ => () => { }
                    };
                    action();
                }, ["Voltage", "Missing One Voltage Source", "Missing Rf", "Missing Rg", "Configure Resistors (E12)", "Back"]
            ).Build();
    }
    private static void NonInvertingSummingVoltage() {
        // assume 2 inputs
        var gainMode = false;
        double feedbackResistance = 0, groundResistance = 0, saturation = 0;
        List<double> inputResistors = [];
        List<double> inputVoltages = [];
        new UserInputBuilder()
            .AddSelection("Do you need to calculate the [green]gain[/] or the [green]output voltage[/]?", 
                val => gainMode = val, 
                new Dictionary<bool, string> { { true, "Gain" }, { false, "Output Voltage" } })
            .AddMultipleVoltageInput("input", 2, val => inputVoltages.AddRange(val.Select(v => v.Real)), condition: () => !gainMode)
            .AddMultipleResistorInput("input", 2, val => inputResistors = val)
            .AddResistorInput("feedback", val => feedbackResistance = val)
            .AddResistorInput("ground", val => groundResistance = val)
            .AddVoltageInput("saturation", val => saturation = val.Real, condition: () => !gainMode)
            .Build();
        
        var gain = OpAmpUtils.CalculateGain(feedbackResistance, groundResistance, false);
        var A = gain * (inputResistors[1] / (inputResistors[0] + inputResistors[1]));
        var B = gain * (inputResistors[0] / (inputResistors[0] + inputResistors[1]));
        AnsiConsole.WriteLine($"The gain is {UnitConverter.ConvertToUnit(gain)}");
        AnsiConsole.WriteLine($"The output in terms of the inputs A and B is {A}A + {B}B");
        if (gainMode) {
            var menuGain = AnsiConsole.Confirm("Back to the main menu");
            if (menuGain) OpAmps.OpAmp();
        }
        
        var output = (inputVoltages[0]*inputResistors[0] + inputVoltages[1]*inputResistors[1]) / (inputResistors[0] + inputResistors[1]) * gain;
        Math.Clamp(output, -saturation, saturation);
        
        AnsiConsole.WriteLine($"The output voltage is {UnitConverter.ConvertToUnit(output)}V");
        bool menu = AnsiConsole.Confirm("Back to the main menu");
        if (menu) OpAmps.OpAmp();
    }

    private static void NonInvertingSummingMissingSource() {
        // assume 2 inputs
        double inputVoltage = 0, feedbackResistor = 0, groundResistor = 0, outputVoltage = 0;
        List<double> inputResistors = [];
        new UserInputBuilder()
            .AddVoltageInput("input", val => inputVoltage = val.Real, postfix: "(the given voltage source)")
            .AddMultipleResistorInput("input", 2, val => inputResistors = val, postfix: "(starting from the given voltage source)")
            .AddResistorInput("feedback", val => feedbackResistor = val)
            .AddResistorInput("ground", val => groundResistor = val)
            .AddVoltageInput("output", val => outputVoltage = val.Real)
            .Build();
        
        var gain = OpAmpUtils.CalculateGain(feedbackResistor, groundResistor, false);
        var potentialDividerRatio = inputResistors[0] / (inputResistors[0] + inputResistors[1]);
        var voltageSource = (outputVoltage / (potentialDividerRatio * gain)) + inputVoltage;
        
        AnsiConsole.WriteLine($"The gain is {UnitConverter.ConvertToUnit(gain)}");
        AnsiConsole.WriteLine($"The missing input voltage is {UnitConverter.ConvertToUnit(voltageSource)}V");
        bool menu = AnsiConsole.Confirm("Back to the main menu");
        if (menu) OpAmps.OpAmp();
    }
    
    private static void NonInvertingSummingMissingRf() {
        // assume 2 inputs
        double groundResistor = 0, outputVoltage = 0;
        List<double> inputVoltages = [];
        List<double> inputResistors = [];
        new UserInputBuilder()
            .AddMultipleVoltageInput("input", 2, val => inputVoltages.AddRange(val.Select(v => v.Real)))
            .AddMultipleResistorInput("input", 2, val => inputResistors = val)
            .AddResistorInput("ground", val => groundResistor = val)
            .AddVoltageInput("output", val => outputVoltage = val.Real)
            .Build();
        
        var sumVoltage = 0.0;
        var sumResistances = 0.0;
        for (int i = 0; i < 2; i++) {
            sumVoltage += inputVoltages[i] / inputResistors[i];
            sumResistances += 1 / inputResistors[i];
        }
        var nonInvertingInput = sumVoltage / sumResistances;
        var feedbackResistor = (outputVoltage / nonInvertingInput - 1) * groundResistor;
        var gain = OpAmpUtils.CalculateGain(feedbackResistor, groundResistor, false);
        
        AnsiConsole.WriteLine($"The gain is {UnitConverter.ConvertToUnit(gain)}");
        AnsiConsole.WriteLine($"The missing feedback resistor is {UnitConverter.ConvertToUnit(feedbackResistor)}");
        bool menu = AnsiConsole.Confirm("Back to the main menu");
        if (menu) OpAmps.OpAmp();
    }
    
    private static void NonInvertingSummingMissingRg() {
        // assume 2 inputs
        double feedbackResistor = 0, outputVoltage = 0;
        List<double> inputVoltages = [];
        List<double> inputResistors = [];
        new UserInputBuilder()
            .AddMultipleVoltageInput("input", 2, val => inputVoltages.AddRange(val.Select(v => v.Real)))
            .AddMultipleResistorInput("input", 2, val => inputResistors = val)
            .AddResistorInput("feedback", val => feedbackResistor = val)
            .AddVoltageInput("output", val => outputVoltage = val.Real)
            .Build();
        
        var sumVoltage = 0.0;
        var sumResistances = 0.0;
        for (int i = 0; i < 2; i++) {
            sumVoltage += inputVoltages[i] / inputResistors[i];
            sumResistances += 1 / inputResistors[i];
        }
        var nonInvertingInput = sumVoltage / sumResistances;
        var gain = outputVoltage / nonInvertingInput;
        var groundResistor = feedbackResistor / (gain - 1);
        
        AnsiConsole.WriteLine($"The gain is {UnitConverter.ConvertToUnit(gain)}");
        AnsiConsole.WriteLine($"The missing ground resistor is {UnitConverter.ConvertToUnit(groundResistor)}");
        bool menu = AnsiConsole.Confirm("Back to the main menu");
        if (menu) OpAmps.OpAmp();
    }

    private static void NonInvertingSummingConfigureResistors() {
        // assume 2 inputs
        var e12 = new double[] { 1.0, 1.2, 1.5, 1.8, 2.2, 2.7, 3.3, 3.9, 4.7, 5.6, 6.8, 8.2 };
        var scales = new double[] { 1, 10, 100, 1000, 10000, 100000, 1000000, 10000000, 100000000, 1000000000 };
        double outputVoltage = 0;
        List<double> inputVoltages = [];
        new UserInputBuilder()
            .AddMultipleVoltageInput("input", 2, val => inputVoltages.AddRange(val.Select(v => v.Real)))
            .AddVoltageInput("output", val => outputVoltage = val.Real)
            .Build();

        inputVoltages.Sort();
        inputVoltages.Reverse();
        
        var R1 = double.MaxValue;
        var R2 = 1000.0;
        var nonInvertingInput = (inputVoltages[1] - inputVoltages[0]) * R2 / (R1 + R2) + inputVoltages[0];
        
        double targetRatio = (outputVoltage / nonInvertingInput) - 1;
        
        var closestRf = 0.0;
        var closestRg = 0.0;
        var closestDiff = double.MaxValue;
        
        foreach (var scale1 in scales) {
            foreach (var rf in e12) {
                double scaledRf = rf * scale1;
                foreach (var scale2 in scales) {
                    foreach (var rg in e12) {
                        var scaledRg = rg * scale2;
                        var calculatedRatio = scaledRf / scaledRg;
                        if (targetRatio - calculatedRatio > 0) continue; // make sure the calculated ratio is under the target ratio
                        var diff = Math.Abs(calculatedRatio - targetRatio);
                        if (diff < closestDiff) {
                            closestDiff = diff;
                            closestRf = scaledRf;
                            closestRg = scaledRg;
                        }
                    }
                }
            }
        }
        
        var gain = OpAmpUtils.CalculateGain(closestRf, closestRg, false);
        AnsiConsole.WriteLine($"The gain is {UnitConverter.ConvertToUnit(gain)}");
        
        var output = nonInvertingInput * gain;
        var nonInvertingInputNeeded = outputVoltage / (1+closestRf/closestRg);
        R1 = (nonInvertingInputNeeded-inputVoltages[1]) * R2 / ((inputVoltages[0] - inputVoltages[1]) - (nonInvertingInputNeeded - inputVoltages[1]));
        
        var closestR1 = 0.0;
        var closestDiffR1 = double.MaxValue;
        foreach (var scale in scales) {
            foreach (var r1 in e12) {
                var scaledR1 = r1 * scale;
                if (Math.Abs(scaledR1 - R1) > closestDiffR1) continue;
                closestDiffR1 = Math.Abs(scaledR1 - R1);
                closestR1 = scaledR1;
            }
        }
        output = ((inputVoltages[1] - inputVoltages[0]) * R2 / (closestR1 + R2) + inputVoltages[0]) * gain;
        
        AnsiConsole.WriteLine($"The closest Rf resistance is {UnitConverter.ConvertToUnit(closestRf)}");
        AnsiConsole.WriteLine($"The closest Rg resistance is {UnitConverter.ConvertToUnit(closestRg)}");
        AnsiConsole.WriteLine($"The closest R1 resistance (connected to the {UnitConverter.ConvertToUnit(inputVoltages[1])}V input) is {UnitConverter.ConvertToUnit(closestR1)}");
        AnsiConsole.WriteLine($"The closest R2 resistance (connected to the {UnitConverter.ConvertToUnit(inputVoltages[0])}V input) is {UnitConverter.ConvertToUnit(R2)}");
        AnsiConsole.WriteLine($"The output voltage is {UnitConverter.ConvertToUnit(output)}V");
        bool menu = AnsiConsole.Confirm("Back to the main menu");
        if (menu) OpAmps.OpAmp();
    }
}