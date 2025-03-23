using System.Numerics;
using static QuizAppSolver.Utils.PassiveNetworksUtils;
using static QuizAppSolver.Utils.PassiveNetworksUtils.ComponentType;
using static QuizAppSolver.Utils.PassiveNetworksUtils.ConfigTypes;

namespace QuizAppSolver.AC.PassiveNetworks.MoreComponents;

public static class OneResistorTwoComponents {
    public static Complex CalculateSeries(ConfigTypes type, List<double> componentValue, double resistance, double frequency, Complex impedance, ComponentType componentType) {
        Complex comp1Impedance = componentType switch {
            Capacitor => new Complex(0, -1 / (2 * Math.PI * frequency * componentValue[0])),
            Inductor => new Complex(0, 2 * Math.PI * frequency * componentValue[0]),
            _ => throw new InvalidOperationException("Invalid selection")
        };

        Complex comp2Impedance = new Complex();
        if (componentValue.Count != 1) {
            comp2Impedance = componentType switch {
                Capacitor => new Complex(0, -1 / (2 * Math.PI * frequency * componentValue[1])),
                Inductor => new Complex(0, 2 * Math.PI * frequency * componentValue[1]),
                _ => throw new InvalidOperationException("Invalid selection")
            };
        }

        double reactanceFactor = componentType switch {
            Capacitor => -1 / (2 * Math.PI * frequency),
            Inductor => 2 * Math.PI * frequency,
            _ => throw new InvalidOperationException("Invalid selection")
        };
        return type switch {
            Normal => comp1Impedance + comp2Impedance + resistance,
            MissingOneResistor => new Complex((impedance - comp1Impedance - comp2Impedance).Real, 0),
            MissingOneComponent => componentType switch {
                Capacitor => new Complex(0, reactanceFactor / (impedance - comp1Impedance - resistance).Imaginary),
                Inductor => new Complex(0, (impedance - comp1Impedance - resistance).Imaginary / reactanceFactor),
                _ => throw new InvalidOperationException("Invalid selection")
            },
            _ => throw new InvalidOperationException("Invalid selection")
        };
    }
    
    public static Complex CalculateParallel(ConfigTypes type, List<double> componentValue, double resistance, double frequency, Complex impedance, ComponentType componentType) {
        Complex comp1Impedance = componentType switch {
            Capacitor => new Complex(0, -1 / (2 * Math.PI * frequency * componentValue[0])),
            Inductor => new Complex(0, 2 * Math.PI * frequency * componentValue[0]),
            _ => throw new InvalidOperationException("Invalid selection")
        };
    
        Complex comp2Impedance = new Complex();
        if (componentValue.Count != 1) {
            comp2Impedance = componentType switch {
                Capacitor => new Complex(0, -1 / (2 * Math.PI * frequency * componentValue[1])),
                Inductor => new Complex(0, 2 * Math.PI * frequency * componentValue[1]),
                _ => throw new InvalidOperationException("Invalid selection")
            };
        }
    
        double reactanceFactor = componentType switch {
            Capacitor => -1 / (2 * Math.PI * frequency),
            Inductor => 2 * Math.PI * frequency,
            _ => throw new InvalidOperationException("Invalid selection")
        };
        
        return type switch {
            Normal => 1 / (1 / comp1Impedance + 1 / comp2Impedance + 1 / resistance),
            MissingOneResistor => new Complex(1 / (1 / impedance - 1 / comp1Impedance - 1 / comp2Impedance).Real, 0),
            MissingOneComponent => componentType switch {
                Capacitor => new Complex(0, reactanceFactor / (1 / (1 / impedance - 1 / resistance - 1 / comp1Impedance)).Imaginary),
                Inductor => new Complex(0, (1 / (1 / impedance - 1 / resistance - 1 / comp1Impedance)).Imaginary / reactanceFactor),
                _ => throw new InvalidOperationException("Invalid selection")
            },
            _ => throw new InvalidOperationException("Invalid selection")
        };
    }
    
