using System.Numerics;
using static QuizAppSolver.AC.PassiveNetworks.MoreComponents.OneResistorTwoComponents;
using static QuizAppSolver.Utils.PassiveNetworksUtils;
using static QuizAppSolver.Utils.PassiveNetworksUtils.ConfigTypes;
using static QuizAppSolver.Utils.PassiveNetworksUtils.ComponentType;

namespace QuizAppSolverTests.PassiveNetworksTests;

public class OneResistorTwoComponentsTests {
    [TestCase(Normal, new[] { 10e-6, 10e-6 }, 68, 70, 0, 0, Capacitor, 459.8, -81.5)]
    [TestCase(Normal, new[] { 1e-9, 1e-9 }, 18e3, 10e3, 0, 0, Capacitor, 36.57e3, -60.5)]
    [TestCase(MissingOneComponent, new[] { 100e-9 }, 1e3, 3e3, 1.458e3, -46.7, Capacitor, 100e-9, 0)]
    [TestCase(MissingOneComponent, new[] { 47e-15 }, 390e3, 9e6, 629.5e3, -51.7, Capacitor, 150e-15, 0)]
    [TestCase(MissingOneComponent, new[] { 33e-12 }, 6.8e3, 2e6, 7.943e3, -31.1, Capacitor, 47e-12, 0)]
    [TestCase(MissingOneResistor, new[] { 100e-9, 100e-9 }, 0, 3e3, 2.089e3, -30.5, Capacitor, 1.8e3, 0)] // this is repeated
    [TestCase(MissingOneResistor, new[] { 33e-12, 47e-12 }, 0, 2e6, 7.943e3, -31.1, Capacitor, 6.8e3, 0)] // this is repeated
    [TestCase(Normal, new[] { 100.0, 33.0 }, 12e3, 50, 0, 0, Inductor, 43.47e3, 74)]
    [TestCase(Normal, new[] { 220e-3, 330e-3 }, 33e3, 20e3, 0, 0, Inductor, 76.59e3, 64.5)]
    [TestCase(Normal, new[] { 15e-3, 10e-3 }, 22e3, 300e3, 0, 0, Inductor, 52.01e3, 65)]
    [TestCase(MissingOneResistor, new[] { 680e-3, 1.5 }, 0, 100e3, 1.375e6, 85, Inductor, 120e3, 0)]
    [TestCase(MissingOneComponent, new[] { 100e-3 }, 820e3, 1e6, 1.501e6, 56.9, Inductor, 100e-3, 0)]
    [TestCase(MissingOneComponent, new[] { 22.0 }, 150e3, 3e3, 1.703e6, 84.9, Inductor, 68, 0)]
    [TestCase(MissingOneComponent, new[] { 220e-6 }, 82, 90e3, 398.7, 78.1, Inductor, 470e-6, 0)]
    public void TestSeries(ConfigTypes type, double[] componentValues, double resistance, double frequency, double impedanceMagnitude, double impedanceAngle, ComponentType componentType, double expectedResultMagnitude, double expectedResultAngle) {
        var impedance = Complex.FromPolarCoordinates(impedanceMagnitude, impedanceAngle / 180 * Math.PI);
        Complex result = CalculateSeries(type, componentValues.ToList(), resistance, frequency, impedance, componentType);
        var angleTolerance = Math.Abs(result.Phase * 10e-3); // tolerance of 1%
        var tolerance = type switch {
            Normal => result.Magnitude * 5e-3, // tolerance of 0.5%
            MissingOneResistor => result.Real * 5e-3, // tolerance of 0.5%
            MissingOneComponent => result.Imaginary * 5e-3, // tolerance of 0.5%
            _ => 0
        };
        var expectedResult = Complex.FromPolarCoordinates(expectedResultMagnitude, expectedResultAngle / 180 * Math.PI);
        switch (type) {
            case Normal:
                Assert.That(result.Magnitude, Is.EqualTo(expectedResult.Magnitude).Within(tolerance));
                Assert.That(result.Phase, Is.EqualTo(expectedResult.Phase).Within(angleTolerance));
                break;
            case MissingOneResistor:
                Assert.That(result.Real, Is.EqualTo(expectedResult.Magnitude).Within(tolerance));
                break;
            case MissingOneComponent:
                Assert.That(result.Imaginary, Is.EqualTo(expectedResult.Magnitude).Within(tolerance));
                break;
            default:
                Assert.Fail("Invalid type");
                break;
        }
    }
    
