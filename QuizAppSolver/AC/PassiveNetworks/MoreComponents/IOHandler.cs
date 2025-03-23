using System.Numerics;
using Spectre.Console;
using static QuizAppSolver.Utils.PassiveNetworksUtils;
using static QuizAppSolver.Utils.PassiveNetworksUtils.ConfigTypes;

namespace QuizAppSolver.AC.PassiveNetworks.MoreComponents;

public class IoHandler {
    private static (List<double> componentValues, double resisistance, double frequency, Complex impedance, ComponentType componentType, bool isMissingComponentInSeries) GetComponentInputsTwoComponentsOneResistor(ConfigTypes type, string compPostfix = "", bool askIfSeries = false) {
        List<double> componentValues = [];
        double resistance = 0, frequency = 0;
        Complex impedance = new();
        ComponentType componentType = ComponentType.Capacitor;
        bool isMissingComponentInSeries = false;

        var builder = new UserInputBuilder().AddSelection("Select the component type", 
            val => componentType = (ComponentType) Enum.Parse(typeof(ComponentType), val),
            Enum.GetNames( typeof( ComponentType ) ));
        if (type == Normal || type == MissingOneResistor) {
            builder.AddMultipleComponentInput("", 2, val => componentValues = val, postfix: compPostfix);
        } else {
            builder.AddComponentInput("known", val => componentValues.Add(val));
        }
        if (type == Normal || type == MissingOneComponent) {
            builder.AddResistorInput("", val => resistance = val);
        }
        if (type == MissingOneResistor || type == MissingOneComponent) {
            builder.AddComplexImpedanceInput("", val => impedance = val);
        }
        builder.AddFrequencyInput("", val => frequency = val.Real);
        if (type == MissingOneComponent && askIfSeries) {
            builder.AddSelection("Is the missing component in series or parallel with the resistor?",
                val => isMissingComponentInSeries = val == "Series",
                ["Series", "Parallel"]);
        }
        builder.Build();
        
        return (componentValues, resistance, frequency, impedance, componentType, isMissingComponentInSeries);
    }
    
    private static void HandleTwoComponentsOneResistorConfig(Action<ConfigTypes> configHandler) {
        var subConfigTypes = new Dictionary<ConfigTypes, Action<ConfigTypes>> {
            { Normal, configHandler },
            { MissingOneResistor, configHandler },
            { MissingOneComponent, configHandler }
        };

        new UserInputBuilder()
            .AddSelection("Select configuration", val => subConfigTypes[val](val), subConfigTypes.Keys.ToArray())
            .Build();
    }

    public static void HandleTwoComponentsOneResistor() {
        var connectionTypes = new Dictionary<string, Action> {
            { "Series", () => HandleTwoComponentsOneResistorConfig(TwoComponentsOneResistorSeries) },
            { "Parallel", () => HandleTwoComponentsOneResistorConfig(TwoComponentsOneResistorParallel) },
            {
                "Component in series with (Component + Resistor in parallel)",
                () => HandleTwoComponentsOneResistorConfig(TwoComponentsOneResistorComponentSeriesComponentResParallel)
            },
            {
                "Component in parallel with (Component + Resistor in series)",
                () => HandleTwoComponentsOneResistorConfig(TwoComponentsOneResistorComponentParallelComponentResSeries)
            },
            {
                "Resistor in parallel with (2 components in series)",
                () => HandleTwoComponentsOneResistorConfig(TwoComponentsOneResistorResParallelComponentSeries)
            },
            {
                "Resistor in series with (2 components in parallel)",
                () => HandleTwoComponentsOneResistorConfig(TwoComponentsOneResistorResSeriesComponentParallel)
            }
        };

        new UserInputBuilder()
            .AddSelection("How are the components connected?", val => connectionTypes[val](),
                connectionTypes.Keys.ToArray())
            .Build();
    }
    
    private static void TwoComponentsOneResistorSeries(ConfigTypes type) {
        var (componentValues, resistance, frequency, impedance, componentType, _) = GetComponentInputsTwoComponentsOneResistor(type);
        Complex result = OneResistorTwoComponents.CalculateSeries(type, componentValues, resistance, frequency, impedance, componentType);
        DisplayResult(result, type, componentType);
    }
    
