using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EmbeSimp.Core.DataContainers
{
    /// <summary>
    /// Data structure for human annotation entry (for evaluation of different lexical simplification tools)
    /// </summary>
    public class AnnotationEntry
    {
        /// <summary>
        /// A simplified sentence
        /// </summary>
        public string Sentence { get; set; }

        /// <summary>
        /// An annotation assigned by the human annotator
        /// </summary>
        public int Grade { get; set; }

        /// <summary>
        /// Unique ID assigned to each individual annotation
        /// </summary>
        public Guid ID { get; set; }
    }
}
