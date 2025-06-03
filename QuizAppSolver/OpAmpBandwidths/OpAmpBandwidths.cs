using QuizAppSolver.Utils;
using Spectre.Console;

namespace QuizAppSolver.OpAmpBandwidths;

public class OpAmpBandwidths {
    public static void OpAmpBandwithsMenu() {
        new UserInputBuilder().AddSelection("Select the [green]Op-Amp Bandwidth[/]",
            val => {
                Action action = val switch {
                    "3dB Bandwidth" => ThreeDbBandwidth,
                    "3dB Bandwidth (Summing)" => ThreeDbBandwidthSumming,
                    "XdB Bandwidth" => XdbBandwidth,
                    "Missing Resistor" => MissingResistor.MissingResistorMenu,
                    "Back" => Program.Main,
                    _ => () => { }
                };
                action();
            }, ["3dB Bandwidth", "3dB Bandwidth (Summing)", "XdB Bandwidth", "Missing Resistor", "Back"]
        ).Build();
    }
    
    private static void ThreeDbBandwidth() {
        double gainBandwidthProduct = 0, groundResistor = 0, feedbackResistor = 0;
        new UserInputBuilder()
            .AddResistorInput("input/ground" , val => groundResistor = val, postfix: " (input resistor for the inverting amplifier, or the ground resistor for the non-inverting amplifier)")
            .AddResistorInput("feedback", val => feedbackResistor = val)
            .AddFrequencyInput("gain bandwidth product", val => gainBandwidthProduct = val.Real, useSuffix: false)
            .Build();
        
        double threeDbBandwidth = OpAmpUtils.CalculateThreeDbBandwidth(feedbackResistor, groundResistor, gainBandwidthProduct);
        AnsiConsole.WriteLine($"The 3dB bandwidth is {UnitConverter.ConvertToUnit(threeDbBandwidth)}Hz");
        
        bool menu = AnsiConsole.Confirm("Back to the main menu");
        if (menu) OpAmpBandwithsMenu();
    }
    
    private static void ThreeDbBandwidthSumming() {
        // assume 2 inputs
        double gainBandwidthProduct = 0, feedbackResistor = 0;
        List<double> inputResistors = new();
        new UserInputBuilder()
            .AddMultipleResistorInput("input/ground", 2, val => inputResistors = val, postfix: " (input resistor for the inverting amplifier, or the ground resistor for the non-inverting amplifier)")
            .AddResistorInput("feedback", val => feedbackResistor = val)
            .AddFrequencyInput("gain bandwidth product", val => gainBandwidthProduct = val.Real, useSuffix: false)
            .Build();

        double gain1 = OpAmpUtils.CalculateGain(feedbackResistor, inputResistors[0], true);
        double gain2 = OpAmpUtils.CalculateGain(feedbackResistor, inputResistors[1], true);
        double threeDbBandwidth = gainBandwidthProduct / (1 + Math.Abs(gain1) + Math.Abs(gain2));
        AnsiConsole.WriteLine($"The 3dB bandwidth is {UnitConverter.ConvertToUnit(threeDbBandwidth)}Hz");
        
        bool menu = AnsiConsole.Confirm("Back to the main menu");
        if (menu) OpAmpBandwithsMenu();
    }

    private static void XdbBandwidth() {
        double gainBandwidthProduct = 0, inputResistor = 0, feedbackResistor = 0, x = 0;
        new UserInputBuilder()
            .AddResistorInput("input/ground" , val => inputResistor = val, postfix: " (input resistor for the inverting amplifier, or the ground resistor for the non-inverting amplifier)")
            .AddResistorInput("feedback", val => feedbackResistor = val)
            .AddFrequencyInput("gain bandwidth product", val => gainBandwidthProduct = val.Real, useSuffix: false)
            .AddNumericInput("x", val => x = val)
            .Build();

        double threeDbBandwidth = OpAmpUtils.CalculateXdbBandwidth(feedbackResistor, inputResistor, gainBandwidthProduct, x);
        AnsiConsole.WriteLine($"The 3dB bandwidth is {UnitConverter.ConvertToUnit(threeDbBandwidth)}Hz");
        
        bool menu = AnsiConsole.Confirm("Back to the main menu");
        if (menu) OpAmpBandwithsMenu();
    }
}