    public static Complex CalculateComponentSeriesComponentResParallel(ConfigTypes type, List<double> componentValue, double resistance, double frequency, Complex impedance, ComponentType componentType, bool isMissingComponentInSeries) {
        Complex comp1Impedance = componentType switch {
            Capacitor => new Complex(0, -1 / (2 * Math.PI * frequency * componentValue[0])),
            Inductor => new Complex(0, 2 * Math.PI * frequency * componentValue[0]),
            _ => throw new InvalidOperationException("Invalid selection")
        };
    
        Complex comp2Impedance = new Complex();
        if (componentValue.Count != 1) {
            comp2Impedance = componentType switch {
                Capacitor => new Complex(0, -1 / (2 * Math.PI * frequency * componentValue[1])),
                Inductor => new Complex(0, 2 * Math.PI * frequency * componentValue[1]),
                _ => throw new InvalidOperationException("Invalid selection")
            };
        }

        double reactanceFactor = componentType switch {
            Capacitor => -1 / (2 * Math.PI * frequency),
            Inductor => 2 * Math.PI * frequency,
            _ => throw new InvalidOperationException("Invalid selection")
        };

        Console.WriteLine($"{comp1Impedance} {comp2Impedance} {resistance} {impedance} {componentType} {isMissingComponentInSeries}");
        return type switch {
            Normal => comp1Impedance + 1 / (1 / comp2Impedance + 1 / resistance),
            MissingOneResistor => new Complex(1 / (1 / (impedance - comp1Impedance) - 1 / comp2Impedance).Real, 0),
            MissingOneComponent => componentType switch {
                Capacitor => isMissingComponentInSeries
                    ? new Complex(0, reactanceFactor / (impedance - 1 / (1 / comp1Impedance + 1 / resistance)).Imaginary)
                    : new Complex(0, 1 / (-1 / (1 / (impedance - comp1Impedance) - 1 / resistance).Imaginary / reactanceFactor)),
                Inductor => isMissingComponentInSeries
                    ? new Complex(0, (impedance - 1 / (1 / comp1Impedance + 1 / resistance)).Imaginary / reactanceFactor)
                    : new Complex(0, -1 / (1 / (impedance - comp1Impedance) - 1 / resistance).Imaginary / reactanceFactor),
                _ => throw new InvalidOperationException("Invalid selection")
            },
            _ => throw new InvalidOperationException("Invalid selection")
        };
    }
    
    public static Complex CalculateComponentParallelComponentResSeries(ConfigTypes type, List<double> componentValues, double resistance, double frequency, Complex impedance, ComponentType componentType, bool isMissingComponentInSeries) {
        Complex comp1Impedance = componentType switch {
            Capacitor => new Complex(0, -1 / (2 * Math.PI * frequency * componentValues[0])),
            Inductor => new Complex(0, 2 * Math.PI * frequency * componentValues[0]),
            _ => throw new InvalidOperationException("Invalid selection")
        };
    
        Complex comp2Impedance = new Complex();
        if (componentValues.Count != 1) {
            comp2Impedance = componentType switch {
                Capacitor => new Complex(0, -1 / (2 * Math.PI * frequency * componentValues[1])),
                Inductor => new Complex(0, 2 * Math.PI * frequency * componentValues[1]),
                _ => throw new InvalidOperationException("Invalid selection")
            };
        }
    
        double reactanceFactor = componentType switch {
            Capacitor => -1 / (2 * Math.PI * frequency),
            Inductor => 2 * Math.PI * frequency,
            _ => throw new InvalidOperationException("Invalid selection")
        };
        
        return type switch {
            Normal => 1 / (1 / comp2Impedance + 1 / (resistance + comp1Impedance)),
            MissingOneResistor => new Complex((1 / (1 / impedance - 1 / comp2Impedance) - comp1Impedance).Real, 0),
            MissingOneComponent => componentType switch {
                Capacitor => isMissingComponentInSeries
                    ? new Complex(0, reactanceFactor/(comp1Impedance * impedance / (comp1Impedance - impedance)).Imaginary)
                    : new Complex(0, reactanceFactor/(impedance * (resistance + comp1Impedance) / (resistance + comp1Impedance - impedance)).Imaginary),
                Inductor => isMissingComponentInSeries
                    ? new Complex(0, (comp1Impedance * impedance / (comp1Impedance - impedance)).Imaginary / reactanceFactor)
                    : new Complex(0, (impedance * (resistance + comp1Impedance) / (resistance + comp1Impedance - impedance)).Imaginary / reactanceFactor),
                _ => throw new InvalidOperationException("Invalid selection")
            },
            _ => throw new InvalidOperationException("Invalid selection")
        };
    }
    
