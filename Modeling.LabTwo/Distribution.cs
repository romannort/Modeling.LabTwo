using System;
using System.Collections.Generic;
using System.Linq;

namespace Modeling.LabTwo
{
    public class Distribution
    {

        private static readonly ICollection<Double> SourceRealization;

        static Distribution()
        {
            Generator generator = new Generator();
            generator.GenerateRealization();
            SourceRealization = generator.Realization;
        }


        public ICollection<Double> Uniform(Double a, Double b)
        {
            ICollection<Double> result = SourceRealization.Select(x => x*(b - a) + a).ToList();
            return result;
        }

        public ICollection<Double> Gaussian(Double mean, Double sigma)
        {
            ICollection<Double> result = new List<double>();

            for (int i = 0; i < SourceRealization.Count; i += 6)
            {
                Double sum = TakeSum(i) - 3.0;
                Double number = mean + sigma * Math.Sqrt(2) * sum;
                result.Add( number );
            }
            return result;
        }


        public ICollection<Double> Exponential(Double lambda)
        {
            Double divLambda = -1 / lambda;
            ICollection<Double> result = SourceRealization.Select(x => Math.Log(x)*divLambda).ToList();
            return result;
        }


        private static Double TakeSum(Int32 i, Int32 bound = 6)
        {
            Double result = 0.0;
            for (int j = 0; j < bound; ++j)
            {
                result += SourceRealization.ElementAt(i + j);
            }
            return result;
        }

        private static Double TakeSum(Int32 i, Int32 bound, Func<Double,Double> modifier )
        {
            Double result = 0.0;
            for (int j = 0; j < bound; ++j)
            {
                result += modifier(SourceRealization.ElementAt(i + j));
            }
            return result;
        }

        public ICollection<Double> Gamma(Double lambda, Int32 eta)
        {
            ICollection<Double> result = new List<double>();
            Double divLambda = -1 / lambda;
            
            for (int i = 0; i < SourceRealization.Count; i += eta)
            {

                Double sum = TakeSum(i, eta, Math.Log);
                Double number = divLambda * sum;
                result.Add(number);
            }
            return result;
        }


        public ICollection<Double> Triangle(Double a, Double b)
        {
            ICollection<Double> result = new List<double>();

            for (int i = 0; i < SourceRealization.Count; i += 2)
            {
                Double max = Math.Max(SourceRealization.ElementAt(i),
                                         SourceRealization.ElementAt(i + 1));
                Double number = a + (b - a) * max;
                result.Add(number);
            }
            return result;
        }


        public ICollection<Double> Simpson(Double a, Double b)
        {
            ICollection<Double> uniform = this.Uniform(a/2, b/2);

            ICollection<Double> result = new List<double>();

            for (int i = 0; i < uniform.Count; i += 2)
            {
                result.Add( uniform.ElementAt(i) + uniform.ElementAt(i+1) );
            }

            return result;
        }

    }
}
