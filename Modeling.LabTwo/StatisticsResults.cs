using System;
using System.Collections.Generic;
using System.Linq;

namespace Modeling.LabTwo
{
    /// <summary>
    /// 
    /// </summary>
    public static class StatisticsResults
    {

        /// <summary>
        /// 
        /// </summary>
        public static Double ExpectedValue { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public static Double Variance { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public static Double Deviation { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public static ICollection<Double> Cycle
        {
            get;
            private set;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="realization"></param>
        /// <returns></returns>
        public static Boolean Calculate(ICollection<Double> realization)
        {
            try
            {
                Cycle = realization;
                ExpectedValue = ExpectedValueEstimation(Cycle);
                Variance = VarianceEstimation(Cycle);
                Deviation = DeviationEstimation(Cycle);                
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sequence"></param>
        /// <returns></returns>
        private static Double ExpectedValueEstimation(IEnumerable<Double> sequence)
        {
            return Math.Abs(sequence.Average());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sequence"></param>
        /// <returns></returns>
        private static Double VarianceEstimation(IEnumerable<Double> sequence)
        {
            double result = sequence.Average(x => Math.Pow(x - ExpectedValue, 2));
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sequence"></param>
        /// <returns></returns>
        private static Double DeviationEstimation(ICollection<Double> sequence)
        {
            Double result;
            result = Math.Sqrt(sequence.Count / (Double)(sequence.Count - 1) * sequence.Average(x => Math.Pow(x - ExpectedValue, 2)));
            return result;
        }

    }
}
