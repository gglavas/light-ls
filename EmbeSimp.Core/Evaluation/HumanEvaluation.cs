using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace EmbeSimp.Core.Evaluation
{
    /// <summary>
    /// Class that encompasses code for human evaluation of lexical simplification systems
    /// </summary>
    public class HumanEvaluation
    {
        /// <summary>
        /// A method for loading humans annotations from textual annotation files
        /// </summary>
        /// <param name="annotationsPath">Path to the file containing annotations</param>
        /// <param name="codingsPath">Path to the file in which it is coded which annotation corresponds to a simplification of which system</param>
        /// <param name="meaningPreservation">Flag that indicates if the annotations are for simplicity/grammaticality or meaning preservation (different format)</param>
        /// <returns>A dictionary containin annotations for simplifications of all systems being evaluated. Key is the identifier of the system (Biran et al.'s 2011 system, our system, human simplifications)</returns>
        public static Dictionary<string, double> EvaluateHuman(string annotationsPath, string codingsPath, bool meaningPreservation = false)
        {
            AllAnnotatorScores = new List<double>();

            var codings = TakeLab.Utilities.IO.StringLoader.LoadDictionaryStrings(codingsPath);
            var annotationLines = (new StreamReader(annotationsPath)).ReadToEnd().Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).ToList();

            var index = 0;
            if (meaningPreservation)
            {
                annotationLines = annotationLines.Where(x => x.StartsWith("Simplified")).ToList();
                index++;
            }

            Dictionary<string, List<double>> systemScores = new Dictionary<string, List<double>>();
            annotationLines.ForEach(l => {
                var spl = l.Split();
                var system = codings[spl[index]];
                if (!systemScores.ContainsKey(system)) systemScores.Add(system, new List<double>());
                var score = Double.Parse(spl[index + 1]);
                systemScores[system].Add(score);
                AllAnnotatorScores.Add(score);
            });

            return systemScores.ToDictionary(x => x.Key, x => x.Value.Average());
        }

        public static Dictionary<string, double> EvaluateHumanDual(string annotationsPathFirst, string annotationsPathSecond, string codingsPath, bool meaningPreservation = false)
        {
            AllAnnotatorScores = new List<double>();

            var codings = TakeLab.Utilities.IO.StringLoader.LoadDictionaryStrings(codingsPath);
            var annotationLinesFirst = (new StreamReader(annotationsPathFirst)).ReadToEnd().Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            var annotationLinesSecond = (new StreamReader(annotationsPathSecond)).ReadToEnd().Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            
            //var originalLines = annotationLinesFirst.Where(x => x.StartsWith("Original")).ToList();

            var index = 0;
            if (meaningPreservation)
            {
                annotationLinesFirst = annotationLinesFirst.Where(x => x.StartsWith("Simplified")).ToList();
                annotationLinesSecond = annotationLinesSecond.Where(x => x.StartsWith("Simplified")).ToList();

                index++;
            }

            var biranSentence = string.Empty;
            var mySentence = string.Empty;
            var originalSentence = string.Empty;
            double biranScore = -1;
            double originalScore = -1;
            Dictionary<string, List<double>> systemScores = new Dictionary<string, List<double>>();
            for(int i = 0; i < annotationLinesFirst.Count; i++)
            {
                if ((!string.IsNullOrEmpty(biranSentence) && !string.IsNullOrEmpty(originalSentence)) || i % 4 == 1)
                {
                        biranSentence = string.Empty;
                        originalSentence = string.Empty;
                        biranScore = -1;
                        originalScore = -1;
                }

                var firstLineSplit = annotationLinesFirst[i].Split();
                var secondLineSplit = annotationLinesSecond[i].Split();
                
                //var originalLineSplit = originalLines[i].Split();

                var system = codings[firstLineSplit[index]];

                if (system == "biran" /*|| system == "embesimp"*/ || system == "original")
                {

                    if (system == "biran")
                    {
                        for (int j = 2; j < firstLineSplit.Length; j++) biranSentence += firstLineSplit[j];
                        biranScore = (Double.Parse(firstLineSplit[1]) + 4 * Double.Parse(secondLineSplit[1])) / 5;
                    }
                    //if (system == "embesimp") for (int j = 1; j < firstLineSplit.Length; j++) mySentence += firstLineSplit[j];
                    if (system == "original")
                    {
                        for (int j = 2; j < firstLineSplit.Length; j++) originalSentence += firstLineSplit[j];
                        originalScore = (Double.Parse(firstLineSplit[1]) + 4 * Double.Parse(secondLineSplit[1])) / 5;
                    }

                    if (!string.IsNullOrEmpty(originalSentence) && !string.IsNullOrEmpty(biranSentence) && ((biranSentence.Substring(0, 7) == originalSentence.Substring(0, 7)) || ((biranSentence.Substring(biranSentence.Length - 7, 6) == originalSentence.Substring(originalSentence.Length - 7, 6)))))
                    {
                        
                        //string origSent = string.Empty;
                        //string simpSentence = string.Empty;

                        //for (int j = 1; j < originalLineSplit.Length; j++) origSent += originalLineSplit[j];
                        //for (int j = 3; j < firstLineSplit.Length; j++) simpSentence += firstLineSplit[j];

                        if (biranSentence != originalSentence)
                        {
                            if (!systemScores.ContainsKey("biran")) systemScores.Add("biran", new List<double>());
                            if (!systemScores.ContainsKey("original")) systemScores.Add("original", new List<double>());
                            systemScores["biran"].Add(biranScore);
                            systemScores["original"].Add(originalScore);
                            //if (!systemScores.ContainsKey(system)) systemScores.Add(system, new List<double>());
                            //var score = (Double.Parse(firstLineSplit[index + 1]) + 4 * Double.Parse(secondLineSplit[index + 1])) / 5;
                            //systemScores[system].Add(score);
                            //AllAnnotatorScores.Add(score);
                        }

                    }

                    

                }
            }

            return systemScores.ToDictionary(x => x.Key, x => x.Value.Average());
        }

        public static List<double> AllAnnotatorScores { get; private set; } 

    }
}
