using System.Numerics;
using QuizAppSolver.Utils;
using Spectre.Console;
using static QuizAppSolver.Utils.ACUtils;

namespace QuizAppSolver.AC.Dividers;

public class CurrentDivider {
    // is it a voltage divider or a current divider
    // what is the input voltage/current 
    // what is the frequnecy if there is one
    // the first branch
    // do we know the voltage/current across/through the first branch
    // what is the component in the first branch
    // what is the value of the component in the first branch if we know it
    
    // the second branch
    // do we know the voltage/current across/through the second branch
    // what is the component in the second branch
    // what is the value of the component in the second branch if we know it
    
    public static void CurrentDividerMenu() {
        new UserInputBuilder()
            .AddSelection("Select the [green]Current Divider type[/]",
                val => {
                    Action action = val switch {
                        "Unknown Component Value" => UnknownComponentValue,
                        "Unknown Frequency" => UnknownFrequency,
                        "Back" => Dividers.DividersMenu,
                        _ => () => { }
                    };
                    action();
                }, ["Unknown Component Value", "Unknown Frequency", "Back"]
            ).Build();
    }
    
    private static void UnknownComponentValue() {
        double frequency = 0, branchOneComponentValue = 0, branchTwoComponentValue = 0;
        Complex inputCurrent = 0, branchOneCurrent = 0, branchTwoCurrent = 0;
        bool branchOneCurrentKnown = false, branchOneValueKnown = false;
        ComponentType knownComponentType = ComponentType.Resistor, unknownComponentType = ComponentType.Resistor;
        new UserInputBuilder().AddSelection("Which branch do you know the current through?",
            val => {
                Action action = val switch {
                    "Branch One" => () => {
                        branchOneCurrentKnown = true;
                    },
                    _ => () => { }
                };
                action();
            }, ["Branch One", "Branch Two"]
        ).AddSelection("Which branch do you know the component value for?",
            val => {
                Action action = val switch {
                    "Branch One" => () => {
                        branchOneValueKnown = true;
                    },
                    _ => () => { }
                };
                action();
            }, ["Branch One", "Branch Two"]
        ).AddSelection("What is the Component Type which is known?",
            val => {
                Action action = val switch {
                    "Resistor" => () => {
                        knownComponentType = ComponentType.Resistor;
                    },
                    "Capacitor" => () => {
                        knownComponentType = ComponentType.Capacitor;
                    },
                    "Inductor" => () => {
                        knownComponentType = ComponentType.Inductor;
                    },
                    _ => () => { }
                };
                action();
            }, ["Resistor", "Capacitor", "Inductor"]
        ).AddSelection("What is the Component type which is unknown?",
            val => {
                Action action = val switch {
                    "Resistor" => () => {
                        unknownComponentType = ComponentType.Resistor;
                    },
                    "Capacitor" => () => {
                        unknownComponentType = ComponentType.Capacitor;
                    },
                    "Inductor" => () => {
                        unknownComponentType = ComponentType.Inductor;
                    },
                    _ => () => { }
                };
                action();
            }, ["Resistor", "Capacitor", "Inductor"]
        ).Build();
        new UserInputBuilder()
            .AddCurrentInput("input", val => inputCurrent = val)
            .AddFrequencyInput("", val => frequency = val.Real)
            .AddCurrentInput("branch one", val => branchOneCurrent = val, postfix: " (shown on the ammeter)", condition: () => branchOneCurrentKnown)
            .AddCurrentInput("branch two", val => branchTwoCurrent = val, postfix: " (shown on the ammeter)", condition: () => !branchOneCurrentKnown)
            .AddComponentInput("branch one", val => branchOneComponentValue = val, condition: () => branchOneValueKnown)
            .AddComponentInput("branch two", val => branchTwoComponentValue = val, condition: () => !branchOneValueKnown) 
            .Build();
        
        Component knownComponent = new(knownComponentType, branchOneValueKnown ? branchOneComponentValue : branchTwoComponentValue, frequency);

        Complex knownCurrent = branchOneCurrentKnown ? branchOneCurrent : branchTwoCurrent;
        Complex unknownCurrent = branchOneCurrentKnown ? inputCurrent - branchOneCurrent : inputCurrent - branchTwoCurrent;

        Complex voltage = knownComponent.Impedance * knownCurrent;
        Complex unknownImpedance = voltage / unknownCurrent;

        double unknownComponentValue = GetComponentValueFromImpedance(unknownImpedance, unknownComponentType, frequency);
        
        AnsiConsole.MarkupLine($"The current in branch {(branchOneCurrentKnown ? "One" : "Two")} is {UnitConverter.ConvertToUnit(unknownCurrent, true)}");
        AnsiConsole.MarkupLine($"The unknown component value is {UnitConverter.ConvertToUnit(unknownComponentValue)}{(unknownComponentType == ComponentType.Resistor ? "Ω" : unknownComponentType == ComponentType.Capacitor ? "F" : "H")}");
    }
    
