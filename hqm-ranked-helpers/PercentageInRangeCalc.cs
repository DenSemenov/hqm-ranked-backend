

namespace hqm_ranked_helpers
{
    public static class PercentageInRangeCalc
    {
        public static double CalculatePercentage(double value, double minValue, double maxValue)
        {
            value = double.IsNaN(value) ? 0 : value;
            minValue = double.IsNaN(minValue) ? 0 : minValue;
            maxValue = double.IsNaN(maxValue) ? 0 : maxValue;


            if (value < minValue )
            {
                value = minValue;
            }

            if (value > maxValue)
            {
                value = maxValue;
            }

            double range = maxValue - minValue;
            double percent = ((value - minValue) / range) * 100;

            return percent;
        }
    }
}
