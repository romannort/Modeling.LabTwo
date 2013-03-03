using System;
using System.Collections.Generic;
using System.Linq;


namespace Modeling.LabTwo
{
    public class Generator
    {
        private IList<Double> realization;

        private const ulong aCoeff = 1111;

        private const ulong mCoeff = 1111111;

        private const ulong startingValue = 111;

        
        public IList<Double> Realization
        {
            get
            {
                return realization;
            }
        }


        private UInt64 NextNumber(UInt64 previousNumber)
        {
            UInt64 result = (aCoeff * previousNumber) % mCoeff;
            return result;
        }


        public void GenerateRealization()
        {
            ISet<ulong> lemerSequence = new HashSet<ulong>();
            ulong currentNumber = this.NextNumber(startingValue);

            while(true)
            {
                if (lemerSequence.Add(currentNumber) == false)
                {
                    break;
                }
                currentNumber = this.NextNumber(currentNumber);
            }
            this.realization = lemerSequence.ToList().ConvertAll(x => x / (Double)mCoeff );
        }        
    }
}
