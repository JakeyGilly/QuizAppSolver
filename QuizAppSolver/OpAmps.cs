using Spectre.Console;

namespace QuizAppSolver;

public class OpAmps {
    public static void OpAmp() {
        AnsiConsole.WriteLine();
        var opamp = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Select the [green]Op-Amp type[/]")
                .PageSize(10)
                .MoreChoicesText("[grey](Move up and down to reveal more options)[/]")
                .AddChoices("Inverting", "Non-Inverting", "Inverting-Summing", "Non-Inverting Summing", "Differential"));
        switch (opamp) {
            case "Inverting":
                Inverting();
                break;
            case "Non-Inverting":
                NonInverting();
                break;
            case "Inverting-Summing":
                InvertingSumming();
                break;
            case "Non-Inverting Summing":
                NonInvertingSumming();
                break;
            case "Differential":
                Differential();
                break;
        }
    }
    
    private static void Inverting() {
        var gainMode = AnsiConsole.Prompt(
            new SelectionPrompt<bool>()
                .Title("Do you need to calculate the [green]gain[/] or the [green]output voltage[/]?")
                .PageSize(10)
                .MoreChoicesText("[grey](Move up and down to reveal more options)[/]")
                .AddChoices(true, false)
                .UseConverter(choice => choice ? "Gain" : "Output Voltage"));
        var inputVoltageInput = "1";
        if (!gainMode) {
            inputVoltageInput = AnsiConsole.Prompt(
                new TextPrompt<string>("Enter the [green]input voltage[/]")
                    .PromptStyle("grey")
                    .DefaultValue("12")
                    .DefaultValueStyle("grey"));
        }
        var feedbackResistorInput = AnsiConsole.Prompt(
            new TextPrompt<string>("Enter the [green]feedback resistor value[/]")
                .PromptStyle("grey")
                .DefaultValue("1k")
                .DefaultValueStyle("grey"));
        var inputResistorInput = AnsiConsole.Prompt(
            new TextPrompt<string>("Enter the [green]input resistor value[/]")
                .PromptStyle("grey")
                .DefaultValue("1k")
                .DefaultValueStyle("grey"));
        var saturationInput = AnsiConsole.Prompt(
            new TextPrompt<string>("Enter the [green]saturation voltage[/]")
                .PromptStyle("grey")
                .DefaultValue("12")
                .DefaultValueStyle("grey"));
        var isFeedbackNonInverting = AnsiConsole.Prompt(
            new SelectionPrompt<bool>()
                .Title("Is the feedback resistor connected to the [green]non-inverting input[/]?")
                .PageSize(10)
                .MoreChoicesText("[grey](Move up and down to reveal more options)[/]")
                .AddChoices(true, false)
                .UseConverter(choice => choice ? "Yes" : "No"));
        var resistorToGroundInput = "";
        if (isFeedbackNonInverting) {
            resistorToGroundInput = AnsiConsole.Prompt(
                new TextPrompt<string>("Enter the [green]resistor to ground value[/]")
                    .PromptStyle("grey")
                    .DefaultValue("1k")
                    .DefaultValueStyle("grey"));
        }

        var inputVoltage = UnitConverter.ConvertUnits(inputVoltageInput);
        var feedbackResistor = UnitConverter.ConvertUnits(feedbackResistorInput);
        var inputResistor = UnitConverter.ConvertUnits(inputResistorInput);
        var saturation = UnitConverter.ConvertUnits(saturationInput);
        var resistorToGround = 0.0;
        if (isFeedbackNonInverting) {
            resistorToGround = UnitConverter.ConvertUnits(resistorToGroundInput);
        }

        var gain = -feedbackResistor / inputResistor;
        var output = gain * inputVoltage;
        if (output > saturation) {
            output = saturation;
        }
        if (output < -saturation) {
            output = -saturation;
        }
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
        AnsiConsole.WriteLine($"The gain is {UnitConverter.ConvertToUnit(gain)}");
        if (gainMode) return;
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
    }
    
    private static void NonInverting() {
        var gainMode = AnsiConsole.Prompt(
            new SelectionPrompt<bool>()
                .Title("Do you need to calculate the [green]gain[/] or the [green]output voltage[/]?")
                .PageSize(10)
                .MoreChoicesText("[grey](Move up and down to reveal more options)[/]")
                .AddChoices(true, false)
                .UseConverter(choice => choice ? "Gain" : "Output Voltage"));
        var inputVoltageInput = "1";
        if (!gainMode) {
            inputVoltageInput = AnsiConsole.Prompt(
                new TextPrompt<string>("Enter the [green]input voltage[/]")
                    .PromptStyle("grey")
                    .DefaultValue("12")
                    .DefaultValueStyle("grey"));
        }
        var feedbackResistorInput = AnsiConsole.Prompt(
            new TextPrompt<string>("Enter the [green]feedback resistor value[/]")
                .PromptStyle("grey")
                .DefaultValue("1k")
                .DefaultValueStyle("grey"));
        var inputResistorInput = AnsiConsole.Prompt(
            new TextPrompt<string>("Enter the [green]input resistor value[/]")
                .PromptStyle("grey")
                .DefaultValue("1k")
                .DefaultValueStyle("grey"));
        var saturationInput = AnsiConsole.Prompt(
            new TextPrompt<string>("Enter the [green]saturation voltage[/]")
                .PromptStyle("grey")
                .DefaultValue("12")
                .DefaultValueStyle("grey"));
        var feedbackNonInverting = AnsiConsole.Prompt(
            new SelectionPrompt<bool>()
                .Title("Is the feedback resistor connected to the [green]non-inverting input[/]?")
                .PageSize(10)
                .MoreChoicesText("[grey](Move up and down to reveal more options)[/]")
                .AddChoices(true, false)
                .UseConverter(choice => choice ? "Yes" : "No"));
        
        var inputVoltage = UnitConverter.ConvertUnits(inputVoltageInput);
        var feedbackResistor = UnitConverter.ConvertUnits(feedbackResistorInput);
        var inputResistor = UnitConverter.ConvertUnits(inputResistorInput);
        var saturation = UnitConverter.ConvertUnits(saturationInput);
        
        var gain = 1 + feedbackResistor / inputResistor;
        var output = gain * inputVoltage;
        if (output > saturation) {
            output = saturation;
        }
        if (output < -saturation) {
            output = -saturation;
        }
        var satPositive = false;
        var satNegative = false;
        if (feedbackNonInverting) {
            output = saturation;
            var voltageAcrossDivider = saturation - inputVoltage;
            var current = voltageAcrossDivider / (inputResistor + feedbackResistor);
            var voltageAtNonInverting = inputVoltage + current * inputResistor;
            var Vd = voltageAtNonInverting;
            if (Vd > 0) {
                satPositive = true;
            }
            voltageAcrossDivider = double.Abs(-saturation - inputVoltage);
            current = voltageAcrossDivider / (inputResistor + feedbackResistor);
            var voltageAtNonInvertingNegative = inputVoltage - current * inputResistor;
            Vd = voltageAtNonInvertingNegative;
            if (Vd < 0) {
                satNegative = true;
            }
        }
        
        AnsiConsole.WriteLine($"The gain is {UnitConverter.ConvertToUnit(gain)}");
        if (gainMode) return;
        switch (feedbackNonInverting) {
            case true:
                char sign = satPositive && satNegative ? '±' : satPositive ? '+' : '-';
                AnsiConsole.WriteLine($"The output voltage is {sign}{UnitConverter.ConvertToUnit(output)}V");
                break;
            case false:
                AnsiConsole.WriteLine($"The output voltage is {UnitConverter.ConvertToUnit(output)}V");
                break;
        }
        if (!feedbackNonInverting) return;
        if (satPositive && satNegative) {
            AnsiConsole.WriteLine("The op-amp is saturated in both directions");
        } else if (satPositive) {
            AnsiConsole.WriteLine("The op-amp is saturated in the positive direction");
        } else if (satNegative) {
            AnsiConsole.WriteLine("The op-amp is saturated in the negative direction");
        }
    }
    
    private static void InvertingSumming() {
        var option = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Select the type of [green]inverting summing amplifier[/]")
                .PageSize(10)
                .MoreChoicesText("[grey](Move up and down to reveal more options)[/]")
                .AddChoices("Voltage", "Missing One Voltage Source", "Configure Resistors (E12)"));
        switch (option) {
            case "Voltage":
                InvertingSummingVoltage();
                break;
            case "Missing One Voltage Source":
                InvertingSummingMissingOneSource();
                break;
            case "Configure Resistors (E12)":
                InvertingSummingConfigureResistors();
                break;
        }
    }
    
    private static void InvertingSummingVoltage() {
        // assume 2 inputs
        var inputVoltagesInputs = new string[2];
        for (int i = 0; i < 2; i++) {
            inputVoltagesInputs[i] = AnsiConsole.Prompt(
                new TextPrompt<string>($"Enter [green]input voltage {i + 1}[/]")
                    .PromptStyle("grey")
                    .DefaultValue("12")
                    .DefaultValueStyle("grey"));
        }
        var inputResistorsInputs = new string[2];
        for (int i = 0; i < 2; i++) {
            inputResistorsInputs[i] = AnsiConsole.Prompt(
                new TextPrompt<string>($"Enter [green]input resistor {i + 1} value[/]")
                    .PromptStyle("grey")
                    .DefaultValue("1k")
                    .DefaultValueStyle("grey"));
        }
        var feedbackResistorInput = AnsiConsole.Prompt(
            new TextPrompt<string>("Enter the [green]feedback resistor value[/]")
                .PromptStyle("grey")
                .DefaultValue("1k")
                .DefaultValueStyle("grey"));
        var saturationInput = AnsiConsole.Prompt(
            new TextPrompt<string>("Enter the [green]saturation voltage[/]")
                .PromptStyle("grey")
                .DefaultValue("12")
                .DefaultValueStyle("grey"));
        
        var inputs = new double[2];
        for (int i = 0; i < 2; i++) {
            inputs[i] = UnitConverter.ConvertUnits(inputVoltagesInputs[i]);
        }
        var feedback = UnitConverter.ConvertUnits(feedbackResistorInput);
        var inputResistors = new double[2];
        for (int i = 0; i < 2; i++) {
            inputResistors[i] = UnitConverter.ConvertUnits(inputResistorsInputs[i]);
        }
        var saturation = UnitConverter.ConvertUnits(saturationInput);
        
        var resistanceWeightedVoltages = 0.0;
        for (int i = 0; i < 2; i++) {
            resistanceWeightedVoltages += inputs[i] / inputResistors[i];
        }
        var output = -feedback * resistanceWeightedVoltages;
        if (output > saturation) {
            output = saturation;
        }
        if (output < -saturation) {
            output = -saturation;
        }
        
        for (int i = 0; i < 2; i++) {
            var gain = -feedback / inputResistors[i];
            AnsiConsole.WriteLine($"The gain for input {i + 1} is {UnitConverter.ConvertToUnit(gain)}");
        }
        AnsiConsole.WriteLine($"The output voltage is {UnitConverter.ConvertToUnit(output)}V");
    }
    
    private static void InvertingSummingMissingOneSource() {
        // assume 2 inputs
        var inputVoltageInput = AnsiConsole.Prompt(
            new TextPrompt<string>($"Enter [green]input voltage of the given voltage source[/]")
                .PromptStyle("grey")
                .DefaultValue("12")
                .DefaultValueStyle("grey"));
        var inputResistorsInputs = new string[2];
        for (int i = 0; i < 2; i++) {
            inputResistorsInputs[i] = AnsiConsole.Prompt(
                new TextPrompt<string>($"Enter [green]input resistor {i + 1} value starting from the given voltage source[/]")
                    .PromptStyle("grey")
                    .DefaultValue("1k")
                    .DefaultValueStyle("grey"));
        }
        var feedbackResistorInput = AnsiConsole.Prompt(
            new TextPrompt<string>("Enter the [green]feedback resistor value[/]")
                .PromptStyle("grey")
                .DefaultValue("1k")
                .DefaultValueStyle("grey"));
        var outputVoltageInput = AnsiConsole.Prompt(
            new TextPrompt<string>("Enter the [green]output voltage[/]")
                .PromptStyle("grey")
                .DefaultValue("12")
                .DefaultValueStyle("grey"));
        
        var inputVoltage = UnitConverter.ConvertUnits(inputVoltageInput);
        var feedback = UnitConverter.ConvertUnits(feedbackResistorInput);
        var inputResistors = new double[2];
        for (int i = 0; i < 2; i++) {
            inputResistors[i] = UnitConverter.ConvertUnits(inputResistorsInputs[i]);
        }
        var outputVoltage = UnitConverter.ConvertUnits(outputVoltageInput);
        
        var voltageSource = ((-outputVoltage/feedback) - (inputVoltage/inputResistors[0])) * inputResistors[1];
        
        for (int i = 0; i < 2; i++) {
            var gain = -feedback / inputResistors[i];
            AnsiConsole.WriteLine($"The gain for input {i + 1} is {UnitConverter.ConvertToUnit(gain)}");
        }
        AnsiConsole.WriteLine($"The missing input voltage is {UnitConverter.ConvertToUnit(voltageSource)}V");
    }

    private static void InvertingSummingConfigureResistors()
    {
        // assume 2 inputs
        var e12 = new double[] { 1.0, 1.2, 1.5, 1.8, 2.2, 2.7, 3.3, 3.9, 4.7, 5.6, 6.8, 8.2 };
        var scales = new double[] { 1, 10, 100, 1000, 10000, 100000, 1000000, 10000000, 100000000, 1000000000 };
        var inputVoltagesInputs = new string[2];
        for (int i = 0; i < 2; i++) {
            inputVoltagesInputs[i] = AnsiConsole.Prompt(
                new TextPrompt<string>("Enter the [green]input voltage[/]")
                    .PromptStyle("grey")
                    .DefaultValue("12")
                    .DefaultValueStyle("grey"));
        }
        var outputVoltageInput = AnsiConsole.Prompt(
            new TextPrompt<string>("Enter the [green]output voltage[/]")
                .PromptStyle("grey")
                .DefaultValue("12")
                .DefaultValueStyle("grey"));

        var inputVoltages = new double[2];
        for (int i = 0; i < 2; i++) {
            inputVoltages[i] = UnitConverter.ConvertUnits(inputVoltagesInputs[i]);
        }
        var outputVoltage = UnitConverter.ConvertUnits(outputVoltageInput);

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
        
        AnsiConsole.WriteLine($"The gain is {UnitConverter.ConvertToUnit(gain)}");
        AnsiConsole.WriteLine($"The closest R1 is {UnitConverter.ConvertToUnit(closestR1)}");
        AnsiConsole.WriteLine($"The closest R2 is {UnitConverter.ConvertToUnit(closestR2)}");
        AnsiConsole.WriteLine($"The closest Rf is {UnitConverter.ConvertToUnit(closestRf)}");
    }



    private static void Differential() {
        var option = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Select the type of [green]differential amplifier[/]")
                .PageSize(10)
                .MoreChoicesText("[grey](Move up and down to reveal more options)[/]")
                .AddChoices("Voltage", "Missing One Voltage Source"));
        switch (option) {
            case "Voltage":
                DifferntialVoltage();
                break;
            case "Missing One Voltage Source":
                DifferentialMissingOneSource();
                break;
        }
    }

    private static void DifferntialVoltage() {
        // assume 2 inputs
        // assume input resistor values are the same
        // assume feedback resistor value is the same
        var inputVoltagesInputs = new string[2];
        for (int i = 0; i < 2; i++) {
            inputVoltagesInputs[i] = AnsiConsole.Prompt(
                new TextPrompt<string>($"Enter [green]input voltage {i + 1}[/]")
                    .PromptStyle("grey")
                    .DefaultValue("12")
                    .DefaultValueStyle("grey"));
        }
        var feedbackResistorInput = AnsiConsole.Prompt(
            new TextPrompt<string>("Enter the [green]feedback resistors value[/]")
                .PromptStyle("grey")
                .DefaultValue("1k")
                .DefaultValueStyle("grey"));
        var inputResistorInput = AnsiConsole.Prompt(
            new TextPrompt<string>($"Enter [green]input resistors value[/]")
                .PromptStyle("grey")
                .DefaultValue("1k")
                .DefaultValueStyle("grey"));
        
        var inputVoltages = new double[2];
        for (int i = 0; i < 2; i++) {
            inputVoltages[i] = UnitConverter.ConvertUnits(inputVoltagesInputs[i]);
        }
        var feedbackResistor = UnitConverter.ConvertUnits(feedbackResistorInput);
        var inputResistor = UnitConverter.ConvertUnits(inputResistorInput);
        
        var gain = feedbackResistor / inputResistor;
        var output = gain * (inputVoltages[0] - inputVoltages[1]);
        
        AnsiConsole.WriteLine($"The gain for both inputs is {UnitConverter.ConvertToUnit(gain)}");
        AnsiConsole.WriteLine($"The output voltage is {UnitConverter.ConvertToUnit(output)}V");
    }

    private static void DifferentialMissingOneSource() {
        // assume 2 inputs
        // assume input resistor values are the same
        // assume feedback resistor value is the same
        var inputVoltageInput = AnsiConsole.Prompt(
            new TextPrompt<string>($"Enter [green]input voltage of the given voltage source[/]")
                .PromptStyle("grey")
                .DefaultValue("12")
                .DefaultValueStyle("grey"));
        var feedbackResistorInput = AnsiConsole.Prompt(
            new TextPrompt<string>("Enter the [green]feedback resistors value[/]")
                .PromptStyle("grey")
                .DefaultValue("1k")
                .DefaultValueStyle("grey"));
        var inputResistorInput = AnsiConsole.Prompt(
            new TextPrompt<string>($"Enter [green]input resistors value[/]")
                .PromptStyle("grey")
                .DefaultValue("1k")
                .DefaultValueStyle("grey"));
        var outputVoltageInput = AnsiConsole.Prompt(
            new TextPrompt<string>("Enter the [green]output voltage[/]")
                .PromptStyle("grey")
                .DefaultValue("12")
                .DefaultValueStyle("grey"));
        
        var inputVoltage = UnitConverter.ConvertUnits(inputVoltageInput);
        var feedbackResistor = UnitConverter.ConvertUnits(feedbackResistorInput);
        var inputResistor = UnitConverter.ConvertUnits(inputResistorInput);
        var outputVoltage = UnitConverter.ConvertUnits(outputVoltageInput);
        
        var gain = feedbackResistor / inputResistor;
        var voltageSource = (outputVoltage / (feedbackResistor / inputResistor)) + inputVoltage;
        
        AnsiConsole.WriteLine($"The gain for both inputs is {UnitConverter.ConvertToUnit(gain)}");
        AnsiConsole.WriteLine($"The missing input voltage is {UnitConverter.ConvertToUnit(voltageSource)}V");
    }

    private static void NonInvertingSumming() {
        // assume 2 inputs
        var inputVoltagesInputs = new string[2];
        for (int i = 0; i < 2; i++) {
            inputVoltagesInputs[i] = AnsiConsole.Prompt(
                new TextPrompt<string>($"Enter [green]input voltage {i + 1}[/]")
                    .PromptStyle("grey")
                    .DefaultValue("12")
                    .DefaultValueStyle("grey"));
        }
        var inputResistorsInputs = new string[2];
        for (int i = 0; i < 2; i++) {
            inputResistorsInputs[i] = AnsiConsole.Prompt(
                new TextPrompt<string>($"Enter [green]input resistor {i + 1} value[/]")
                    .PromptStyle("grey")
                    .DefaultValue("1k")
                    .DefaultValueStyle("grey"));
        }
        var feedbackResistanceInput = AnsiConsole.Prompt(
            new TextPrompt<string>("Enter the [green]feedback resistor value[/]")
                .PromptStyle("grey")
                .DefaultValue("1k")
                .DefaultValueStyle("grey"));
        var groundResistanceInput = AnsiConsole.Prompt(
            new TextPrompt<string>("Enter the [green]ground resistor value[/]")
                .PromptStyle("grey")
                .DefaultValue("1k")
                .DefaultValueStyle("grey"));
        var saturationInput = AnsiConsole.Prompt(
            new TextPrompt<string>("Enter the [green]saturation voltage[/]")
                .PromptStyle("grey")
                .DefaultValue("12")
                .DefaultValueStyle("grey"));
        
        var inputVoltages = new double[2];
        for (int i = 0; i < 2; i++) {
            inputVoltages[i] = UnitConverter.ConvertUnits(inputVoltagesInputs[i]);
        }
        var feedbackResistance = UnitConverter.ConvertUnits(feedbackResistanceInput);
        var groundResistance = UnitConverter.ConvertUnits(groundResistanceInput);
        var inputResistors = new double[2];
        for (int i = 0; i < 2; i++) {
            inputResistors[i] = UnitConverter.ConvertUnits(inputResistorsInputs[i]);
        }
        var saturation = UnitConverter.ConvertUnits(saturationInput);

        var gain = 1 + (feedbackResistance / groundResistance);
        var output = ((inputVoltages[0]*inputResistors[0] + inputVoltages[1]*inputResistors[1]) / (inputResistors[0] + inputResistors[1])) * gain;
        
        if (output > saturation) {
            output = saturation;
        }
        if (output < -saturation) {
            output = -saturation;
        }
        
        AnsiConsole.WriteLine($"The gain is {UnitConverter.ConvertToUnit(gain)}");
        AnsiConsole.WriteLine($"The output voltage is {UnitConverter.ConvertToUnit(output)}V");
    }
}