    [TestCase(Normal, new[] { 330e-6, 330e-6 }, 330, 10, 0, 0, Capacitor, 24.05, -85.8)]
    [TestCase(Normal, new[] { 150e-9, 47e-9 }, 270, 5e3, 0, 0, Capacitor, 138.6, -59.1)]
    [TestCase(Normal, new[] { 330e-9, 680e-9 }, 820, 1e3, 0, 0, Capacitor, 154.7, -79.1)]
    [TestCase(MissingOneResistor, new[] { 220e-9, 150e-9 }, 0, 20, 20.51e3, -72.4, Capacitor, 68e3, 0)]
    [TestCase(MissingOneComponent, new[] { 150e-9 }, 560e3, 10, 91.29e3, -80.6, Capacitor, 22e-9, 0)]
    [TestCase(MissingOneComponent, new[] { 6.8e-9 }, 6.8e3, 3e3, 2.864e3, -65.1, Capacitor, 10e-9, 0)]
    [TestCase(Normal, new[] { 1, 330e-3 }, 68e3, 20e3, 0, 0, Inductor, 28.34e3, 65.4)]
    [TestCase(Normal, new[] { 100e-3, 47e-3 }, 68, 600, 0, 0, Inductor, 59.23, 29.4)]
    [TestCase(Normal, new[] { 6.8, 1.5 }, 330e3, 20e3, 0, 0, Inductor, 139.9e3, 64.9)]
    [TestCase(Normal, new[] { 680e-6, 1.5e-3 }, 56e3, 5e6, 0, 0, Inductor, 14.22e3, 75.3)]
    [TestCase(Normal, new[] { 10, 3.3 }, 220e3, 2e3, 0, 0, Inductor, 30.87e3, 81.9)]
    [TestCase(MissingOneResistor, new[] { 15e-6, 47e-6 }, 0, 2e6, 71.12, 29.8, Inductor, 82, 0)]
    [TestCase(MissingOneResistor, new[] { 100e-6, 47e-6 }, 0, 3e6, 581.3, 74.7, Inductor, 2.2e3, 0)]
    [TestCase(MissingOneResistor, new[] { 150.0, 68.0 }, 0, 400, 98.45e3, 56.8, Inductor, 180e3, 0)]
    [TestCase(MissingOneResistor, new[] { 1.5e-3, 6.8e-3 }, 0, 2e6, 12.64e3, 54.9, Inductor, 22e3, 0)]
    [TestCase(MissingOneResistor, new[] { 22e-3, 15e-3 }, 0, 10e3, 558.5, 85.3, Inductor, 6.8e3, 0)]
    [TestCase(MissingOneComponent, new[] { 4.7e-3 }, 3.3e3, 600e3, 2.849e3, 30.3, Inductor, 2.2e-3, 0)]
    [TestCase(MissingOneComponent, new[] { 470e-3 }, 1e6, 600e3, 491.9e3, 60.5, Inductor, 220e-3, 0)]
    [TestCase(MissingOneComponent, new[] { 10e-3 }, 12e3, 400e3, 5.533e3, 62.5, Inductor, 3.3e-3, 0)]
    [TestCase(MissingOneComponent, new[] { 4.7 }, 1.5e6, 10e3, 173.4e3, 83.4, Inductor, 6.8, 0)]
    [TestCase(MissingOneComponent, new[] { 100e-6 }, 470, 1e6, 318, 47.4, Inductor, 220e-6, 0)]
    public void TestParallel(ConfigTypes type, double[] componentValues, double resistance, double frequency, double impedanceMagnitude, double impedanceAngle, ComponentType componentType, double expectedResultMagnitude, double expectedResultAngle) {
        var impedance = Complex.FromPolarCoordinates(impedanceMagnitude, impedanceAngle / 180 * Math.PI);
        Complex result = CalculateParallel(type, componentValues.ToList(), resistance, frequency, impedance, componentType);
        var angleTolerance = Math.Abs(result.Phase * 10e-3); // tolerance of 1%
        var tolerance = type switch {
            Normal => result.Magnitude * 5e-3, // tolerance of 0.5%
            MissingOneResistor => result.Real * 5e-3, // tolerance of 0.5%
            MissingOneComponent => result.Imaginary * 5e-3, // tolerance of 0.5%
            _ => 0
        };
        var expectedResult = Complex.FromPolarCoordinates(expectedResultMagnitude, expectedResultAngle / 180 * Math.PI);
        switch (type) {
            case Normal:
                Assert.That(result.Magnitude, Is.EqualTo(expectedResult.Magnitude).Within(tolerance));
                Assert.That(result.Phase, Is.EqualTo(expectedResult.Phase).Within(angleTolerance));
                break;
            case MissingOneResistor:
                Assert.That(result.Real, Is.EqualTo(expectedResult.Magnitude).Within(tolerance));
                break;
            case MissingOneComponent:
                Assert.That(result.Imaginary, Is.EqualTo(expectedResult.Magnitude).Within(tolerance));
                break;
            default:
                Assert.Fail("Invalid type");
                break;
        }
    }
    
