using Spectre.Console;

namespace QuizAppSolver.OpAmpBandwidths;

public class MissingResistor {
    public static void MissingResistorMenu() {
        new UserInputBuilder().AddSelection("Select the [green]Missing Resistor type[/]",
            val => {
                Action action = val switch {
                    "Missing Feedback Resistor" => MissingFeedbackResistor,
                    "Missing Input/Ground Resistor" => MissingInputGroundResistor,
                    "Missing Feedback Resistor (XdB)" => MissingFeedbackResistorXdB,
                    "Missing Input/Ground Resistor (XdB)" => MissingInputGroundResistorXdB,
                    "Back" => Program.Main,
                    _ => () => { }
                };
                action();
            }, ["Missing Feedback Resistor", "Missing Ground Resistor", "Missing Feedback Resistor (XdB)", "Missing Input/Ground Resistor (XdB)", "Back"]
        ).Build();
    }
    
    private static void MissingFeedbackResistor() {
        double gainBandwidthProduct = 0, groundResistor = 0, threeDbBandwidth = 0;
        new UserInputBuilder()
            .AddResistorInput("input/ground", val => groundResistor = val, postfix: " (input resistor for the inverting amplifier, or the ground resistor for the non-inverting amplifier)")
            .AddFrequencyInput("gain bandwidth product", val => gainBandwidthProduct = val.Real, useSuffix: false)
            .AddFrequencyInput("3dB bandwidth", val => threeDbBandwidth = val.Real, useSuffix: false)
            .Build();

        double gain = gainBandwidthProduct / threeDbBandwidth;
        double feedbackResistor = (gain - 1) * groundResistor;
        
        AnsiConsole.WriteLine($"The missing feedback resistor is {UnitConverter.ConvertToUnit(feedbackResistor)}Ω");

        bool menu = AnsiConsole.Confirm("Back to the main menu");
        if (menu) OpAmpBandwidths.OpAmpBandwithsMenu();
    }
    
    private static void MissingInputGroundResistor() {
        double gainBandwidthProduct = 0, feedbackResistor = 0, threeDbBandwidth = 0;
        new UserInputBuilder()
            .AddResistorInput("feedback", val => feedbackResistor = val)
            .AddFrequencyInput("gain bandwidth product", val => gainBandwidthProduct = val.Real, useSuffix: false)
            .AddFrequencyInput("3dB bandwidth", val => threeDbBandwidth = val.Real, useSuffix: false)
            .Build();

        double gain = gainBandwidthProduct / threeDbBandwidth;
        double groundResistor = feedbackResistor / (gain - 1);
        
        AnsiConsole.WriteLine($"The missing ground resistor is {UnitConverter.ConvertToUnit(groundResistor)}Ω");

        bool menu = AnsiConsole.Confirm("Back to the main menu");
        if (menu) OpAmpBandwidths.OpAmpBandwithsMenu();
    }
    
    private static void MissingFeedbackResistorXdB() {
        double gainBandwidthProduct = 0, groundResistor = 0, x = 0, xdBBandwidth = 0;
        new UserInputBuilder()
            .AddResistorInput("input/ground", val => groundResistor = val, postfix: " (input resistor for the inverting amplifier, or the ground resistor for the non-inverting amplifier)")
            .AddFrequencyInput("gain bandwidth product", val => gainBandwidthProduct = val.Real, useSuffix: false)
            .AddNumericInput("x", val => x = val)
            .AddFrequencyInput("XdB bandwidth", val => xdBBandwidth = val.Real, useSuffix: false)
            .Build();

        double threeDbBandwidth = xdBBandwidth / Math.Sqrt(Math.Pow(10, x / 10) - 1);
        double gain = gainBandwidthProduct / threeDbBandwidth;
        double feedbackResistor = (gain - 1) * groundResistor;
        
        AnsiConsole.WriteLine($"The missing feedback resistor is {UnitConverter.ConvertToUnit(feedbackResistor)}Ω");

        bool menu = AnsiConsole.Confirm("Back to the main menu");
        if (menu) OpAmpBandwidths.OpAmpBandwithsMenu();
    }
    
    private static void MissingInputGroundResistorXdB() {
        double gainBandwidthProduct = 0, feedbackResistor = 0, x = 0, xdBBandwidth = 0;
        new UserInputBuilder()
            .AddResistorInput("feedback", val => feedbackResistor = val)
            .AddFrequencyInput("gain bandwidth product", val => gainBandwidthProduct = val.Real, useSuffix: false)
            .AddNumericInput("x", val => x = val)
            .AddFrequencyInput("XdB bandwidth", val => xdBBandwidth = val.Real, useSuffix: false)
            .Build();

        double threeDbBandwidth = xdBBandwidth / Math.Sqrt(Math.Pow(10, x / 10) - 1);
        double gain = gainBandwidthProduct / threeDbBandwidth;
        double groundResistor = feedbackResistor / (gain - 1);
        
        AnsiConsole.WriteLine($"The missing ground resistor is {UnitConverter.ConvertToUnit(groundResistor)}Ω");

        bool menu = AnsiConsole.Confirm("Back to the main menu");
        if (menu) OpAmpBandwidths.OpAmpBandwithsMenu();
    }
}