    private static void TwoComponentsOneResistorParallel(ConfigTypes type) {
        var (componentValues, resistance, frequency, impedance, componentType, _) = GetComponentInputsTwoComponentsOneResistor(type);
        Complex result = OneResistorTwoComponents.CalculateParallel(type, componentValues, resistance, frequency, impedance, componentType);
        DisplayResult(result, type, componentType);
    }
    
    private static void TwoComponentsOneResistorComponentSeriesComponentResParallel(ConfigTypes type) {
        var (componentValues, resistance, frequency, impedance, componentType, isMissingComponentInSeries) = GetComponentInputsTwoComponentsOneResistor(type, "(starting with the component in series)", true);
        Complex result = OneResistorTwoComponents.CalculateComponentSeriesComponentResParallel(type, componentValues, resistance, frequency, impedance, componentType, isMissingComponentInSeries);
        DisplayResult(result, type, componentType);
    }
    
    private static void TwoComponentsOneResistorComponentParallelComponentResSeries(ConfigTypes type) {
        var (componentValues, resistance, frequency, impedance, componentType, isMissingComponentInSeries) = GetComponentInputsTwoComponentsOneResistor(type, "(starting with the component in series)");
        Complex result = OneResistorTwoComponents.CalculateComponentParallelComponentResSeries(type, componentValues, resistance, frequency, impedance, componentType, isMissingComponentInSeries);
        DisplayResult(result, type, componentType);
    }
    
    private static void TwoComponentsOneResistorResParallelComponentSeries(ConfigTypes type) {
        var (componentValues, resistance, frequency, impedance, componentType, _) = GetComponentInputsTwoComponentsOneResistor(type, "(starting with the resistor in parallel)");
        Complex result = OneResistorTwoComponents.CalculateResParallelComponentSeries(type, componentValues, resistance, frequency, impedance, componentType);
        DisplayResult(result, type, componentType);
    }
    
    private static void TwoComponentsOneResistorResSeriesComponentParallel(ConfigTypes type) {
        var (componentValues, resistance, frequency, impedance, componentType, _) = GetComponentInputsTwoComponentsOneResistor(type, "(starting with the resistor in series)");
        Complex result = OneResistorTwoComponents.CalculateResSeriesComponentParallel(type, componentValues, resistance, frequency, impedance, componentType);
        DisplayResult(result, type, componentType);
    }
    
    private static (List<double> resistances, double componentValue, double frequency, Complex impedance, ComponentType componentType, bool isMissingComponentInSeries) GetComponentInputsTwoResistorsOneComponent(ConfigTypes type, string resPostfix = "", bool askIfSeries = false) {
        List<double> resistances = [];
        double componentValue = 0, frequency = 0;
        Complex impedance = new();
        ComponentType componentType = ComponentType.Capacitor;
        bool isMissingComponentInSeries = false;

        var builder = new UserInputBuilder().AddSelection("Select the component type", 
            val => componentType = (ComponentType) Enum.Parse(typeof(ComponentType), val),
            Enum.GetNames( typeof( ComponentType ) ));
        if (type == Normal || type == MissingOneComponent) {
            builder.AddMultipleResistorInput("", 2, val => resistances = val, postfix: resPostfix);
        } else {
            builder.AddResistorInput("known", val => resistances.Add(val));
        }
        if (type == MissingOneResistor || type == Normal) {
            builder.AddComponentInput("", val => componentValue = val);
        }
        if (type == MissingOneComponent || type == MissingOneResistor) {
            builder.AddComplexImpedanceInput("", val => impedance = val);
        }
        builder.AddFrequencyInput("", val => frequency = val.Real);
        if (type == MissingOneResistor && askIfSeries) {
            builder.AddSelection("Is the missing resistor in series or parallel with the component?",
                val => isMissingComponentInSeries = val == "Series",
                ["Series", "Parallel"]);
        }
        builder.Build();
        
        return (resistances, componentValue, frequency, impedance, componentType, isMissingComponentInSeries);
    }
    
    private static void HandleTwoResistorsOneComponentConfig(Action<ConfigTypes> configHandler) {
        var subConfigTypes = new Dictionary<ConfigTypes, Action<ConfigTypes>> {
            { Normal, configHandler },
            { MissingOneResistor, configHandler },
            { MissingOneComponent, configHandler }
        };

        new UserInputBuilder()
            .AddSelection("Select configuration", val => subConfigTypes[val](val), subConfigTypes.Keys.ToArray())
            .Build();
    }
    