    [TestCase(Normal, new[] { 22e-3, 47e-3 }, 27e3, 90e3, 0, 0, Inductor, 29.14e3, 62.9, false)]
    [TestCase(Normal, new[] { 100.0, 68.0 }, 560e3, 300, 0, 0, Inductor, 311.5e3, 84.9, false)]
    [TestCase(Normal, new[] { 22e-3, 100e-3 }, 180e3, 400e3, 0, 0, Inductor, 184.1e3, 49.7, false)]
    [TestCase(Normal, new[] { 33.0, 220.0 }, 2.2e6, 2e3, 0, 0, Inductor, 2.006e6, 47.82, false)]
    [TestCase(Normal, new[] { 680e-3, 220e-3 }, 560, 200, 0, 0, Inductor, 1.082e3, 84.18, false)]
    [TestCase(MissingOneResistor, new[] { 150e-6, 150e-6 }, 0, 600e3, 705.6, 71.8, Inductor, 270, 0, false)]
    [TestCase(MissingOneResistor, new[] { 1, 220e-3 }, 0, 3e3, 22.73e3, 87.3, Inductor, 15e3, 0, false)]
    [TestCase(MissingOneComponent, new[] { 1.5e3 }, 680e3, 50, 819.8e3, 74.4, Inductor, 1.5e3, 0, false)]
    [TestCase(MissingOneComponent, new[] { 150.0 }, 150e3, 1e3, 976.8e3, 81.4, Inductor, 150, 0, true)]
    [TestCase(MissingOneComponent, new[] { 330e-3 }, 10e3, 5e3, 11e3, 61.9, Inductor, 150e-3, 0, true)]
    [TestCase(MissingOneComponent, new[] { 4.7 }, 18e3, 800, 22.79e3, 60, Inductor, 2.2, 0, true)]
    [TestCase(Normal, new[] { 6.8e-9, 4.7e-9 }, 10e3, 2e3, 0, 0, Capacitor, 17.71e3, -65.2, false)]
    [TestCase(Normal, new[] { 15e-9, 3.3e-9 }, 820, 20e3, 0, 0, Capacitor, 1.072e3, -46.7, false)] // these are repeated
    [TestCase(Normal, new[] { 100e-15, 15e-15 }, 470e3, 9e6, 0, 0, Capacitor, 528.3e3, -39.9, false)] // these are repeated
    [TestCase(MissingOneResistor, new[] { 15e-9, 3.3e-9 }, 0, 20e3, 1.072e3, -46.7, Capacitor, 820, 0, false)] // these are repeated
    [TestCase(MissingOneResistor, new[] { 100e-15, 15e-15 }, 0, 9e6, 528.3e3, -39.9, Capacitor, 470e3, 0, false)]// these are repeated
    [TestCase(MissingOneComponent, new[] { 15e-9 }, 820, 20e3, 1.072e3, -46.7, Capacitor, 3.3e-9, 0, false)] // these are repeated
    [TestCase(MissingOneComponent, new[] { 100e-15 }, 470e3, 9e6, 528.3e3, -39.9, Capacitor, 15e-15, 0, false)] // these are repeated
    [TestCase(MissingOneComponent, new[] { 3.3e-9 }, 820, 20e3, 1.072e3, -46.7, Capacitor, 15e-9, 0, true)] // these are repeated
    [TestCase(MissingOneComponent, new[] { 15e-15 }, 470e3, 9e6, 528.3e3, -39.9, Capacitor, 100e-15, 0, true)] // these are repeated
    public void TestComponentSeriesComponentResParallel(ConfigTypes type, double[] componentValues, double resistance, double frequency, double impedanceMagnitude, double impedanceAngle, ComponentType componentType, double expectedResultMagnitude, double expectedResultAngle, bool isMissingComponentInSeries) { 
        var impedance = Complex.FromPolarCoordinates(impedanceMagnitude, impedanceAngle / 180 * Math.PI);
        Complex result = CalculateComponentSeriesComponentResParallel(type, componentValues.ToList(), resistance, frequency, impedance, componentType, isMissingComponentInSeries);
        var angleTolerance = Math.Abs(result.Phase * 10e-3); // tolerance of 1%
        var tolerance = type switch {
            Normal => result.Magnitude * 5e-3, // tolerance of 0.5%
            MissingOneResistor => result.Real * 5e-3, // tolerance of 0.5%
            MissingOneComponent => result.Imaginary * 5e-3, // tolerance of 0.5%
            _ => 0
        };
        var expectedResult = Complex.FromPolarCoordinates(expectedResultMagnitude, expectedResultAngle / 180 * Math.PI);
        switch (type) {
            case Normal:
                Assert.That(result.Magnitude, Is.EqualTo(expectedResult.Magnitude).Within(tolerance));
                Assert.That(result.Phase, Is.EqualTo(expectedResult.Phase).Within(angleTolerance));
                break;
            case MissingOneResistor:
                Assert.That(result.Real, Is.EqualTo(expectedResult.Magnitude).Within(tolerance));
                break;
            case MissingOneComponent:
                Assert.That(result.Imaginary, Is.EqualTo(expectedResult.Magnitude).Within(tolerance));
                break;
            default:
                Assert.Fail("Invalid type");
                break;
        }
    }
    