    private static void UnknownFrequency() {
        double branchOneComponentValue = 0, branchTwoComponentValue = 0;
        Complex inputCurrent = 0, branchOneCurrent = 0, branchTwoCurrent = 0;
        bool branchOneCurrentKnown = false;
        ComponentType branchOneComponentType = ComponentType.Resistor, branchTwoComponentType = ComponentType.Resistor;
        new UserInputBuilder().AddSelection("Which branch do you know the current through?",
            val => {
                Action action = val switch {
                    "Branch One" => () => {
                        branchOneCurrentKnown = true;
                    },
                    _ => () => { }
                };
                action();
            }, ["Branch One", "Branch Two"]
        ).AddSelection("What is the Component Type in Branch One?",
            val => {
                Action action = val switch {
                    "Resistor" => () => {
                        branchOneComponentType = ComponentType.Resistor;
                    },
                    "Capacitor" => () => {
                        branchOneComponentType = ComponentType.Capacitor;
                    },
                    "Inductor" => () => {
                        branchOneComponentType = ComponentType.Inductor;
                    },
                    _ => () => { }
                };
                action();
            }, ["Resistor", "Capacitor", "Inductor"]
        ).AddSelection("What is the Component type in Branch Two?",
            val => {
                Action action = val switch {
                    "Resistor" => () => {
                        branchTwoComponentType = ComponentType.Resistor;
                    },
                    "Capacitor" => () => {
                        branchTwoComponentType = ComponentType.Capacitor;
                    },
                    "Inductor" => () => {
                        branchTwoComponentType = ComponentType.Inductor;
                    },
                    _ => () => { }
                };
                action();
            }, ["Resistor", "Capacitor", "Inductor"]
        ).Build();
        new UserInputBuilder()
            .AddCurrentInput("input", val => inputCurrent = val)
            .AddCurrentInput("branch one", val => branchOneCurrent = val, postfix: " (shown on the ammeter)", condition: () => branchOneCurrentKnown)
            .AddCurrentInput("branch two", val => branchTwoCurrent = val, postfix: " (shown on the ammeter)", condition: () => !branchOneCurrentKnown)
            .AddComponentInput("branch one", val => branchOneComponentValue = val)
            .AddComponentInput("branch two", val => branchTwoComponentValue = val) 
            .Build();
        
        Complex knownCurrent = branchOneCurrentKnown ? branchOneCurrent : branchTwoCurrent;

        double FrequencyError(double frequency) {
            var Z1 = CalculateImpedance(branchOneComponentType, branchOneComponentValue, frequency);
            var Z2 = CalculateImpedance(branchTwoComponentType, branchTwoComponentValue, frequency);

            Complex IknownCalc;
            if (branchOneCurrentKnown) {
                IknownCalc = inputCurrent * (Z2 / (Z1 + Z2));
            } else {
                IknownCalc = inputCurrent * (Z1 / (Z1 + Z2));
            }
            return (IknownCalc - knownCurrent).Magnitude;
        }
        
        double FindFrequency(double lowerBound, double upperBound, double tolerance = 1e-6) {
            while(upperBound - lowerBound > tolerance) {
                double mid = (lowerBound + upperBound) / 2;
                double errorMid = FrequencyError(mid);
                double errorLow = FrequencyError(lowerBound);

                // Root is between low and mid if error changes sign or error decreases
                if (errorLow > errorMid) {
                    upperBound = mid;
                } else {
                    lowerBound = mid;
                }
            }
            return (lowerBound + upperBound) / 2;
        }
        
        double frequency = FindFrequency(1, 1_000_000);
        AnsiConsole.MarkupLine($"The frequency is {UnitConverter.ConvertToUnit(frequency, true)}Hz");
    }
}