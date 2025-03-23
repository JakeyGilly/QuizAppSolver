using QuizAppSolver.Utils;
using Spectre.Console;

namespace QuizAppSolver.OpAmps;

public class Inverting {
    public static void InvertingMenu() {
        new UserInputBuilder().AddSelection("Select the [green]inverting amplifier type[/]",
            val => {
                Action action = val switch {
                    "Voltage" => InvertingVoltage,
                    "Configure Resistors (E12)" => InvertingConfigureResistors,
                    "Back" => OpAmps.OpAmp,
                    _ => () => { }
                };
                action();
            }, ["Voltage", "Configure Resistors (E12)", "Back"]
        ).Build();
    }

    private static void InvertingVoltage() {
        bool gainMode = false, isFeedbackNonInverting = false;
        double inputVoltage = 0, feedbackResistor = 0, resistorToGround = 0, inputResistor = 0, saturation = 0;
        new UserInputBuilder()
            .AddSelection("Do you need to calculate the [green]gain[/] or the [green]output voltage[/]?", 
                val => gainMode = val, 
                new Dictionary<bool, string> { { true, "Gain" }, { false, "Output Voltage" } })
            .AddVoltageInput("input", val => inputVoltage = val.Real)
            .AddResistorInput("input", val => inputResistor = val)
            .AddResistorInput("feedback", val => feedbackResistor = val)
            .AddVoltageInput("saturation", val => saturation = val.Real, condition: () => !gainMode)
            .AddSelection("Is the feedback resistor connected to the [green]non-inverting input[/]?",
                val => isFeedbackNonInverting = val, 
                new Dictionary<bool, string> { { true, "Yes" }, { false, "No" } }, 
                () => !gainMode)
            .AddResistorInput("ground", val => resistorToGround = val, condition: () => isFeedbackNonInverting)
            .Build();

        var gain = OpAmpUtils.CalculateGain(feedbackResistor, inputResistor, true);
        AnsiConsole.WriteLine($"The gain is {UnitConverter.ConvertToUnit(gain)}");
        if (gainMode) {
            var menuGain = AnsiConsole.Confirm("Back to the main menu");
            if (menuGain) OpAmps.OpAmp();
        }
        
        var output = gain * inputVoltage;
        output = Math.Clamp(output, -saturation, saturation);
        var satPositive = false;
        var satNegative = false;
        if (isFeedbackNonInverting) {
            output = saturation;
            var nonInvertingInput = saturation * resistorToGround / (feedbackResistor + resistorToGround);
            var Vd = nonInvertingInput - inputVoltage;
            if (Vd > 0) {
                satPositive = true;
            }
            nonInvertingInput = -saturation * resistorToGround / (feedbackResistor + resistorToGround);
            Vd = nonInvertingInput - inputVoltage;
            if (Vd < 0) {
                satNegative = true;
            }
        }
        switch (isFeedbackNonInverting) {
            case true:
                char sign = satPositive && satNegative ? '±' : satPositive ? '+' : '-';
                AnsiConsole.WriteLine($"The output voltage is {sign}{UnitConverter.ConvertToUnit(output)}V");
                break;
            case false:
                AnsiConsole.WriteLine($"The output voltage is {UnitConverter.ConvertToUnit(output)}V");
                break;
        }
        if (!isFeedbackNonInverting) return;
        if (satPositive && satNegative) {
            AnsiConsole.WriteLine("The op-amp is saturated in both directions");
        } else if (satPositive) {
            AnsiConsole.WriteLine("The op-amp is saturated in the positive direction");
        } else if (satNegative) {
            AnsiConsole.WriteLine("The op-amp is saturated in the negative direction");
        }
        bool menu = AnsiConsole.Confirm("Back to the main menu");
        if (menu) OpAmps.OpAmp();
    }

    private static void InvertingConfigureResistors() {
        var e12 = new double[] { 1.0, 1.2, 1.5, 1.8, 2.2, 2.7, 3.3, 3.9, 4.7, 5.6, 6.8, 8.2 };
        var scales = new double[] { 1, 10, 100, 1000, 10000, 100000, 1000000, 10000000, 100000000, 1000000000 };
        double inputVoltage = 0, outputVoltage = 0;
        new UserInputBuilder()
            .AddVoltageInput("input", val => inputVoltage = val.Real)
            .AddVoltageInput("output", val => outputVoltage = val.Real)
            .Build();
        
        double targetRatio = -(outputVoltage / inputVoltage);
        
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
        
        var gain = OpAmpUtils.CalculateGain(closestRf, closestRg, true);
        var output = inputVoltage * gain;

        AnsiConsole.WriteLine($"The gain is {UnitConverter.ConvertToUnit(gain)}");
        AnsiConsole.WriteLine($"The closest Rf is {UnitConverter.ConvertToUnit(closestRf)}");
        AnsiConsole.WriteLine($"The closest Rg is {UnitConverter.ConvertToUnit(closestRg)}");
        AnsiConsole.WriteLine($"The output voltage is {UnitConverter.ConvertToUnit(output)}V");
        bool menu = AnsiConsole.Confirm("Back to the main menu");
        if (menu) OpAmps.OpAmp();
    }
}