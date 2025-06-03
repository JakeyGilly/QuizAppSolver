using System.Numerics;

namespace QuizAppSolver.Utils;

public class ACUtils {
    public enum ComponentType {
        Resistor,
        Capacitor,
        Inductor,
    }
    
    public class Component {
        public ComponentType Type { get; set; }
        public double Value { get; set; }
        public double Frequency { get; set; } = 50; // Default frequency in Hz

        public Complex Impedance {
            get {
                return Type switch {
                    ComponentType.Resistor => new Complex(Value, 0),
                    ComponentType.Capacitor => new Complex(0, -1 / (2 * Math.PI * Frequency * Value)),
                    ComponentType.Inductor => new Complex(0, 2 * Math.PI * Frequency * Value),
                    _ => throw new ArgumentOutOfRangeException(nameof(Type), "Unknown component type")
                };
            }
        }
        public Component(ComponentType type, double value, double frequency = 50) {
            Type = type;
            Value = value;
            Frequency = frequency;
        }
    }
    
    public static double GetComponentValueFromImpedance(Complex impedance, ComponentType type, double frequency) {
        return type switch {
            ComponentType.Resistor => impedance.Real,
            ComponentType.Inductor => impedance.Imaginary / (2 * Math.PI * frequency),
            ComponentType.Capacitor => -1 / (2 * Math.PI * frequency * impedance.Imaginary),
            _ => throw new InvalidOperationException("Unsupported component type")
        };
    }
    
    public static Complex CalculateImpedance(ComponentType type, double value, double frequency) {
        switch(type) {
            case ComponentType.Resistor:
                return new Complex(value, 0);
            case ComponentType.Capacitor:
                return new Complex(0, -1 / (2 * Math.PI * frequency * value));
            case ComponentType.Inductor:
                return new Complex(0, 2 * Math.PI * frequency * value);
            default:
                throw new ArgumentException("Invalid component type");
        }
    }
    
    public static double? CalculateFrequencyAnalytic(
        ComponentType branchOneType, double branchOneValue,
        ComponentType branchTwoType, double branchTwoValue,
        Complex inputCurrent, Complex knownCurrent,
        bool branchOneCurrentKnown)
    {
        // Determine which branch is resistor and which is reactive
        // Also assign R and reactive value accordingly
        
        double R = 0;
        double reactiveValue = 0;
        bool reactiveIsCapacitor = false;
        
        if (branchOneType == ComponentType.Resistor)
        {
            R = branchOneValue;
            if (branchTwoType == ComponentType.Capacitor || branchTwoType == ComponentType.Inductor)
            {
                reactiveValue = branchTwoValue;
                reactiveIsCapacitor = branchTwoType == ComponentType.Capacitor;
            }
            else
            {
                // Unsupported case: both branches resistors or unknown
                return null;
            }
        }
        else if (branchTwoType == ComponentType.Resistor)
        {
            R = branchTwoValue;
            if (branchOneType == ComponentType.Capacitor || branchOneType == ComponentType.Inductor)
            {
                reactiveValue = branchOneValue;
                reactiveIsCapacitor = branchOneType == ComponentType.Capacitor;
            }
            else
            {
                // Unsupported case
                return null;
            }
        }
        else
        {
            // No resistor branch - analytic solution not supported here
            return null;
        }

        // Decide which current magnitude corresponds to resistor branch
        double IinMag = inputCurrent.Magnitude;
        double IknownMag = knownCurrent.Magnitude;

        // Sanity checks
        if (IknownMag <= 0 || IinMag <= 0 || IinMag <= IknownMag)
            return null; // invalid values or no solution

        double frequency;

        if (reactiveIsCapacitor)
        {
            // f = (Iin - Iknown) / (2π * C * R * Iknown)
            frequency = (IinMag - IknownMag) / (2 * Math.PI * reactiveValue * R * IknownMag);
        }
        else
        {
            // Inductor:
            // f = Iknown / (2π * L * (Iin - Iknown))
            frequency = IknownMag / (2 * Math.PI * reactiveValue * (IinMag - IknownMag));
        }

        if (frequency <= 0)
            return null; // physically invalid frequency

        return frequency;
    }
}