    [TestCase(Normal, new[] { 100e-6, 220e-6 }, 82, 100e3, 0, 0, Inductor, 65.76, 59.6, false)]
    [TestCase(Normal, new[] { 22.0, 150.0 }, 4.7e3, 20, 0, 0, Inductor, 4.647e3, 42.7, false)]
    [TestCase(Normal, new[] { 330e-3, 68e-3 }, 470e3, 400e3, 0, 0, Inductor, 147.4e3, 85.6, false)]
    [TestCase(Normal, new[] { 330e-6, 220e-6 }, 2.7e3, 7e6, 0, 0, Inductor, 5.869e3, 85.8, false)]
    [TestCase(Normal, new[] { 220e-3, 220e-3 }, 1e3, 4e3, 0, 0, Inductor, 2.798e3, 84.9, false)]
    [TestCase(MissingOneResistor, new[] { 150.0, 68.0 }, 0, 20, 7.572e3, 80, Inductor, 33e3, 0, false)]
    [TestCase(MissingOneResistor, new[] { 47e-3, 15e-3 }, 0, 6e6, 484.1e3, 82.1, Inductor, 1.8e6, 0, false)]
    [TestCase(MissingOneComponent, new[] { 2.2e3 }, 1e6, 100, 750.6e3, 66.6, Inductor, 1.5e3, 0, true)]
    [TestCase(MissingOneComponent, new[] { 4.7e-3 }, 18e3, 600e3, 12.7e3, 82.5, Inductor, 10e-3, 0, true)]
    [TestCase(MissingOneComponent, new[] { 4.7e-3 }, 39e3, 600e3, 14.4e3, 72.2, Inductor, 4.7e-3, 0, false)]
    [TestCase(MissingOneComponent, new[] { 680e-3 }, 220e3, 60e3, 224.5e3, 64.4, Inductor, 1.5, 0, false)]
    [TestCase(Normal, new[] { 47e-15, 150e-15 }, 120e3, 8e6, 0, 0, Capacitor, 102.6e3, -86.35, false)]
    [TestCase(MissingOneResistor, new[] { 47e-15, 150e-15 }, 0, 8e6, 102.6e3, -86.35, Capacitor, 120e3, 0, false)] // this is repeated
    [TestCase(MissingOneComponent, new[] { 22e-12 }, 1.5e3, 10e6, 605.3, -67.4, Capacitor, 47e-12, 0, true)]
    [TestCase(MissingOneComponent, new[] { 47e-15 }, 120e3, 8e6, 102.6e3, -86.35, Capacitor, 150e-15, 0, false)] // this is repeated
    [TestCase(MissingOneComponent, new[] { 150e-15}, 120e3, 8e6, 102.6e3, -86.35, Capacitor, 47e-15, 0, true)] // this is repeated
    public void TestComponentParallelComponentResSeries(ConfigTypes type, double[] componentValues, double resistance, double frequency, double impedanceMagnitude, double impedanceAngle, ComponentType componentType, double expectedResultMagnitude, double expectedResultAngle, bool isMissingComponentInSeries) {
        var impedance = Complex.FromPolarCoordinates(impedanceMagnitude, impedanceAngle / 180 * Math.PI);
        Complex result = CalculateComponentParallelComponentResSeries(type, componentValues.ToList(), resistance, frequency, impedance, componentType, isMissingComponentInSeries);
        var angleTolerance = Math.Abs(result.Phase * 10e-3); // tolerance of 1%
        var tolerance = type switch {
            Normal => result.Magnitude * 5e-3, // tolerance of 0.5%
            MissingOneResistor => result.Real * 5e-3, // tolerance of 0.5%
            MissingOneComponent => result.Imaginary * 5e-3, // tolerance of 0.5%
            _ => 0
        };
        var expectedResult = Complex.FromPolarCoordinates(expectedResultMagnitude, expectedResultAngle / 180 * Math.PI);
        switch (type) {
            case Normal:
                Assert.That(result.Magnitude, Is.EqualTo(expectedResult.Magnitude).Within(tolerance));
                Assert.That(result.Phase, Is.EqualTo(expectedResult.Phase).Within(angleTolerance));
                break;
            case MissingOneResistor:
                Assert.That(result.Real, Is.EqualTo(expectedResult.Magnitude).Within(tolerance));
                break;
            case MissingOneComponent:
                Assert.That(result.Imaginary, Is.EqualTo(expectedResult.Magnitude).Within(tolerance));
                break;
            default:
                Assert.Fail("Invalid type");
                break;
        }
    }
    
