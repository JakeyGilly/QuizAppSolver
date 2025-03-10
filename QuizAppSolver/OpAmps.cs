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
        var inputVoltageInput = "";
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
        var saturationInput = "";
        var isFeedbackNonInverting = false;
        var resistorToGroundInput = "";
        if (!gainMode) {
            saturationInput = AnsiConsole.Prompt(
                new TextPrompt<string>("Enter the [green]saturation voltage[/]")
                    .PromptStyle("grey")
                    .DefaultValue("12")
                    .DefaultValueStyle("grey"));
            isFeedbackNonInverting = AnsiConsole.Prompt(
                new SelectionPrompt<bool>()
                    .Title("Is the feedback resistor connected to the [green]non-inverting input[/]?")
                    .PageSize(10)
                    .MoreChoicesText("[grey](Move up and down to reveal more options)[/]")
                    .AddChoices(true, false)
                    .UseConverter(choice => choice ? "Yes" : "No"));
            if (isFeedbackNonInverting) {
                resistorToGroundInput = AnsiConsole.Prompt(
                    new TextPrompt<string>("Enter the [green]resistor to ground value[/]")
                        .PromptStyle("grey")
                        .DefaultValue("1k")
                        .DefaultValueStyle("grey"));
            }
        }
        
        var inputVoltage = 0.0;
        if (!gainMode) {
            inputVoltage = UnitConverter.ConvertUnits(inputVoltageInput);
        }
        var feedbackResistor = UnitConverter.ConvertUnits(feedbackResistorInput);
        var inputResistor = UnitConverter.ConvertUnits(inputResistorInput);
        var saturation = 0.0;
        if (!gainMode) {
            saturation = UnitConverter.ConvertUnits(saturationInput);
        }

        var gain = -feedbackResistor / inputResistor;
        AnsiConsole.WriteLine($"The gain is {UnitConverter.ConvertToUnit(gain)}");
        if (gainMode) return;
        
        var resistorToGround = 0.0;
        if (isFeedbackNonInverting) {
            resistorToGround = UnitConverter.ConvertUnits(resistorToGroundInput);
        }
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
        var inputVoltageInput = "";
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
        var saturationInput = "";
        var feedbackNonInverting = false;
        if (!gainMode) {
            saturationInput = AnsiConsole.Prompt(
                new TextPrompt<string>("Enter the [green]saturation voltage[/]")
                    .PromptStyle("grey")
                    .DefaultValue("12")
                    .DefaultValueStyle("grey"));
            feedbackNonInverting = AnsiConsole.Prompt(
                new SelectionPrompt<bool>()
                    .Title("Is the feedback resistor connected to the [green]non-inverting input[/]?")
                    .PageSize(10)
                    .MoreChoicesText("[grey](Move up and down to reveal more options)[/]")
                    .AddChoices(true, false)
                    .UseConverter(choice => choice ? "Yes" : "No"));
        }

        var inputVoltage = 0.0;
        if (!gainMode) {
            inputVoltage = UnitConverter.ConvertUnits(inputVoltageInput);
        }
        var feedbackResistor = UnitConverter.ConvertUnits(feedbackResistorInput);
        var inputResistor = UnitConverter.ConvertUnits(inputResistorInput);
        var saturation = 0.0;
        if (!gainMode) {
            saturation = UnitConverter.ConvertUnits(saturationInput);
        }

        var gain = 1 + feedbackResistor / inputResistor;
        AnsiConsole.WriteLine($"The gain is {UnitConverter.ConvertToUnit(gain)}");
        if (gainMode) return;
        
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
                .AddChoices("Voltage", "Missing One Voltage Source", "Missing Rf", "Missing One Rin", "Configure Resistors (E12)"));
        switch (option) {
            case "Voltage":
                InvertingSummingVoltage();
                break;
            case "Missing One Voltage Source":
                InvertingSummingMissingOneSource();
                break;
            case "Missing Rf":
                InvertingSummingMissingRf();
                break;
            case "Missing One Rin":
                InvertingSummingMissingOneRin();
                break;
            case "Configure Resistors (E12)":
                InvertingSummingConfigureResistors();
                break;
        }
    }
    
    private static void InvertingSummingVoltage() {
        // assume 2 inputs
        var gainMode = AnsiConsole.Prompt(
            new SelectionPrompt<bool>()
                .Title("Do you need to calculate the [green]gain[/] or the [green]output voltage[/]?")
                .PageSize(10)
                .MoreChoicesText("[grey](Move up and down to reveal more options)[/]")
                .AddChoices(true, false)
                .UseConverter(choice => choice ? "Gain" : "Output Voltage"));
        var inputVoltagesInputs = new string[2];
        if (!gainMode) {
            for (int i = 0; i < 2; i++) {
                inputVoltagesInputs[i] = AnsiConsole.Prompt(
                    new TextPrompt<string>($"Enter [green]input voltage {i + 1}[/]")
                        .PromptStyle("grey")
                        .DefaultValue("12")
                        .DefaultValueStyle("grey"));
            }
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
        var saturationInput = "";
        if (!gainMode) {
            saturationInput = AnsiConsole.Prompt(
                new TextPrompt<string>("Enter the [green]saturation voltage[/]")
                    .PromptStyle("grey")
                    .DefaultValue("12")
                    .DefaultValueStyle("grey"));
        }

        var inputs = new double[2];
        if (!gainMode) {
            for (int i = 0; i < 2; i++) {
                inputs[i] = UnitConverter.ConvertUnits(inputVoltagesInputs[i]);
            }
        }
        var feedback = UnitConverter.ConvertUnits(feedbackResistorInput);
        var inputResistors = new double[2];
        for (int i = 0; i < 2; i++) {
            inputResistors[i] = UnitConverter.ConvertUnits(inputResistorsInputs[i]);
        }
        var saturation = 0.0;
        if (!gainMode) {
            saturation = UnitConverter.ConvertUnits(saturationInput);
        }

        for (int i = 0; i < 2; i++) {
            var gain = -feedback / inputResistors[i];
            AnsiConsole.WriteLine($"The gain for input {i + 1} is {UnitConverter.ConvertToUnit(gain)}");
        }
        var A = -(1 / inputResistors[0]) * feedback;
        var B = -(1 / inputResistors[1]) * feedback;
        AnsiConsole.WriteLine($"The output in terms of the inputs A and B is {A}A + {B}B");
        if (gainMode) return;

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
    
    private static void InvertingSummingMissingRf() {
        // assume 2 inputs
        var inputVoltagesInputs = new string[2];
        for (int i = 0; i < 2; i++) {
            inputVoltagesInputs[i] = AnsiConsole.Prompt(
                new TextPrompt<string>($"Enter the [green]input voltage {i + 1}[/]")
                    .PromptStyle("grey")
                    .DefaultValue("12")
                    .DefaultValueStyle("grey"));
        }
        var inputResistorsInputs = new string[2];
        for (int i = 0; i < 2; i++) {
            inputResistorsInputs[i] = AnsiConsole.Prompt(
                new TextPrompt<string>($"Enter [green]input resistor {i + 1}[/]")
                    .PromptStyle("grey")
                    .DefaultValue("1k")
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
        var inputResistors = new double[2];
        for (int i = 0; i < 2; i++) {
            inputResistors[i] = UnitConverter.ConvertUnits(inputResistorsInputs[i]);
        }
        var outputVoltage = UnitConverter.ConvertUnits(outputVoltageInput);
        
        var feedbackResistor = -outputVoltage/ (inputVoltages[0] / inputResistors[0] + inputVoltages[1] / inputResistors[1]);
        
        for (int i = 0; i < 2; i++) {
            var gain = -feedbackResistor / inputResistors[i];
            AnsiConsole.WriteLine($"The gain for input {i + 1} is {UnitConverter.ConvertToUnit(gain)}");
        }
        AnsiConsole.WriteLine($"The missing feedback resistor is {UnitConverter.ConvertToUnit(feedbackResistor)}");
    }
    
    private static void InvertingSummingMissingOneRin() {
        // assume 2 inputs
        var inputVoltagesInputs = new string[2];
        for (int i = 0; i < 2; i++) {
            inputVoltagesInputs[i] = AnsiConsole.Prompt(
                new TextPrompt<string>($"Enter the [green]input voltage {i + 1} starting from the given resistor[/]")
                    .PromptStyle("grey")
                    .DefaultValue("12")
                    .DefaultValueStyle("grey"));
        }
        var inputResistorInput = AnsiConsole.Prompt(
            new TextPrompt<string>($"Enter the [green]input resistor[/]")
                .PromptStyle("grey")
                .DefaultValue("1k")
                .DefaultValueStyle("grey"));
        var feedbackResistorInput = AnsiConsole.Prompt(
            new TextPrompt<string>("Enter the [green]feedback resistor[/]")
                .PromptStyle("grey")
                .DefaultValue("1k")
                .DefaultValueStyle("grey"));
        var outputVoltageInput = AnsiConsole.Prompt(
            new TextPrompt<string>("Enter the [green]output voltage[/]")
                .PromptStyle("grey")
                .DefaultValue("12")
                .DefaultValueStyle("grey"));
        
        var inputVoltages = new double[2];
        for (int i = 0; i < 2; i++) {
            inputVoltages[i] = UnitConverter.ConvertUnits(inputVoltagesInputs[i]);
        }
        var inputResistor = UnitConverter.ConvertUnits(inputResistorInput);
        var feedbackResistor = UnitConverter.ConvertUnits(feedbackResistorInput);
        var outputVoltage = UnitConverter.ConvertUnits(outputVoltageInput);
        
        var sumOfInputVoltagesDividedByInputResistance = outputVoltage / -feedbackResistor;
        var missingInputResistor = (1/(sumOfInputVoltagesDividedByInputResistance - (inputVoltages[0] / inputResistor))) * inputVoltages[1];
        
        var inputResistors = new double[] { inputResistor, missingInputResistor };
        for (int i = 0; i < 2; i++) {
            var gain = -feedbackResistor / inputResistors[i];
            AnsiConsole.WriteLine($"The gain for input {i + 1} is {UnitConverter.ConvertToUnit(gain)}");
        }
        AnsiConsole.WriteLine($"The missing input resistor is {UnitConverter.ConvertToUnit(missingInputResistor)}");
    }

    private static void InvertingSummingConfigureResistors() {
        // assume 2 inputs
        var e12 = new double[] { 1.0, 1.2, 1.5, 1.8, 2.2, 2.7, 3.3, 3.9, 4.7, 5.6, 6.8, 8.2 };
        var scales = new double[] { 1, 10, 100, 1000, 10000, 100000, 1000000, 10000000, 100000000, 1000000000 };
        var inputVoltagesInputs = new string[2];
        for (int i = 0; i < 2; i++) {
            inputVoltagesInputs[i] = AnsiConsole.Prompt(
                new TextPrompt<string>($"Enter the [green]input voltage {i + 1}[/]")
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
        
        var output = -closestRf * (inputVoltages[0] / closestR1 + inputVoltages[1] / closestR2);
        
        AnsiConsole.WriteLine($"The gain is {UnitConverter.ConvertToUnit(gain)}");
        AnsiConsole.WriteLine($"The closest R1 is {UnitConverter.ConvertToUnit(closestR1)}");
        AnsiConsole.WriteLine($"The closest R2 is {UnitConverter.ConvertToUnit(closestR2)}");
        AnsiConsole.WriteLine($"The closest Rf is {UnitConverter.ConvertToUnit(closestRf)}");
        AnsiConsole.WriteLine($"The output voltage is {UnitConverter.ConvertToUnit(output)}V");
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
                DifferentialVoltage();
                break;
            case "Missing One Voltage Source":
                DifferentialMissingOneSource();
                break;
        }
    }

    private static void DifferentialVoltage() {
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
        var inputResistorInput = AnsiConsole.Prompt(
            new TextPrompt<string>($"Enter [green]input resistors value[/]")
                .PromptStyle("grey")
                .DefaultValue("1k")
                .DefaultValueStyle("grey"));
        var feedbackResistorInput = AnsiConsole.Prompt(
            new TextPrompt<string>("Enter the [green]feedback resistors value[/]")
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
        var inputResistorInput = AnsiConsole.Prompt(
            new TextPrompt<string>($"Enter [green]input resistors value[/]")
                .PromptStyle("grey")
                .DefaultValue("1k")
                .DefaultValueStyle("grey"));
        var feedbackResistorInput = AnsiConsole.Prompt(
            new TextPrompt<string>("Enter the [green]feedback resistors value[/]")
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
        var option = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Select the type of [green]non-inverting amplifier[/]")
                .PageSize(10)
                .MoreChoicesText("[grey](Move up and down to reveal more options)[/]")
                .AddChoices("Voltage", "Missing One Voltage Source", "Missing Rf", "Missing Rg", "Configure Resistors (E12)"));
        switch (option) {
            case "Voltage":
                NonInvertingSummingVoltage();
                break;
            case "Missing One Voltage Source":
                NonInvertingSummingMissingSource();
                break;
            case "Missing Rf":
                NonInvertingSummingMissingRf();
                break;
            case "Missing Rg":
                NonInvertingSummingMissingRg();
                break;
            case "Configure Resistors (E12)":
                NonInvertingSummingConfigureResistors();
                break;
        }
    }
    private static void NonInvertingSummingVoltage() {
        // assume 2 inputs
        var gainMode = AnsiConsole.Prompt(
            new SelectionPrompt<bool>()
                .Title("Do you need to calculate the [green]gain[/] or the [green]output voltage[/]?")
                .PageSize(10)
                .MoreChoicesText("[grey](Move up and down to reveal more options)[/]")
                .AddChoices(true, false)
                .UseConverter(choice => choice ? "Gain" : "Output Voltage"));
        var inputVoltagesInputs = new string[2];
        if (!gainMode) {
            for (int i = 0; i < 2; i++) {
                inputVoltagesInputs[i] = AnsiConsole.Prompt(
                    new TextPrompt<string>($"Enter [green]input voltage {i + 1}[/]")
                        .PromptStyle("grey")
                        .DefaultValue("12")
                        .DefaultValueStyle("grey"));
            }
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
        var saturationInput = "";
        if (!gainMode) {
            saturationInput = AnsiConsole.Prompt(
                new TextPrompt<string>("Enter the [green]saturation voltage[/]")
                    .PromptStyle("grey")
                    .DefaultValue("12")
                    .DefaultValueStyle("grey"));
        }

        var inputVoltages = new double[2];
        if (!gainMode) {
            for (int i = 0; i < 2; i++) {
                inputVoltages[i] = UnitConverter.ConvertUnits(inputVoltagesInputs[i]);
            }
        }
        var feedbackResistance = UnitConverter.ConvertUnits(feedbackResistanceInput);
        var groundResistance = UnitConverter.ConvertUnits(groundResistanceInput);
        var inputResistors = new double[2];
        for (int i = 0; i < 2; i++) {
            inputResistors[i] = UnitConverter.ConvertUnits(inputResistorsInputs[i]);
        }
        var saturation = 0.0;
        if (!gainMode) {
            saturation = UnitConverter.ConvertUnits(saturationInput);
        }
        
        var gain = 1 + (feedbackResistance / groundResistance);
        var A = gain * (inputResistors[1] / (inputResistors[0] + inputResistors[1]));
        var B = gain * (inputResistors[0] / (inputResistors[0] + inputResistors[1]));
        AnsiConsole.WriteLine($"The gain is {UnitConverter.ConvertToUnit(gain)}");
        AnsiConsole.WriteLine($"The output in terms of the inputs A and B is {A}A + {B}B");
        if (gainMode) return;
        
        var output = (inputVoltages[0]*inputResistors[0] + inputVoltages[1]*inputResistors[1]) / (inputResistors[0] + inputResistors[1]) * gain;
        
        if (output > saturation) {
            output = saturation;
        }
        if (output < -saturation) {
            output = -saturation;
        }
        
        AnsiConsole.WriteLine($"The output voltage is {UnitConverter.ConvertToUnit(output)}V");
    }

    private static void NonInvertingSummingMissingSource() {
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
        var groundResistorInput = AnsiConsole.Prompt(
            new TextPrompt<string>("Enter the [green]ground resistor value[/]")
                .PromptStyle("grey")
                .DefaultValue("1k")
                .DefaultValueStyle("grey"));
        var outputVoltageInput = AnsiConsole.Prompt(
            new TextPrompt<string>("Enter the [green]output voltage[/]")
                .PromptStyle("grey")
                .DefaultValue("12")
                .DefaultValueStyle("grey"));
        
        var inputVoltage = UnitConverter.ConvertUnits(inputVoltageInput);
        var inputResistors = new double[2];
        for (int i = 0; i < 2; i++) {
            inputResistors[i] = UnitConverter.ConvertUnits(inputResistorsInputs[i]);
        }
        var feedbackResistor = UnitConverter.ConvertUnits(feedbackResistorInput);
        var groundResistor = UnitConverter.ConvertUnits(groundResistorInput);
        var outputVoltage = UnitConverter.ConvertUnits(outputVoltageInput);
        
        var gain = 1 + (feedbackResistor / groundResistor);
        var potentialDividerRatio = inputResistors[0] / (inputResistors[0] + inputResistors[1]);
        var voltageSource = (outputVoltage / (potentialDividerRatio * gain)) + inputVoltage;
        
        AnsiConsole.WriteLine($"The gain is {UnitConverter.ConvertToUnit(gain)}");
        AnsiConsole.WriteLine($"The missing input voltage is {UnitConverter.ConvertToUnit(voltageSource)}V");
    }
    
    private static void NonInvertingSummingMissingRf() {
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
                new TextPrompt<string>($"Enter [green]input resistor {i + 1}[/]")
                    .PromptStyle("grey")
                    .DefaultValue("1k")
                    .DefaultValueStyle("grey"));
        }
        var groundResistorInput = AnsiConsole.Prompt(
            new TextPrompt<string>("Enter the [green]ground resistor value[/]")
                .PromptStyle("grey")
                .DefaultValue("1k")
                .DefaultValueStyle("grey"));
        var outputVoltageInput = AnsiConsole.Prompt(
            new TextPrompt<string>("Enter the [green]output voltage[/]")
                .PromptStyle("grey")
                .DefaultValue("12")
                .DefaultValueStyle("grey"));
        
        var inputVoltages = new double[2];
        for (int i = 0; i < 2; i++) {
            inputVoltages[i] = UnitConverter.ConvertUnits(inputVoltagesInputs[i]);
        }
        var inputResistors = new double[2];
        for (int i = 0; i < 2; i++) {
            inputResistors[i] = UnitConverter.ConvertUnits(inputResistorsInputs[i]);
        }
        var groundResistor = UnitConverter.ConvertUnits(groundResistorInput);
        var outputVoltage = UnitConverter.ConvertUnits(outputVoltageInput);
        
        var sumVoltage = 0.0;
        var sumResistances = 0.0;
        for (int i = 0; i < 2; i++) {
            sumVoltage += inputVoltages[i] / inputResistors[i];
            sumResistances += 1 / inputResistors[i];
        }
        var nonInvertingInput = sumVoltage / sumResistances;
        var gain = 1 + (groundResistor / inputResistors[0]);
        var feedbackResistor = (outputVoltage / nonInvertingInput - 1) * groundResistor;
        
        AnsiConsole.WriteLine($"The gain is {UnitConverter.ConvertToUnit(gain)}");
        AnsiConsole.WriteLine($"The missing feedback resistor is {UnitConverter.ConvertToUnit(feedbackResistor)}");
    }
    
    private static void NonInvertingSummingMissingRg() {
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
                new TextPrompt<string>($"Enter [green]input resistor {i + 1}[/]")
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
        
        var inputVoltages = new double[2];
        for (int i = 0; i < 2; i++) {
            inputVoltages[i] = UnitConverter.ConvertUnits(inputVoltagesInputs[i]);
        }
        var inputResistors = new double[2];
        for (int i = 0; i < 2; i++) {
            inputResistors[i] = UnitConverter.ConvertUnits(inputResistorsInputs[i]);
        }
        var feedbackResistor = UnitConverter.ConvertUnits(feedbackResistorInput);
        var outputVoltage = UnitConverter.ConvertUnits(outputVoltageInput);
        
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
    }

    private static void NonInvertingSummingConfigureResistors() {
        // assume 2 inputs
        var e12 = new double[] { 1.0, 1.2, 1.5, 1.8, 2.2, 2.7, 3.3, 3.9, 4.7, 5.6, 6.8, 8.2 };
        var scales = new double[] { 1, 10, 100, 1000, 10000, 100000, 1000000, 10000000, 100000000, 1000000000 };
        var inputVoltagesInputs = new string[2];
        for (int i = 0; i < 2; i++) {
            inputVoltagesInputs[i] = AnsiConsole.Prompt(
                new TextPrompt<string>($"Enter the [green]input voltage {i + 1}[/]")
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
        
        Array.Sort(inputVoltages);
        Array.Reverse(inputVoltages);
        
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
        
        var gain = 1 + (closestRf / closestRg);
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
    }
}