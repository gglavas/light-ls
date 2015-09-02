using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EmbeSimp.Core.Evaluation
{
    public class Pearson
    {
        public static double Correlation(List<double> goldStandard, List<double> results)
        {
            if (goldStandard.Count != results.Count) throw new NotSupportedException();

            double meanGoldStandard = 0;
            double meanResults = 0;

            foreach (double val in goldStandard) meanGoldStandard += val;
            foreach (double val in results) meanResults += val;

            meanGoldStandard = meanGoldStandard / goldStandard.Count;
            meanResults = meanResults / results.Count;

            double nomSum = 0;
            double squareSumGoldStandard = 0;
            double squareSumResults = 0;

            for (int i = 0; i < goldStandard.Count; i++)
            {
                nomSum += (goldStandard[i] - meanGoldStandard) * (results[i] - meanResults);
                squareSumGoldStandard += (goldStandard[i] - meanGoldStandard) * (goldStandard[i] - meanGoldStandard);
                squareSumResults += (results[i] - meanResults) * (results[i] - meanResults);
            }

            return nomSum / Math.Sqrt(squareSumGoldStandard * squareSumResults);
        }
    }
}
