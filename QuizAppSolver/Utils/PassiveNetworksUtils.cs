namespace QuizAppSolver.Utils;

public class PassiveNetworksUtils {
    public enum ImpedanceType {
        ComplexImpedance,
        Magnitude,
        PhaseAngle
    }
    
    public enum ComponentType {
        Capacitor,
        Inductor
    }

    public enum ConfigTypes {
        Normal,
        MissingOneResistor,
        MissingOneComponent
    }
}

