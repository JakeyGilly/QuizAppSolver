using System.Numerics;
using static QuizAppSolver.AC.PassiveNetworks.MoreComponents.TwoResistorsOneComponent;
using static QuizAppSolver.Utils.PassiveNetworksUtils;
using static QuizAppSolver.Utils.PassiveNetworksUtils.ComponentType;
using static QuizAppSolver.Utils.PassiveNetworksUtils.ConfigTypes;

namespace QuizAppSolverTests.PassiveNetworksTests;

public class TwoResistorsOneComponentTests {
    [TestCase(Normal, new[] { 220e3, 390e3 }, 3.3, 4e3, 0, 0, Inductor, 615.6e3, 7.7)]
    
    [TestCase(MissingOneResistor, new[] { 39e3 }, 150e-12, 70e3, 67.72e3, -12.9, Capacitor, 27e3, 0)]
    [TestCase(MissingOneResistor, new[] { 56e3 }, 3.3e-9, 600, 115.5e3, -44.1, Capacitor, 27e3, 0)]
    [TestCase(MissingOneResistor, new[] { 1e3 }, 220e-12, 1e6, 1.418e3, -30.7, Capacitor, 220, 0)]
    public void TestSeries(ConfigTypes type, double[] resistances, double componentValue, double frequency, double impedanceMagnitude, double impedanceAngle, ComponentType componentType, double expectedResultMagnitude, double expectedResultAngle) {
        var impedance = Complex.FromPolarCoordinates(impedanceMagnitude, impedanceAngle / 180 * Math.PI);
        Complex result = CalculateSeries(type, resistances.ToList(), componentValue, frequency, impedance, componentType);
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
    
    [TestCase(Normal, new[] { 330.0, 390.0 }, 1.5, 40, 0, 0, Inductor, 161.5, 25.4)]
    [TestCase(Normal, new[] { 1e6, 1.5e6 }, 15, 20e3, 0, 0, Inductor, 571.7e3, 17.7)]
    [TestCase(MissingOneComponent, new[] { 180e3, 560e3 }, 0, 5e3, 114.9e3, 32.5, Inductor, 6.8, 0)]
    
    [TestCase(Normal, new[] { 1.2e3, 2.7e3 }, 47e-9, 4e3, 0, 0, Capacitor, 592.9, -44.5)]
    [TestCase(MissingOneResistor, new[] { 270.0 }, 68e-9, 20e3, 49.27, -24.9, Capacitor, 68, 0)]
    public void TestParallel(ConfigTypes type, double[] resistances, double componentValue, double frequency, double impedanceMagnitude, double impedanceAngle, ComponentType componentType, double expectedResultMagnitude, double expectedResultAngle) {
        var impedance = Complex.FromPolarCoordinates(impedanceMagnitude, impedanceAngle / 180 * Math.PI);
        Complex result = CalculateParallel(type, resistances.ToList(), componentValue, frequency, impedance, componentType);
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
    
    [TestCase(Normal, new[] { 180.0, 150 }, 47e-6, 900e3, 0, 0, Inductor, 300.7, 12.3, false)]
    [TestCase(MissingOneComponent, new[] { 820.0, 560.0 }, 0, 100e3, 1.061e3, 14.7, Inductor, 680e-6, 0, false)]
    
    [TestCase(MissingOneResistor, new[] { 12e3 }, 10e-12, 4e6, 13.72e3, -3.9, Capacitor, 2.2e3, 0, false)]
    [TestCase(MissingOneResistor, new[] { 1.8e3 }, 33e-12, 600e3, 13.72e3, -1.6, Capacitor, 12e3, 0, true)]
    public void TestResSeriesComponentResParallel(ConfigTypes type, double[] resistances, double componentValue, double frequency, double impedanceMagnitude, double impedanceAngle, ComponentType componentType, double expectedResultMagnitude, double expectedResultAngle, bool isMissingResistorInSeries) {
        var impedance = Complex.FromPolarCoordinates(impedanceMagnitude, impedanceAngle / 180 * Math.PI);
        Complex result = CalculateResSeriesComponentResParallel(type, resistances.ToList(), componentValue, frequency, impedance, componentType, isMissingResistorInSeries);
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
    
    [TestCase(MissingOneResistor, new[] { 47e3 }, 3.3, 2e3, 40.16e3, 1.3, Inductor, 270e3, 0, true)]
    
    [TestCase(MissingOneResistor, new[] { 2.7e3 }, 47e-12, 600e3, 2.163e3, -18.2, Capacitor, 2.7e3, 0, true)]
    [TestCase(MissingOneResistor, new[] { 180.0 }, 68e-9, 4e3, 400.2, -38.7, Capacitor, 680, 0, false)]
    [TestCase(MissingOneResistor, new[] { 47e3 }, 100e-12, 30e3, 57.28e3, -37.2, Capacitor, 220e3, 0, false)]
    public void TestResParallelComponentResSeries(ConfigTypes type, double[] resistances, double componentValue, double frequency, double impedanceMagnitude, double impedanceAngle, ComponentType componentType, double expectedResultMagnitude, double expectedResultAngle, bool isMissingResistorInSeries) {
        var impedance = Complex.FromPolarCoordinates(impedanceMagnitude, impedanceAngle / 180 * Math.PI);
        Complex result = CalculateResParallelComponentResSeries(type, resistances.ToList(), componentValue, frequency, impedance, componentType, isMissingResistorInSeries);
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
    
    [TestCase(Normal, new[] { 56.0, 390.0 }, 68e-6, 800e3, 0, 0, Inductor, 271.3, 52.5)]
    public void TestComponentParallelResSeries(ConfigTypes type, double[] resistances, double componentValue, double frequency, double impedanceMagnitude, double impedanceAngle, ComponentType componentType, double expectedResultMagnitude, double expectedResultAngle) {
        var impedance = Complex.FromPolarCoordinates(impedanceMagnitude, impedanceAngle / 180 * Math.PI);
        Complex result = CalculateComponentParallelResSeries(type, resistances.ToList(), componentValue, frequency, impedance, componentType);
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
    
    [TestCase(Normal, new[] { 15e3, 56e3 }, 1, 10e3, 0, 0, Inductor, 63.94e3, 79.3)]
    public void TestComponentSeriesResParallel(ConfigTypes type, double[] resistances, double componentValue, double frequency, double impedanceMagnitude, double impedanceAngle, ComponentType componentType, double expectedResultMagnitude, double expectedResultAngle) {
        var impedance = Complex.FromPolarCoordinates(impedanceMagnitude, impedanceAngle / 180 * Math.PI);
        Complex result = CalculateComponentSeriesResParallel(type, resistances.ToList(), componentValue, frequency, impedance, componentType);
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