    [TestCase(Normal, new[] { 3.3e-6, 2.2e-6 }, 470, 7e6, 0, 0, Inductor, 215.1, 62.8)]
    [TestCase(Normal, new[] { 4.7e-3, 1.5e-3 }, 3.9e3, 700e3, 0, 0, Inductor, 3.861e3, 8.1)]
    [TestCase(Normal, new[] { 22.0, 33.0 }, 1.8e3, 60, 0, 0, Inductor, 1.793e3, 5)]
    [TestCase(Normal, new[] { 33.0, 68.0 }, 220e3, 1e3, 0, 0, Inductor, 207.9e3, 19.12)]
    [TestCase(MissingOneResistor, new[] { 220e-3, 680e-3 }, 0, 8e3, 16.72e3, 21.7, Inductor, 18e3, 0)]
    [TestCase(MissingOneComponent, new[] { 2.2 }, 560, 30, 531.8, 18.3, Inductor, 6.8, 0)]
    [TestCase(MissingOneComponent, new[] { 470e-3 }, 2.7e3, 1e3, 2.456e3, 24.6, Inductor, 470e-3, 0)]
    [TestCase(MissingOneComponent, new[] { 33.0 }, 2.2e6, 7e3, 1.434e6, 49.3, Inductor, 10, 0)]
    [TestCase(MissingOneComponent, new[] { 330e-3 }, 220e3, 200e3, 214.9e3, 12.3, Inductor, 0.4728, 0)]
    [TestCase(Normal, new[] { 2.2e-9, 4.7e-9 }, 27e3, 3e3, 0, 0, Capacitor, 21.47e3, -37.3)]
    [TestCase(Normal, new[] { 330e-12, 1e-9 }, 22e3, 9e3, 0, 0, Capacitor, 21.02e3, -17.2)]
    [TestCase(Normal, new[] { 220e-12, 68e-12 }, 270e3, 2e3, 0, 0, Capacitor, 265.9e3, -10)]
    [TestCase(Normal, new[] { 68e-9, 470e-9 }, 3.9e3, 200, 0, 0, Capacitor, 3.745e3, -16.2)]
    [TestCase(MissingOneResistor, new[] { 6.8e-6, 1e-6 }, 0, 50, 1.884e3, -31.1, Capacitor, 2.2e3, 0)]
    [TestCase(MissingOneResistor, new[] { 220e-12, 68e-12 }, 0, 2e3, 265.9e3, -10, Capacitor, 270e3, 0)] // this is repeated
    [TestCase(MissingOneComponent, new[] { 220e-12 }, 270e3, 2e3, 265.9e3, -10, Capacitor, 68e-12, 0)] // this is repeated
    public void TestResParallelComponentSeries(ConfigTypes type, double[] componentValues, double resistance, double frequency, double impedanceMagnitude, double impedanceAngle, ComponentType componentType, double expectedResultMagnitude, double expectedResultAngle) {
        var impedance = Complex.FromPolarCoordinates(impedanceMagnitude, impedanceAngle / 180 * Math.PI);
        Complex result = CalculateResParallelComponentSeries(type, componentValues.ToList(), resistance, frequency, impedance, componentType);
        var angleTolerance = Math.Abs(result.Phase * 10e-3); // tolerance of 1%
        var tolerance = type switch {
            Normal => result.Magnitude * 5e-3, // tolerance of 0.5%
            MissingOneResistor => result.Real * 5e-3, // tolerance of 0.5%
            MissingOneComponent => result.Imaginary * 5e-3, // tolerance of 0.5%
            _ => 0
        };
        var expectedResult = Complex.FromPolarCoordinates(expectedResultMagnitude, expectedResultAngle / 180 * Math.PI);
        switch (type) {
            case Normal:
                Assert.That(result.Magnitude, Is.EqualTo(expectedResult.Magnitude).Within(tolerance));
                Assert.That(result.Phase, Is.EqualTo(expectedResult.Phase).Within(angleTolerance));
                break;
            case MissingOneResistor:
                Assert.That(result.Real, Is.EqualTo(expectedResult.Magnitude).Within(tolerance));
                break;
            case MissingOneComponent:
                Assert.That(result.Imaginary, Is.EqualTo(expectedResult.Magnitude).Within(tolerance));
                break;
            default:
                Assert.Fail("Invalid type");
                break;
        }
    }
    
