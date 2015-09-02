using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EmbeSimp.Core.DataContainers
{
    /// <summary>
    /// An individual ranking task (consisting of 4 replacement candidate words) for the ranking evaluation (SemEval 2012 lexical simplification task)
    /// </summary>
    public class RankingTask
    {
        /// <summary>
        /// Individual task's ID
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Original sentence
        /// </summary>
        public string Sentence { get; set; }

        /// <summary>
        /// Target word to be simplified
        /// </summary>
        public string Target { get; set; }

        /// <summary>
        /// Simplification candidate words (to replace the target word in the original sentence)
        /// </summary>
        public List<string> Candidates { get; set; }
    }
}