    public static void HandleTwoResistorsOneComponent() {
        var connectionTypes = new Dictionary<string, Action> {
            { "Series", () => HandleTwoResistorsOneComponentConfig(TwoResistorsOneComponentSeries) },
            { "Parallel", () => HandleTwoResistorsOneComponentConfig(TwoResistorsOneComponentParallel) },
            {
                "Resistor in series with (Component + Resistor in parallel)",
                () => HandleTwoResistorsOneComponentConfig(TwoResistorsOneComponentResSeriesComponentResParallel)
            },
            {
                "Resistor in parallel with (Component + Resistor in series)",
                () => HandleTwoResistorsOneComponentConfig(TwoResistorsOneComponentResParallelComponentResSeries)
            },
            {
                "Component in parallel with (2 resistors in series)",
                () => HandleTwoResistorsOneComponentConfig(TwoResistorsOneComponentComponentParallelResSeries)
            },
            {
                "Component in series with (2 resistors in parallel)",
                () => HandleTwoResistorsOneComponentConfig(TwoResistorsOneComponentComponentSeriesResParallel)
            }
        };

        new UserInputBuilder()
            .AddSelection("How are the components connected?", val => connectionTypes[val](),
                connectionTypes.Keys.ToArray())
            .Build();
    }
    
    private static void TwoResistorsOneComponentSeries(ConfigTypes type) {
        var (resistances, componentValue , frequency, impedance, componentType, _) = GetComponentInputsTwoResistorsOneComponent(type);
        Complex result = TwoResistorsOneComponent.CalculateSeries(type, resistances, componentValue, frequency, impedance, componentType);
        DisplayResult(result, type, componentType);
    }
    
    private static void TwoResistorsOneComponentParallel(ConfigTypes type) {
        var (resistances, componentValue , frequency, impedance, componentType, _) = GetComponentInputsTwoResistorsOneComponent(type);
        Complex result = TwoResistorsOneComponent.CalculateParallel(type, resistances, componentValue, frequency, impedance, componentType);
        DisplayResult(result, type, componentType);
    }
    
    private static void TwoResistorsOneComponentResSeriesComponentResParallel(ConfigTypes type) {
        var (resistances, componentValue , frequency, impedance, componentType, isMissingComponentInSeries) = GetComponentInputsTwoResistorsOneComponent(type, "(starting with the resistor in series)", true);
        Complex result = TwoResistorsOneComponent.CalculateResSeriesComponentResParallel(type, resistances, componentValue, frequency, impedance, componentType, isMissingComponentInSeries);
        DisplayResult(result, type, componentType);
    }
    
    private static void TwoResistorsOneComponentResParallelComponentResSeries(ConfigTypes type) {
        var (resistances, componentValue , frequency, impedance, componentType, isMissingComponentInSeries) = GetComponentInputsTwoResistorsOneComponent(type, "(starting with the resistor in series)", true);
        Complex result = TwoResistorsOneComponent.CalculateResParallelComponentResSeries(type, resistances, componentValue, frequency, impedance, componentType, isMissingComponentInSeries);
        DisplayResult(result, type, componentType);
    }
    
    private static void TwoResistorsOneComponentComponentParallelResSeries(ConfigTypes type) {
        var (resistances, componentValue , frequency, impedance, componentType, _) = GetComponentInputsTwoResistorsOneComponent(type);
        Complex result = TwoResistorsOneComponent.CalculateComponentParallelResSeries(type, resistances, componentValue, frequency, impedance, componentType);
        DisplayResult(result, type, componentType);
    }
    
    private static void TwoResistorsOneComponentComponentSeriesResParallel(ConfigTypes type) {
        var (resistances, componentValue , frequency, impedance, componentType, _) = GetComponentInputsTwoResistorsOneComponent(type);
        Complex result = TwoResistorsOneComponent.CalculateComponentSeriesResParallel(type, resistances, componentValue, frequency, impedance, componentType);
        DisplayResult(result, type, componentType);
    }
    
    private static void DisplayResult(Complex result, ConfigTypes type, ComponentType componentType) {
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
        if (AnsiConsole.Confirm("Back to the main menu")) Program.Main();
    }
}