    [TestCase(Normal, new[] { 3.3, 6.8 }, 470, 60, 0, 0, Inductor, 960.4, 60.7)]
    [TestCase(MissingOneResistor, new[] { 4.7, 10 }, 0, 10e3, 844.2e3, 13.8, Inductor, 820e3, 0)]
    [TestCase(MissingOneComponent, new[] { 680e-3 }, 33e3, 6e3, 35.4e3, 21.2, Inductor, 680e-3, 0)]
    [TestCase(Normal, new[] { 100e-9, 22e-9 }, 3.9e3, 400, 0, 0, Capacitor, 5.084e3, -39.9)]
    [TestCase(Normal, new[] { 470e-9, 220e-9 }, 39, 3e3, 0, 0, Capacitor, 86.21, -63.1)]
    [TestCase(MissingOneResistor, new[] { 100e-9, 22e-9 }, 0, 400, 5.084e3, -39.9, Capacitor, 3.9e3, 0)] // this is repeated
    [TestCase(MissingOneComponent, new[] { 470e-9 }, 39, 3e3, 86.21, -63.1, Capacitor, 220e-9, 0)] // this is repeated
    [TestCase(MissingOneComponent, new[] { 220e-9 }, 39, 3e3, 86.21, -63.1, Capacitor, 470e-9, 0)] // this is repeated
    public void TestResSeriesComponentParallel(ConfigTypes type, double[] componentValues, double resistance, double frequency, double impedanceMagnitude, double impedanceAngle, ComponentType componentType, double expectedResultMagnitude, double expectedResultAngle) {
        var impedance = Complex.FromPolarCoordinates(impedanceMagnitude, impedanceAngle / 180 * Math.PI);
        Complex result = CalculateResSeriesComponentParallel(type, componentValues.ToList(), resistance, frequency, impedance, componentType);
        var angleTolerance = Math.Abs(result.Phase * 10e-3); // tolerance of 1%
        var tolerance = type switch {
            Normal => result.Magnitude * 5e-3, // tolerance of 0.5%
            MissingOneResistor => result.Real * 5e-3, // tolerance of 0.5%
            MissingOneComponent => result.Imaginary * 5e-3, // tolerance of 0.5%
            _ => 0
        };
        var expectedResult = Complex.FromPolarCoordinates(expectedResultMagnitude, expectedResultAngle / 180 * Math.PI);
        switch (type) {
            case Normal:
                Assert.That(result.Magnitude, Is.EqualTo(expectedResult.Magnitude).Within(tolerance));
                Assert.That(result.Phase, Is.EqualTo(expectedResult.Phase).Within(angleTolerance));
                break;
            case MissingOneResistor:
                Assert.That(result.Real, Is.EqualTo(expectedResult.Magnitude).Within(tolerance));
                break;
            case MissingOneComponent:
                Assert.That(result.Imaginary, Is.EqualTo(expectedResult.Magnitude).Within(tolerance));
                break;
            default:
                Assert.Fail("Invalid type");
                break;
        }
    }

}