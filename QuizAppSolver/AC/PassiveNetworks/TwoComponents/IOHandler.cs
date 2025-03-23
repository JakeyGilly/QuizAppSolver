using System.Numerics;
using QuizAppSolver.AC.PassiveNetworks.MoreComponents;
using Spectre.Console;
using static QuizAppSolver.Utils.PassiveNetworksUtils;
using static QuizAppSolver.Utils.PassiveNetworksUtils.ConfigTypes;

namespace QuizAppSolver.AC.PassiveNetworks.TwoComponents;

public class IoHandler {
    private static (double componentValue, double resistance, double frequency, Complex impedance, double magnitude, double angle, ComponentType componentType, ImpedanceType impedanceType) GetComponentInputs(ConfigTypes type) {
        double resistance = 0, componentValue = 0, frequency = 0, magnitude = 0, phaseAngle = 0;
        Complex impedance = new();
        ImpedanceType impedanceType = ImpedanceType.ComplexImpedance;
        ComponentType componentType = ComponentType.Capacitor;

        var builder = new UserInputBuilder()
            .AddSelection("Select the component type",
                val => componentType = (ComponentType)Enum.Parse(typeof(ComponentType), val),
                Enum.GetNames(typeof(ComponentType)));
        
        if (type == MissingOneResistor || type == MissingOneComponent) {
            builder.AddSelection("Select the [green]impedance type[/]",
                val => impedanceType = (ImpedanceType)Enum.Parse(typeof(ImpedanceType), val),
                Enum.GetNames(typeof(ImpedanceType)));
            builder.AddComplexImpedanceInput("", val => impedance = val, condition: () => impedanceType == ImpedanceType.ComplexImpedance);
            builder.AddNumericInput("impedance magnitude", val => magnitude = val, condition: () => impedanceType == ImpedanceType.Magnitude);
            builder.AddNumericInput("impedance phase angle", val => phaseAngle = val, condition: () => impedanceType == ImpedanceType.PhaseAngle);
        }

        if (type == Normal || type == MissingOneResistor) {
            builder.AddComponentInput("", val => componentValue = val);
        }
        if (type == Normal || type == MissingOneComponent) {
            builder.AddResistorInput("", val => resistance = val);
        }
        if (type == MissingOneResistor || type == MissingOneComponent) {
        }
        builder.AddFrequencyInput("", val => frequency = val.Real); // TODO: change this when adding frequency calculation
        builder.Build();

        return (componentValue, resistance, frequency, impedance, magnitude, phaseAngle, componentType, impedanceType);
    }
    
    private static void HandleTwoComponentsConfig(Action<ConfigTypes> configHandler) {
        var subConfigTypes = new Dictionary<ConfigTypes, Action<ConfigTypes>> {
            { Normal, configHandler },
            { MissingOneResistor, configHandler },
            { MissingOneComponent, configHandler }
        };

        new UserInputBuilder()
            .AddSelection("Select configuration", val => subConfigTypes[val](val), subConfigTypes.Keys.ToArray())
            .Build();
    }

    public static void HandleTwoComponents() {
        var connectionTypes = new Dictionary<string, Action> {
            { "Series", () => HandleTwoComponentsConfig(TwoComponentsSeries) },
            { "Series with Unknown Component", () => HandleTwoComponentsConfig(TwoComponentsSeriesWithUnknownComponent) },
            { "Series with Unknown Resistor", () => HandleTwoComponentsConfig(TwoComponentsSeriesWithUnknownResistor) },
            { "Parallel", () => HandleTwoComponentsConfig(TwoComponentsParallel) },
            { "Parallel with Unknown Component", () => HandleTwoComponentsConfig(TwoComponentsParallelWithUnknownComponent) },
            { "Parallel with Unknown Resistor", () => HandleTwoComponentsConfig(TwoComponentsParallelWithUnknownResistor) }

        };

        new UserInputBuilder()
            .AddSelection("How are the components connected?", val => connectionTypes[val](),
                connectionTypes.Keys.ToArray())
            .Build();
    }
    
