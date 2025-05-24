using System.Numerics;
using Spectre.Console;

namespace QuizAppSolver.AC.PassiveNetworks.RCandRL;

public class ArrangeCircuit() {
    public static void ArrangeRLandRC() {
        int numResistors = 0, numComponents = 0, numRequiredComponents = 0;
        double frequency = 0;
        Complex requiredImpedance = 0;
        List<double> resistances = [];
        List<double> componentValues = [];
        string componentType = "";
        new UserInputBuilder()
            .AddSelection("Select the component type",
                val => componentType = val,
                ["Capacitor", "Inductor"])
            .AddFrequencyInput("", val => frequency = val.Real)
            .AddComplexImpedanceInput("required", val => requiredImpedance = val)
            .AddNumericInput("How many components are needed?", val => numRequiredComponents = (int)Math.Floor(val))
            .AddNumericInput("How many resistors are there?", val => numResistors = (int)Math.Floor(val))
            .AddNumericInput("How many components are there?", val => numComponents = (int)Math.Floor(val))
            .Build();

        if (numResistors > 0) {
            new UserInputBuilder()
                .AddMultipleResistorInput("Enter resistor values:", numResistors, val => resistances = val)
                .Build();
        }

        if (numComponents > 0) {
            new UserInputBuilder()
                .AddMultipleComponentInput("Enter component values:", numComponents, val => componentValues = val)
                .Build();
        }

        List<Complex> componentImpedances = [];
        for (int i = 0; i < numComponents; i++) {
            componentImpedances.Add(componentType switch {
                "Capacitor" => new Complex(0, -1 / (2 * Math.PI * frequency * componentValues[i])),
                "Inductor" => new Complex(0, 2 * Math.PI * frequency * componentValues[i]),
                _ => throw new ArgumentException("Invalid component type")
            });
        }


        void TwoComponents() {
            List<double> results;
            double bestAbsDiff = double.MaxValue;
            string bestConfig = "";
            if (numRequiredComponents != 2) return;
            if (numResistors == 1 && numComponents == 1) {
                results = OneResistorOneComponent(resistances, componentImpedances);
                CompareResults(results, "One Resistor, One Component", ref bestAbsDiff, ref bestConfig);
            } else if (numResistors == 2 && numComponents == 0) {
                results = TwoResistors(resistances);
                CompareResults(results, "Two Resistors", ref bestAbsDiff, ref bestConfig);
            } else if (numResistors == 0 && numComponents == 2) {
                results = TwoComponents(componentImpedances);
                CompareResults(results, "Two Components", ref bestAbsDiff, ref bestConfig);
            } else if (numResistors > 1 && numComponents == 1) {
                for (int i = 0; i < numResistors; i++) {
                    results = OneResistorOneComponent([resistances[i]], componentImpedances);
                    CompareResults(results, $"Resistor {UnitConverter.ConvertToUnit(resistances[i])}, Component {UnitConverter.ConvertToUnit(componentValues[0])}", ref bestAbsDiff, ref bestConfig);
                }
            } else if (numResistors == 1 && numComponents > 1) {
                for (int i = 0; i < numComponents; i++) {
                    results = OneResistorOneComponent(resistances, [componentImpedances[i]]);
                    CompareResults(results, $"Resistor {UnitConverter.ConvertToUnit(resistances[0])}, Component {UnitConverter.ConvertToUnit(componentValues[i])}", ref bestAbsDiff, ref bestConfig);
                }
            } else if (numResistors == 0 && numComponents > 2) {
                for (int i = 0; i < numComponents; i++) {
                    for (int j = 1; j < numComponents; j++) {
                        if (j == componentValues.Count) break;
                        results = TwoComponents([componentImpedances[i], componentImpedances[j]]);
                        CompareResults(results, $"Component {UnitConverter.ConvertToUnit(componentValues[i])}, Component {UnitConverter.ConvertToUnit(componentValues[j])}", ref bestAbsDiff, ref bestConfig);
                    }
                }
            } else if (numResistors > 2 && numComponents == 0) {
                for (int i = 0; i < numResistors; i++) {
                    for (int j = 1; j < numResistors; j++) {
                        if (j == resistances.Count) break;
                        results = TwoResistors([resistances[i], resistances[j]]);
                        CompareResults(results, $"Resistor {resistances[i]}, Resistor {resistances[j]}", ref bestAbsDiff, ref bestConfig);
                    }
                }
            }

            Console.WriteLine($"Best configuration: {bestConfig}");
            Console.WriteLine($"Best Abs Diff: {bestAbsDiff}");

            void CompareResults(List<double> results, string config, ref double bestAbsDiff, ref string bestConfig) {
                double seriesAbsDiff = results[0];
                double parallelAbsDiff = results[1];
                if (seriesAbsDiff < bestAbsDiff) {
                    bestAbsDiff = seriesAbsDiff;
                    bestConfig = config + " (Series)";
                }
                if (parallelAbsDiff < bestAbsDiff) {
                    bestAbsDiff = parallelAbsDiff;
                    bestConfig = config + " (Parallel)";
                }
            }

            List<double> TwoResistors(List<double> resistances) {
                if (resistances.Count != 2) throw new ArgumentException("Two resistors are required");
                // Series Configuration
                Complex seriesImpedance = resistances[0] + resistances[1];
                double seriesAbsDiff = Complex.Abs(seriesImpedance - requiredImpedance);

                // Parallel Configuration
                Complex parallelImpedance = 1 / (1 / resistances[0] + 1 / resistances[1]);
                double parallelAbsDiff = Complex.Abs(parallelImpedance - requiredImpedance);

                return [seriesAbsDiff, parallelAbsDiff];
            }

            List<double> TwoComponents(List<Complex> componentImpedances) {
                if (componentImpedances.Count != 2) throw new ArgumentException("Two components are required");
                // Series Configuration
                Complex seriesImpedance = componentImpedances[0] + componentImpedances[1];
                double seriesAbsDiff = Complex.Abs(seriesImpedance - requiredImpedance);

                // Parallel Configuration
                Complex parallelImpedance = 1 / (1 / componentImpedances[0] + 1 / componentImpedances[1]);
                double parallelAbsDiff = Complex.Abs(parallelImpedance - requiredImpedance);

                return [seriesAbsDiff, parallelAbsDiff];
            }

            List<double> OneResistorOneComponent(List<double> resistances, List<Complex> componentImpedances) {
                if (resistances.Count != 1 || componentImpedances.Count != 1) throw new ArgumentException("One resistor and one component are required");
                // Series Configuration
                Complex seriesImpedance = resistances[0] + componentImpedances[0];
                double seriesAbsDiff = Complex.Abs(seriesImpedance - requiredImpedance);

                // Parallel Configuration
                Complex parallelImpedance = 1 / (1 / resistances[0] + 1 / componentImpedances[0]);
                double parallelAbsDiff = Complex.Abs(parallelImpedance - requiredImpedance);

                return [seriesAbsDiff, parallelAbsDiff];
            }
        }

        void ThreeComponents() {
            (string description, Complex impedance) results = ("", new Complex());
            double bestAbsDiff = double.MaxValue;
            string bestConfig = "";
            string bestDescription = "";
            if (numRequiredComponents != 3) return;
            if (numResistors == 1 && numComponents == 2) {
                Console.WriteLine("One Resistor, Two Components");
                results = OneResistorTwoComponents(resistances, componentImpedances);
                CompareResults(results, "One Resistor, Two Components", ref bestAbsDiff, ref bestConfig, ref bestDescription);
            } else if (numResistors == 2 && numComponents == 1) {
                Console.WriteLine("Two Resistors, One Component");
                results = TwoResistorsOneComponent(resistances, componentImpedances);
                CompareResults(results, "Two Resistors, One Component", ref bestAbsDiff, ref bestConfig, ref bestDescription);
            } else if (numResistors == 0 && numComponents == 3) {
                Console.WriteLine("Three Components");
                results = ThreeComponents(componentImpedances);
                CompareResults(results, "Three Components", ref bestAbsDiff, ref bestConfig, ref bestDescription);
            } else if (numResistors == 3 && numComponents == 0) {
                Console.WriteLine("Three Resistors");
                results = ThreeResistors(resistances);
                CompareResults(results, "Three Resistors", ref bestAbsDiff, ref bestConfig, ref bestDescription);
            } else if (numResistors > 2 && numComponents == 1) {
                Console.WriteLine("Lots Resistors, One Component, but 3 Components");
                for (int i = 0; i < numResistors; i++) {
                    for (int j = 1; j < numResistors; j++) {
                        if (j == resistances.Count) break;
                        Console.WriteLine($"trying resistors {resistances[i]} and {resistances[j]} and component {componentImpedances[0]}");
                        results = TwoResistorsOneComponent([resistances[i], resistances[j]], componentImpedances);
                        CompareResults(results, $"Resistor {UnitConverter.ConvertToUnit(resistances[i])}, Resistor {UnitConverter.ConvertToUnit(resistances[j])}, Component {UnitConverter.ConvertToUnit(componentValues[0])}", ref bestAbsDiff, ref bestConfig, ref bestDescription);
                    }
                }
            } else if (numResistors == 1 && numComponents > 1) {
                Console.WriteLine("One Resistor, Lots Components, but 3 Components");
                for (int i = 0; i < numComponents; i++) {
                    for (int j = 1; j < numComponents; j++) {
                        if (j == componentImpedances.Count) break;
                        Console.WriteLine($"trying resistor {resistances[0]} and components {componentImpedances[i]} and {componentImpedances[j]}");
                        results = OneResistorTwoComponents(resistances, [componentImpedances[i], componentImpedances[j]]);
                        CompareResults(results, $"Resistor {UnitConverter.ConvertToUnit(resistances[0])}, Component {UnitConverter.ConvertToUnit(componentValues[i])}, Component {UnitConverter.ConvertToUnit(componentValues[j])}", ref bestAbsDiff, ref bestConfig, ref bestDescription);
                    }
                }
            } else if (numResistors == 0 && numComponents > 3) {
                Console.WriteLine("Lots Components, but 3 Components");
                for (int i = 0; i < numComponents; i++) {
                    for (int j = 1; j < numComponents; j++) {
                        for (int k = 2; k < numComponents; k++) {
                            if (k == componentImpedances.Count) break;
                            Console.WriteLine($"trying components {componentImpedances[i]} and {componentImpedances[j]} and {componentImpedances[k]}");
                            results = ThreeComponents([componentImpedances[i], componentImpedances[j], componentImpedances[k]]);
                            CompareResults(results, $"Component {UnitConverter.ConvertToUnit(componentValues[i])}, Component {UnitConverter.ConvertToUnit(componentValues[j])}, Component {UnitConverter.ConvertToUnit(componentValues[k])}", ref bestAbsDiff, ref bestConfig, ref bestDescription);
                        }
                    }
                }
            } else if (numResistors > 2 && numComponents == 0) {
                Console.WriteLine("Lots Resistors, but 3 Components");
                for (int i = 0; i < numResistors; i++) {
                    for (int j = 1; j < numResistors; j++) {
                        for (int k = 2; k < numResistors; k++) {
                            if (k == resistances.Count) break;
                            Console.WriteLine($"trying resistors {resistances[i]} and {resistances[j]} and {resistances[k]}");
                            results = ThreeResistors([resistances[i], resistances[j], resistances[k]]);
                            CompareResults(results, $"Resistor {UnitConverter.ConvertToUnit(resistances[i])}, Resistor {UnitConverter.ConvertToUnit(resistances[j])}, Resistor {UnitConverter.ConvertToUnit(resistances[k])}", ref bestAbsDiff, ref bestConfig, ref bestDescription);
                        }
                    }
                }
            }

            Console.WriteLine($"Best configuration: {bestConfig}");
            Console.WriteLine($"Best Abs Diff: {bestAbsDiff}");
            Console.WriteLine($"Best Description: {bestDescription}");

            void CompareResults((string description, Complex impedance) results, string config, ref double bestAbsDiff, ref string bestConfig, ref string bestDescription) {
                double absDiff = Complex.Abs(results.impedance - requiredImpedance);
                if (absDiff < bestAbsDiff) {
                    bestAbsDiff = absDiff;
                    bestConfig = config;
                    bestDescription = results.description;
                }
            }

            (string description, Complex impedance) OneResistorTwoComponents(List<double> resistances, List<Complex> componentImpedances) {
                if (resistances.Count != 1 || componentImpedances.Count != 2) throw new ArgumentException("One resistor and two components are required");
                Complex res = resistances[0];
                var circuitOptions = new (string description, Complex impedance)[] {
                    ("Resistor in parallel with (Component 1 + Component 2 in series)",
                        1 / (1 / res + 1 / (componentImpedances[0] + componentImpedances[1]))),
                    ("Resistor in parallel with Component 1, in series with Component 2",
                        1 / (1 / res + 1 / componentImpedances[0]) + componentImpedances[1]),
                    ("Resistor in parallel with Component 2, in series with Component 1",
                        1 / (1 / res + 1 / componentImpedances[1]) + componentImpedances[0]),
                    ("Resistor in series with (Component 1 in parallel with Component 2)",
                        res + 1 / (1 / componentImpedances[0] + 1 / componentImpedances[1])),
                    ("Component 2 in parallel with (Component 1 + Resistor in series)",
                        1 / (1 / componentImpedances[1] + 1 / (componentImpedances[0] + res))),
                    ("Component 1 in parallel with (Component 2 + Resistor in series)",
                        1 / (1 / componentImpedances[0] + 1 / (componentImpedances[1] + res))),
                    ("All components in parallel",
                        1 / (1 / res + 1 / componentImpedances[0] + 1 / componentImpedances[1])),
                    ("ALl components in series",
                        res + componentImpedances[0] + componentImpedances[1])
                };

                // Find the best match
                var bestMatch = circuitOptions.MinBy(option => Complex.Abs(option.impedance - requiredImpedance));
                return bestMatch;
            }

            (string description, Complex impedance) TwoResistorsOneComponent(List<double> resistances, List<Complex> componentImpedances) {
                if (resistances.Count != 2 || componentImpedances.Count != 1) throw new ArgumentException("Two resistors and one component are required");
                var circuitOptions = new (string description, Complex impedance)[] {
                    ("Component in parallel with (Resistor 1 + Resistor 2 in series)",
                        1 / (1 / componentImpedances[0] + 1 / (resistances[0] + resistances[1]))),
                    ("Component in parallel with Resistor 1, in series with Resistor 2",
                        1 / (1 / componentImpedances[0] + 1 / resistances[0]) + resistances[1]),
                    ("Component in parallel with Resistor 2, in series with Resistor 1",
                        1 / (1 / componentImpedances[0] + 1 / resistances[1]) + resistances[0]),
                    ("Component in series with (Resistor 1 in parallel with Resistor 2)",
                        componentImpedances[0] + 1 / (1 / resistances[0] + 1 / resistances[1])),
                    ("Resistor 2 in parallel with (Resistor 1 + Component in series)",
                        1 / (1 / resistances[1] + 1 / (resistances[0] + componentImpedances[0]))),
                    ("Resistor 1 in parallel with (Resistor 2 + Component in series)",
                        1 / (1 / resistances[0] + 1 / (resistances[1] + componentImpedances[0]))),
                    ("All components in parallel",
                        1 / (1 / componentImpedances[0] + 1 / resistances[0] + 1 / resistances[1])),
                    ("All components in series",
                        componentImpedances[0] + resistances[0] + resistances[1])
                };

                // Find the best match
                var bestMatch = circuitOptions.MinBy(option => Complex.Abs(option.impedance - requiredImpedance));
                return bestMatch;
            }

            (string description, Complex impedance) ThreeComponents(List<Complex> componentImpedances) {
                if (componentImpedances.Count != 3) throw new ArgumentException("Three components are required");
                var circuitOptions = new (string description, Complex impedance)[] {
                    ("Component 1 in parallel with (Component 2 + Component 3 in series)",
                        1 / (1 / componentImpedances[0] + 1 / (componentImpedances[1] + componentImpedances[2]))),
                    ("Component 3 in parallel with (Component 2 + Component 1 in series)",
                        1 / (1 / componentImpedances[2] + 1 / (componentImpedances[1] + componentImpedances[0]))),
                    ("Component 2 in parallel with (Component 1 + Component 3 in series)",
                        1 / (1 / componentImpedances[1] + 1 / (componentImpedances[0] + componentImpedances[2]))),
                    ("Component 1 in parallel with Component 2, in series with Component 3",
                        1 / (1 / componentImpedances[0] + 1 / componentImpedances[1]) + componentImpedances[2]),
                    ("Component 1 in parallel with Component 3, in series with Component 2",
                        1 / (1 / componentImpedances[0] + 1 / componentImpedances[2]) + componentImpedances[1]),
                    ("Component 1 in series with (Component 2 in parallel with Component 3)",
                        1 / (1 / componentImpedances[0] + 1 / (componentImpedances[1] + componentImpedances[2]))),
                    ("Component 1 in series with (Component 2 in parallel with Component 3)",
                        componentImpedances[0] + 1 / (1 / componentImpedances[1] + 1 / componentImpedances[2])),
                    ("All components in parallel",
                        1 / (1 / componentImpedances[0] + 1 / componentImpedances[1] + 1 / componentImpedances[2])),
                    ("All components in series",
                        componentImpedances[0] + componentImpedances[1] + componentImpedances[2])
                };

                // Find the best match
                var bestMatch = circuitOptions.MinBy(option => Complex.Abs(option.impedance - requiredImpedance));
                return bestMatch;
            }

            (string description, Complex impedance) ThreeResistors(List<double> resistances) {
                if (resistances.Count != 3) throw new ArgumentException("Three resistors are required");
                var circuitOptions = new (string description, Complex impedance)[] {
                    ("Resistor 1 in parallel with (Resistor 2 + Resistor 3 in series)",
                        1 / (1 / resistances[0] + 1 / (resistances[1] + resistances[2]))),
                    ("Resistor 3 in parallel with (Resistor 2 + Resistor 1 in series)",
                        1 / (1 / resistances[2] + 1 / (resistances[1] + resistances[0]))),
                    ("Resistor 2 in parallel with (Resistor 1 + Resistor 3 in series)",
                        1 / (1 / resistances[1] + 1 / (resistances[0] + resistances[2]))),
                    ("Resistor 1 in parallel with Resistor 2, in series with Resistor 3",
                        1 / (1 / resistances[0] + 1 / resistances[1]) + resistances[2]),
                    ("Resistor 1 in parallel with Resistor 3, in series with Resistor 2",
                        1 / (1 / resistances[0] + 1 / resistances[2]) + resistances[1]),
                    ("Resistor 1 in series with (Resistor 2 in parallel with Resistor 3)",
                        1 / (1 / resistances[0] + 1 / (resistances[1] + resistances[2]))),
                    ("Resistor 1 in series with (Resistor 2 in parallel with Resistor 3)",
                        resistances[0] + 1 / (1 / resistances[1] + 1 / resistances[2])),
                    ("All resistors in parallel",
                        1 / (1 / resistances[0] + 1 / resistances[1] + 1 / resistances[2])),
                    ("All resistors in series",
                        resistances[0] + resistances[1] + resistances[2])
                };

                // Find the best match
                var bestMatch = circuitOptions.MinBy(option => Complex.Abs(option.impedance - requiredImpedance));
                return bestMatch;
            }
        }

        TwoComponents();
        ThreeComponents();

        bool menu = AnsiConsole.Confirm("Back to the main menu");
        if (menu) Program.Main();
    }
}