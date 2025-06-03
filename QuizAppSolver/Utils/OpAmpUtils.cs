namespace QuizAppSolver.Utils;

public static class OpAmpUtils {
    public static double CalculateGain(double feedbackResistor, double inputResistor, bool isInverting) {
        return isInverting ? -feedbackResistor / inputResistor : 1 + feedbackResistor / inputResistor;
    }
    
    public static double CalculateThreeDbBandwidth(double feedbackResistor, double inputResistor, double gainBandwidthProduct) {
        double gain = CalculateGain(feedbackResistor, inputResistor, true);
        return gainBandwidthProduct / (1 + Math.Abs(gain));
    }
    
    public static double CalculateXdbBandwidth(double feedbackResistor, double inputResistor, double gainBandwidthProduct, double x) {
        double threeDbBandwidth = CalculateThreeDbBandwidth(feedbackResistor, inputResistor, gainBandwidthProduct);
        return threeDbBandwidth * Math.Sqrt(Math.Pow(10, x / 10) - 1);
    }
}