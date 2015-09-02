using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EmbeSimp.Core.Evaluation
{
    /// <summary>
    /// A class containing code relating to automated evaluations of different systems for lexical simplification
    /// </summary>
    public class ReplacementEvaluator
    {
        /// <summary>
        /// Method for evaluating the precision of the systems on the replacement task (see Horn et al., ACL 2014)
        /// </summary>
        /// <param name="dataset">the evaluation dataset by Horn et al., ACL 2014</param>
        /// <param name="systemReplacements">Replacements proposed by the system</param>
        /// <returns>precision score of the system</returns>
        public double EvaluatePrecision(List<Tuple<string, string, Dictionary<string, int>>> dataset, List<string> systemReplacements)
        {
            if (dataset.Count != systemReplacements.Count) throw new NotSupportedException();
            double numCorrect = 0;
            double numTotal = 0;

            for (int i = 0; i < dataset.Count; i++)
            {
                if (!string.IsNullOrEmpty(systemReplacements[i]))
                {
                    numTotal++;
                    if (dataset[i].Item3.ContainsKey(systemReplacements[i].ToLower())) numCorrect++; 
                }
            }

            return numCorrect / numTotal;
        }

        /// <summary>
        /// Evaluating candidate precision (i.e., precision but considering all the candidates produced by the system and not ony the first ranked)
        /// </summary>
        /// <param name="dataset">the evaluation dataset by Horn et al., ACL 2014</param>
        /// <param name="systemReplacements">Replacements proposed by the system</param>
        /// <returns>modified precision score</returns>
        public double EvaluateCandidatePrecision(List<Tuple<string, string, Dictionary<string, int>>> dataset, List<List<string>> systemReplacementCandidates)
        {
            if (dataset.Count != systemReplacementCandidates.Count) throw new NotSupportedException();
            double numCorrect = 0;
            double numTotal = 0;

            for (int i = 0; i < dataset.Count; i++)
            {
                if (systemReplacementCandidates[i].Count > 0)
                {
                    numTotal++;
                    if (dataset[i].Item3.Any(h => systemReplacementCandidates[i].Any(s => h.Key.ToLower().Trim() == s.ToLower().Trim()))) numCorrect++;
                }
            }

            return numCorrect / numTotal;
        }

        /// <summary>
        /// Method for evaluating the accuracy of the systems on the replacement task (see Horn et al., ACL 2014)
        /// </summary>
        /// <param name="dataset">the evaluation dataset by Horn et al., ACL 2014</param>
        /// <param name="systemReplacements">Replacements proposed by the system</param>
        /// <returns>accuracy score of the system</returns>
        public double EvaluateAccuracy(List<Tuple<string, string, Dictionary<string, int>>> dataset, List<string> systemReplacements)
        {
            if (dataset.Count != systemReplacements.Count) throw new NotSupportedException();
            double numCorrect = 0;
            double numTotal = 0;

            for (int i = 0; i < dataset.Count; i++)
            {
                numTotal++;
                if (!string.IsNullOrEmpty(systemReplacements[i]) && dataset[i].Item3.ContainsKey(systemReplacements[i].ToLower())) numCorrect++;
            }

            return numCorrect / numTotal;
        }

        /// <summary>
        /// Modified method for evaluating the accuracy of the systems on the replacement task when considering all the candidates
        /// </summary>
        /// <param name="dataset">the evaluation dataset by Horn et al., ACL 2014</param>
        /// <param name="systemReplacements">Replacements proposed by the system</param>
        /// <returns>accuracy score of the system</returns>
        public double EvaluateCandidateAccuracy(List<Tuple<string, string, Dictionary<string, int>>> dataset, List<List<string>> systemReplacementCandidates)
        {
            if (dataset.Count != systemReplacementCandidates.Count) throw new NotSupportedException();
            double numCorrect = 0;
            double numTotal = 0;

            for (int i = 0; i < dataset.Count; i++)
            {
                numTotal++;
                if (systemReplacementCandidates[i].Count > 0 && dataset[i].Item3.Any(h => systemReplacementCandidates[i].Any(s => h.Key.ToLower().Trim() == s.ToLower().Trim()))) numCorrect++;
            }

            return numCorrect / numTotal;
        }

        /// <summary>
        /// "Changed" evaluation metric counting the number of sentences in which the system proposed to change the target word. (see Horn et al., 2014)
        /// </summary>
        /// <param name="dataset">the evaluation dataset by Horn et al., ACL 2014</param>
        /// <param name="systemReplacements">Replacements proposed by the system</param>
        /// <returns>the "changed" score for the system</returns>
        public double EvaluateChanged(List<Tuple<string, string, Dictionary<string, int>>> dataset, List<string> systemReplacements)
        {
            if (dataset.Count != systemReplacements.Count) throw new NotSupportedException();
            return ((double)(systemReplacements.Where(x => !string.IsNullOrEmpty(x)).Count())) / dataset.Count;
        }
    }
}