    public static Complex CalculateResParallelComponentSeries(ConfigTypes type, List<double> componentValues, double resistance, double frequency, Complex impedance, ComponentType componentType) {
        Complex comp1Impedance = componentType switch {
            Capacitor => new Complex(0, -1 / (2 * Math.PI * frequency * componentValues[0])),
            Inductor => new Complex(0, 2 * Math.PI * frequency * componentValues[0]),
            _ => throw new InvalidOperationException("Invalid selection")
        };
    
        Complex comp2Impedance = new Complex();
        if (componentValues.Count != 1) {
            comp2Impedance = componentType switch {
                Capacitor => new Complex(0, -1 / (2 * Math.PI * frequency * componentValues[1])),
                Inductor => new Complex(0, 2 * Math.PI * frequency * componentValues[1]),
                _ => throw new InvalidOperationException("Invalid selection")
            };
        }
    
        double reactanceFactor = componentType switch {
            Capacitor => -1 / (2 * Math.PI * frequency),
            Inductor => 2 * Math.PI * frequency,
            _ => throw new InvalidOperationException("Invalid selection")
        };
        
        return type switch {
            Normal => 1 / (1 / (comp1Impedance + comp2Impedance) + 1 / resistance),
            MissingOneResistor => new Complex((1 / (1 / impedance - 1 / (comp1Impedance + comp2Impedance))).Real, 0),
            MissingOneComponent => componentType switch {
                Capacitor => new Complex(0, reactanceFactor / (1 / (1 / impedance - 1 / resistance) - comp1Impedance).Imaginary),
                Inductor => new Complex(0, (1 / (1 / impedance - 1 / resistance) - comp1Impedance).Imaginary / reactanceFactor),
                _ => throw new InvalidOperationException("Invalid selection")
            },
            _ => throw new InvalidOperationException("Invalid selection")
        };
    }
    
    public static Complex CalculateResSeriesComponentParallel(ConfigTypes type, List<double> componentValues, double resistance, double frequency, Complex impedance, ComponentType componentType) {
        Complex comp1Impedance = componentType switch {
            Capacitor => new Complex(0, -1 / (2 * Math.PI * frequency * componentValues[0])),
            Inductor => new Complex(0, 2 * Math.PI * frequency * componentValues[0]),
            _ => throw new InvalidOperationException("Invalid selection")
        };
    
        Complex comp2Impedance = new Complex();
        if (componentValues.Count != 1) {
            comp2Impedance = componentType switch {
                Capacitor => new Complex(0, -1 / (2 * Math.PI * frequency * componentValues[1])),
                Inductor => new Complex(0, 2 * Math.PI * frequency * componentValues[1]),
                _ => throw new InvalidOperationException("Invalid selection")
            };
        }
    
        double reactanceFactor = componentType switch {
            Capacitor => -1 / (2 * Math.PI * frequency),
            Inductor => 2 * Math.PI * frequency,
            _ => throw new InvalidOperationException("Invalid selection")
        };
        
        return type switch {
            Normal => resistance + 1/(1 / comp1Impedance + 1 / comp2Impedance),
            MissingOneResistor => new Complex((impedance - 1/(1/comp1Impedance + 1/comp2Impedance)).Real, 0),
            MissingOneComponent => componentType switch {
                Capacitor => new Complex(0, reactanceFactor / (1/(1/(impedance - resistance) - 1/comp1Impedance)).Imaginary),
                Inductor => new Complex(0, (1/(1/(impedance - resistance) - 1/comp1Impedance)).Imaginary / reactanceFactor),
                _ => throw new InvalidOperationException("Invalid selection")
            },
            _ => throw new InvalidOperationException("Invalid selection")
        };
    }
}