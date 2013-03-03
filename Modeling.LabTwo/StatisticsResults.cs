using System;
using System.Collections.Generic;
using System.Linq;

namespace Modeling.LabTwo
{
    /// <summary>
    /// 
    /// </summary>
    public class StatisticsResults
    {

        /// <summary>
        /// 
        /// </summary>
        public Double ExpectedValue { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public Double Variance { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public Double Deviation { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public IList<Double> Cycle
        {
            get;
            private set;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="realization"></param>
        /// <returns></returns>
        public Boolean Calculate(IList<Double> realization)
        {
            try
            {
                this.Cycle = realization;
                ExpectedValue = ExpectedValueEstimation(Cycle);
                Variance = this.VarianceEstimation(Cycle);
                Deviation = this.DeviationEstimation(Cycle);                
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
        private Double VarianceEstimation(IEnumerable<Double> sequence)
        {
            double result = sequence.Average(x => Math.Pow(x - ExpectedValue, 2));
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sequence"></param>
        /// <returns></returns>
        private Double DeviationEstimation(ICollection<Double> sequence)
        {
            Double result;
            result = Math.Sqrt(sequence.Count / (Double)(sequence.Count - 1) * sequence.Average(x => Math.Pow(x - ExpectedValue, 2)));
            return result;
        }

    }
}
