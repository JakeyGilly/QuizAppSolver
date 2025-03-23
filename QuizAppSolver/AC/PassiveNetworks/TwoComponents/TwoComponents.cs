using System.Numerics;
using static QuizAppSolver.Utils.PassiveNetworksUtils;
using static QuizAppSolver.Utils.PassiveNetworksUtils.ComponentType;
using static QuizAppSolver.Utils.PassiveNetworksUtils.ImpedanceType;

namespace QuizAppSolver.AC.PassiveNetworks.TwoComponents;

public class TwoComponents {
    public static Complex CalculateSeries(ComponentType componentType, double componentValue, double resistance, double frequency) {
        Complex componentImpedance = componentType switch {
            Capacitor => new Complex(0, -1 / (2 * Math.PI * frequency * componentValue)),
            Inductor => new Complex(0, 2 * Math.PI * frequency * componentValue),
            _ => throw new ArgumentException("Invalid component type")
        };
        
        Complex res = new Complex(resistance, 0);
        return res + componentImpedance;
    }

    public static (Complex, Complex) CalculateSeriesUnknownComponent(ComponentType componentType, ImpedanceType impedanceType, double resistance, double frequency, double magnitude, double phaseAngle, Complex impedance) {
        impedance = impedanceType switch {
            ComplexImpedance => impedance,
            Magnitude => componentType == Capacitor
                ? new Complex(resistance, -Math.Sqrt(Math.Pow(magnitude, 2) - Math.Pow(resistance, 2)))
                : new Complex(resistance, Math.Sqrt(Math.Pow(magnitude, 2) - Math.Pow(resistance, 2))),
            PhaseAngle => componentType == Capacitor
                ? new Complex(resistance, -resistance * Math.Tan(Math.Abs(phaseAngle * Math.PI / 180)))
                : new Complex(resistance, resistance * Math.Tan(Math.Abs(phaseAngle * Math.PI / 180))),
            _ => throw new ArgumentOutOfRangeException()
        };
        
        double reactanceFactor = componentType switch {
            Capacitor => -1 / (2 * Math.PI * frequency),
            Inductor => 2 * Math.PI * frequency,
            _ => throw new InvalidOperationException("Invalid selection")
        };

        double componentImpedance = impedance.Imaginary;
        return (impedance, new Complex(0, reactanceFactor * componentImpedance)); // maybe TODO test this
    }
    
    public static (Complex, Complex) CalculateSeriesUnknownResistor(ComponentType componentType, ImpedanceType impedanceType, double componentValue, double frequency, double magnitude, double phaseAngle, Complex impedance) {
        impedance = impedanceType switch {
            ComplexImpedance => impedance,
            Magnitude => componentType == Capacitor
                ? new Complex(Math.Sqrt(Math.Pow(magnitude, 2) - Math.Pow(1 / (2 * Math.PI * frequency * componentValue), 2)), -1 / (2 * Math.PI * frequency * componentValue))
                : new Complex(Math.Sqrt(Math.Pow(magnitude, 2) - Math.Pow(2 * Math.PI * frequency * componentValue, 2)), 2 * Math.PI * frequency * componentValue),
            PhaseAngle => componentType == Capacitor
                ? new Complex(1 / (2 * Math.PI * frequency * componentValue) / Math.Tan(Math.Abs(phaseAngle * Math.PI / 180)), -1 / (2 * Math.PI * frequency * componentValue))
                : new Complex(2 * Math.PI * frequency * componentValue / Math.Tan(Math.Abs(phaseAngle * Math.PI / 180)), 2 * Math.PI * frequency * componentValue),
            _ => throw new ArgumentOutOfRangeException()
        };
        
        var resistance = impedance.Real;
        return (impedance, new Complex(resistance, 0));
    }
    
    public static Complex CalculateParallel(ComponentType componentType, double componentValue, double resistance, double frequency) {
        Complex componentImpedance = componentType switch {
            Capacitor => new Complex(0, -1 / (2 * Math.PI * frequency * componentValue)),
            Inductor => new Complex(0, 2 * Math.PI * frequency * componentValue),
            _ => throw new ArgumentException("Invalid component type")
        };
        
        Complex res = new Complex(resistance, 0);
        return 1 / (1 / res + 1 / componentImpedance);
    }
    
    public static (Complex, Complex) CalculateParallelUnknownComponent(ComponentType componentType, ImpedanceType impedanceType, double resistance, double frequency, double magnitude, double phaseAngle, Complex impedance) {
        impedance = impedanceType switch {
            ComplexImpedance => impedance,
            Magnitude => componentType == Capacitor
                ? Complex.Reciprocal(new Complex(1 / resistance, Math.Sqrt(1 / Math.Pow(magnitude, 2) - Math.Pow(1 / resistance, 2))))
                : Complex.Reciprocal(new Complex(1 / resistance, -Math.Sqrt(1 / Math.Pow(magnitude, 2) - Math.Pow(1 / resistance, 2)))),
            PhaseAngle => componentType == Capacitor
                ? new Complex(resistance * Math.Pow(Math.Cos(Math.Abs(phaseAngle * Math.PI / 180)), 2), -resistance / 2 * Math.Sin(Math.Abs(2 * phaseAngle * Math.PI / 180)))
                : new Complex(resistance * Math.Pow(Math.Cos(Math.Abs(phaseAngle * Math.PI / 180)), 2), resistance / 2 * Math.Sin(Math.Abs(2 * phaseAngle * Math.PI / 180))),
            _ => throw new ArgumentOutOfRangeException()
        };

        Complex inductImpedance = impedance*resistance / (resistance-impedance);
        return (impedance, new Complex(0, inductImpedance.Magnitude / (2 * Math.PI * frequency)));
        
    }
    
    public static (Complex, Complex) CalculateParallelUnknownResistor(ComponentType componentType, ImpedanceType impedanceType, double componentValue, double frequency, double magnitude, double phaseAngle, Complex impedance) {
        impedance = impedanceType switch {
            ComplexImpedance => impedance,
            Magnitude => componentType == Capacitor
                ? Complex.Reciprocal(new Complex(Math.Sqrt(Math.Pow(1/magnitude, 2) - Math.Pow(2 * Math.PI * frequency * componentValue, 2)), 2 * Math.PI * frequency * componentValue))
                : Complex.Reciprocal(new Complex(Math.Sqrt(Math.Pow(2 * Math.PI * frequency * componentValue, 2) - Math.Pow(magnitude, 2)) / (magnitude * 2 * Math.PI * frequency * componentValue), -1 / (2 * Math.PI * frequency * componentValue))),
            PhaseAngle => componentType == Capacitor
                ? new Complex(Math.Sin(Math.Abs(2 * phaseAngle * Math.PI / 180)) / (4 * Math.PI * frequency * componentValue), -Math.Pow(Math.Sin(Math.Abs(phaseAngle * Math.PI / 180)), 2) / (2 * Math.PI * frequency * componentValue))
                : new Complex(Math.Sin(Math.Abs(2 * phaseAngle * Math.PI / 180)) / (2 / (2 * Math.PI * frequency * componentValue)), Math.Pow(Math.Sin(Math.Abs(phaseAngle * Math.PI / 180)), 2) / (1 / (2 * Math.PI * frequency * componentValue))),
            _ => throw new ArgumentOutOfRangeException(),
        };
        
        return (impedance, new Complex(1 / (1 / impedance).Real, 0));
    }
}