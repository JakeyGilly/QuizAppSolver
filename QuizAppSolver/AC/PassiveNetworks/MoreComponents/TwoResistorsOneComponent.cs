using System.Numerics;
using static QuizAppSolver.Utils.PassiveNetworksUtils;
using static QuizAppSolver.Utils.PassiveNetworksUtils.ComponentType;
using static QuizAppSolver.Utils.PassiveNetworksUtils.ConfigTypes;

namespace QuizAppSolver.AC.PassiveNetworks.MoreComponents;

public class TwoResistorsOneComponent {
    public static Complex CalculateSeries(ConfigTypes type, List<double> resistances, double componentValue, double frequency, Complex impedance, ComponentType componentType) {
        Complex res1 = new(resistances[0], 0);
        Complex res2 = new Complex();
        if (resistances.Count != 1) {
            res2 = new Complex(resistances[1], 0);
        }
        
        Complex compImpedance = componentType switch {
            Capacitor => new Complex(0, -1 / (2 * Math.PI * frequency * componentValue)),
            Inductor => new Complex(0, 2 * Math.PI * frequency * componentValue),
            _ => throw new InvalidOperationException("Invalid selection")
        };

        double reactanceFactor = componentType switch {
            Capacitor => -1 / (2 * Math.PI * frequency),
            Inductor => 2 * Math.PI * frequency,
            _ => throw new InvalidOperationException("Invalid selection")
        };
        
        return type switch {
            Normal => res1 + res2 + compImpedance,
            MissingOneResistor => new Complex((impedance - res1 - compImpedance).Real, 0),
            MissingOneComponent => componentType switch {
                Capacitor => new Complex(0, reactanceFactor/(impedance - res1 - res2).Imaginary),
                Inductor => new Complex(0, (impedance - res1 - res2).Imaginary / reactanceFactor),
                _ => throw new InvalidOperationException("Invalid selection")
            },
            _ => throw new InvalidOperationException("Invalid selection")
        };
    }

    public static Complex CalculateParallel(ConfigTypes type, List<double> resistances, double componentValue, double frequency, Complex impedance, ComponentType componentType) {
        Complex res1 = new(resistances[0], 0);
        Complex res2 = new Complex();
        if (resistances.Count != 1) {
            res2 = new Complex(resistances[1], 0);
        }
        
        Complex compImpedance = componentType switch {
            Capacitor => new Complex(0, -1 / (2 * Math.PI * frequency * componentValue)),
            Inductor => new Complex(0, 2 * Math.PI * frequency * componentValue),
            _ => throw new InvalidOperationException("Invalid selection")
        };

        double reactanceFactor = componentType switch {
            Capacitor => -1 / (2 * Math.PI * frequency),
            Inductor => 2 * Math.PI * frequency,
            _ => throw new InvalidOperationException("Invalid selection")
        };
        
        return type switch {
            Normal => 1 / (1 / res1 + 1 / res2 + 1 / compImpedance),
            MissingOneResistor => new Complex(1 / (1 / impedance - 1 / res1 - 1 / compImpedance).Real, 0),
            MissingOneComponent => componentType switch {
                Capacitor => new Complex(0, reactanceFactor/(1 / (1 / impedance - 1 / res1 - 1 / res2)).Imaginary),
                Inductor => new Complex(0, (1 / (1 / impedance - 1 / res1 - 1 / res2)).Imaginary / reactanceFactor),
                _ => throw new InvalidOperationException("Invalid selection")
            },
            _ => throw new InvalidOperationException("Invalid selection")
        };
    }

    public static Complex CalculateResSeriesComponentResParallel(ConfigTypes type, List<double> resistances, double componentValue, double frequency, Complex impedance, ComponentType componentType, bool isMissingResistorInSeries) {
        Complex res1 = new Complex(resistances[0], 0);
        Complex res2 = new Complex();
        if (resistances.Count != 1) {
            res2 = new Complex(resistances[1], 0);
        }

        Complex compImpedance = componentType switch {
            Capacitor => new Complex(0, -1 / (2 * Math.PI * frequency * componentValue)),
            Inductor => new Complex(0, 2 * Math.PI * frequency * componentValue),
            _ => throw new InvalidOperationException("Invalid selection")
        };

        double reactanceFactor = componentType switch {
            Capacitor => -1 / (2 * Math.PI * frequency),
            Inductor => 2 * Math.PI * frequency,
            _ => throw new InvalidOperationException("Invalid selection")
        };
        
        return type switch {
            Normal => res1 + 1 / (1 / res2 + 1 / compImpedance),
            MissingOneResistor => isMissingResistorInSeries
                ? new Complex((impedance - 1 / (1 / res1 + 1 / compImpedance)).Real, 0)
                : new Complex((1 / (1 / (impedance - res1) - 1 / compImpedance)).Real, 0),
            MissingOneComponent => componentType switch {
                Capacitor => new Complex(0, reactanceFactor/(1 / (1 / (impedance - res1) - 1 / res2)).Imaginary),
                Inductor => new Complex(0, (1 / (1 / (impedance - res1) - 1 / res2)).Imaginary / reactanceFactor),
                _ => throw new InvalidOperationException("Invalid selection")
            },
            _ => throw new InvalidOperationException("Invalid selection")
        };
    }

