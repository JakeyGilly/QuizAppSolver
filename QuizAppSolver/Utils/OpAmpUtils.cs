namespace QuizAppSolver.Utils;

public static class OpAmpUtils {
    public static double CalculateGain(double feedbackResistor, double inputResistor, bool isInverting) {
        return isInverting ? -feedbackResistor / inputResistor : 1 + feedbackResistor / inputResistor;
    }
}