    private static void TwoComponentsSeries(ConfigTypes type) {
        var (componentValue, resistance, frequency, _, _, _, componentType, _) = GetComponentInputs(type);
        Complex result = TwoComponents.CalculateSeries(componentType, componentValue, resistance, frequency);
        DisplayResult(result, new Complex(), type, componentType);
    }
    
    private static void TwoComponentsSeriesWithUnknownComponent(ConfigTypes type) {
        var (_, resistance, frequency, impedance, magnitude, phaseAngle, componentType, impedanceType) = GetComponentInputs(type);
        var (result, impedanceResult) = TwoComponents.CalculateSeriesUnknownComponent(componentType, impedanceType, resistance, frequency, magnitude, phaseAngle, impedance);
        DisplayResult(result, impedanceResult, type, componentType);
    }
    
    private static void TwoComponentsSeriesWithUnknownResistor(ConfigTypes type) {
        var (componentValues, _, frequency, impedance, magnitude, phaseAngle, componentType, impedanceType) = GetComponentInputs(type);
        var (result, impedanceResult) = TwoComponents.CalculateSeriesUnknownResistor(componentType, impedanceType, componentValues, frequency, magnitude, phaseAngle, impedance);
        DisplayResult(result, impedanceResult, type, componentType);
    }
    
    private static void TwoComponentsParallel(ConfigTypes type) {
        var (componentValue, resistance, frequency, _, _, _, componentType, _) = GetComponentInputs(type);
        Complex result = TwoComponents.CalculateParallel(componentType, componentValue, resistance, frequency);
        DisplayResult(result, new Complex(), type, componentType);
    }
    
    private static void TwoComponentsParallelWithUnknownComponent(ConfigTypes type) {
        var (_, resistance, frequency, impedance, magnitude, phaseAngle, componentType, impedanceType) = GetComponentInputs(type);
        var (componentImpedance, reactance) = TwoComponents.CalculateParallelUnknownComponent(componentType, impedanceType, resistance, frequency, magnitude, phaseAngle, impedance);
        Complex result = componentImpedance + reactance;
        DisplayResult(result, new Complex(), type, componentType);
    }
    
    private static void TwoComponentsParallelWithUnknownResistor(ConfigTypes type) {
        var (componentValues, _, frequency, impedance, magnitude, phaseAngle, componentType, impedanceType) = GetComponentInputs(type);
        var (result, impedanceResult) = TwoComponents.CalculateParallelUnknownResistor(componentType, impedanceType, componentValues, frequency, magnitude, phaseAngle, impedance);
        DisplayResult(result, impedanceResult, type, componentType);
    }
    
    private static void DisplayResult(Complex result, Complex impedance, ConfigTypes type, ComponentType componentType) {
        string unit = type switch {
            Normal => "Ω",
            MissingOneResistor => "Ω",
            MissingOneComponent => componentType == ComponentType.Capacitor ? "F" : "H",
            _ => ""
        };
        string description = type switch {
            Normal => "The impedance is",
            MissingOneResistor => "The missing resistor value is",
            MissingOneComponent => "The missing component value is",
            _ => ""
        };

        string formattedResult = result.Imaginary == 0
            ? $"{UnitConverter.ConvertToUnit(result.Real)}{unit}" // Pure resistance
            : result.Real == 0
                ? $"{UnitConverter.ConvertToUnit(result.Imaginary)}{unit}" // Pure inductance
                : $"{UnitConverter.ConvertToUnit(result, true)}{unit}"; // Complex impedance

        AnsiConsole.WriteLine($"{description}: {formattedResult}");
        AnsiConsole.WriteLine($"Impedance: {UnitConverter.ConvertToUnit(impedance)}Ω");
        if (AnsiConsole.Confirm("Back to the main menu")) Program.Main();
    }
}