    public static Complex CalculateResParallelComponentResSeries(ConfigTypes type, List<double> resistances, double componentValue, double frequency, Complex impedance, ComponentType componentType, bool isMissingResistorInSeries) {
        Complex res1 = new Complex(resistances[0], 0);
        Complex res2 = new Complex();
        if (resistances.Count != 1) {
            res2 = new Complex(resistances[1], 0);
        }

        Complex compImpedance = componentType switch {
            Capacitor => new Complex(0, -1 / (2 * Math.PI * frequency * componentValue)),
            Inductor => new Complex(0, 2 * Math.PI * frequency * componentValue),
            _ => throw new InvalidOperationException("Invalid selection")
        };
        
        double reactanceFactor = componentType switch {
            Capacitor => -1 / (2 * Math.PI * frequency),
            Inductor => 2 * Math.PI * frequency,
            _ => throw new InvalidOperationException("Invalid selection")
        };
        
        return type switch {
            Normal => 1 / (1 / (compImpedance + res1) + 1 / res2),
            MissingOneResistor => isMissingResistorInSeries
                ? new Complex((1 / (1 / impedance - 1 / res1) - compImpedance).Real, 0)
                : new Complex((1 / (1 / impedance - 1 / (compImpedance + res1))).Real, 0),
            MissingOneComponent => componentType switch {
                Capacitor => new Complex(0, 1 / ((1 / (1 / impedance - 1 / res2) - res1).Imaginary / reactanceFactor)),
                Inductor => new Complex(0, (1 / (1 / impedance - 1 / res2) - res1).Imaginary / reactanceFactor),
                _ => throw new InvalidOperationException("Invalid selection")
            },
            _ => throw new InvalidOperationException("Invalid selection")
        };
    }

    public static Complex CalculateComponentParallelResSeries(ConfigTypes type, List<double> resistances, double componentValue, double frequency, Complex impedance, ComponentType componentType) {
        Complex res1 = new Complex(resistances[0], 0);
        Complex res2 = new Complex();
        if (resistances.Count != 1) {
            res2 = new Complex(resistances[1], 0);
        }

        Complex compImpedance = componentType switch {
            Capacitor => new Complex(0, -1 / (2 * Math.PI * frequency * componentValue)),
            Inductor => new Complex(0, 2 * Math.PI * frequency * componentValue),
            _ => throw new InvalidOperationException("Invalid selection")
        };
        
        double reactanceFactor = componentType switch {
            Capacitor => -1 / (2 * Math.PI * frequency),
            Inductor => 2 * Math.PI * frequency,
            _ => throw new InvalidOperationException("Invalid selection")
        };
        
        return type switch {
            Normal => 1 / (1 / compImpedance + (1 / (res1 + res2))),
            MissingOneResistor => new Complex((1 / (1 / impedance - 1 / compImpedance) - res1).Real, 0),
            MissingOneComponent => componentType switch {
                Capacitor => new Complex(0, (-reactanceFactor * (1 / impedance - 1 / (res1 + res2))).Imaginary), // ?? negative sign
                Inductor => new Complex(0, (1 / (1 / impedance - 1 / (res1 + res2))).Imaginary / reactanceFactor),
                _ => throw new InvalidOperationException("Invalid selection")
            },
            _ => throw new InvalidOperationException("Invalid selection")
        };
    }

    public static Complex CalculateComponentSeriesResParallel(ConfigTypes type, List<double> resistances, double componentValue, double frequency, Complex impedance, ComponentType componentType) {
        Complex res1 = new Complex(resistances[0], 0);
        Complex res2 = new Complex();
        if (resistances.Count != 1) {
            res2 = new Complex(resistances[1], 0);
        }

        Complex compImpedance = componentType switch {
            Capacitor => new Complex(0, -1 / (2 * Math.PI * frequency * componentValue)),
            Inductor => new Complex(0, 2 * Math.PI * frequency * componentValue),
            _ => throw new InvalidOperationException("Invalid selection")
        };
        
        double reactanceFactor = componentType switch {
            Capacitor => -1 / (2 * Math.PI * frequency),
            Inductor => 2 * Math.PI * frequency,
            _ => throw new InvalidOperationException("Invalid selection")
        };
        
        return type switch {
            Normal => 1 / (1 / res1 + 1 / res2) + compImpedance,
            MissingOneResistor => new Complex((1 / (1 / (impedance - compImpedance) - 1 / res1)).Real, 0),
            MissingOneComponent => componentType switch {
                Capacitor => new Complex(0, reactanceFactor / (impedance - 1 / (1 / res1 + 1 / res2)).Imaginary),
                Inductor => new Complex(0, (impedance - 1 / (1 / res1 + 1 / res2)).Imaginary / reactanceFactor),
                _ => throw new InvalidOperationException("Invalid selection")
            },
            _ => throw new InvalidOperationException("Invalid selection")
        };
    }
}