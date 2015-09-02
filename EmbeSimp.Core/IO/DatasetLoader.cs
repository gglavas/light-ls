using EmbeSimp.Core.DataContainers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace EmbeSimp.Core.IO
{
    /// <summary>
    /// The utility class for loading the datasets
    /// </summary>
    public class DatasetLoader
    {
        /// <summary>
        /// Method for loading the crowdsourced replacement dataset (see Horn et al., ACL 2014)
        /// </summary>
        /// <param name="path">Path to the file containing the dataset</param>
        /// <returns>the dataset consisting of triples <sentence, target word, human replacements (word and frequency)> ,</returns>
        public static List<Tuple<string, string, Dictionary<string, int>>> LoadMechanicalTurkTDataset(string path)
        {
            List<Tuple<string, string, Dictionary<string, int>>> dataset = new List<Tuple<string,string,Dictionary<string,int>>>();
            var lines = (new StreamReader(path)).ReadToEnd().Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            lines.ForEach(l => {
                var split = l.Split(new char[] { '\t' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                Dictionary<string, int> replacements = new Dictionary<string, int>();
                for (int i = 2; i < split.Count; i++)
                {
                    if (!replacements.ContainsKey(split[i].ToLower())) replacements.Add(split[i].ToLower(), 0);
                    replacements[split[i].ToLower()]++;
                }
                dataset.Add(new Tuple<string, string, Dictionary<string, int>>(split[0], split[1], replacements));
            });

            dataset.RemoveAt(0);
            return dataset;
        }

        /// <summary>
        /// Loading the SemEval 2012 task 1 (lexical simplification) dataset
        /// </summary>
        /// <param name="datasetPath">Path to the dataset file containing sentences</param>
        /// <param name="candidateRanksPath">Path to the file containing gold rankings for each of the sentences</param>
        /// <returns></returns>
        public static List<RankingTask> LoadRankingTasksDataset(string datasetPath, string candidateRanksPath)
        {            
            List<Tuple<int, List<string>>> instanceCandidates = new List<Tuple<int,List<string>>>();
            var candidateLines = (new StreamReader(candidateRanksPath)).ReadToEnd().Split(new char[]{'\r', '\n'}, StringSplitOptions.RemoveEmptyEntries).ToList();
            candidateLines.ForEach(line => {
                var coarseSplit = line.Split(new char[]{':', '{', '}', ','}, StringSplitOptions.RemoveEmptyEntries).Where(x => !string.IsNullOrEmpty(x.Trim())).Select(x => x.Trim()).ToList();
                var candidates = coarseSplit.GetRange(1, coarseSplit.Count - 1);
                var fineSplit = coarseSplit[0].Split();
                var id = Int32.Parse(fineSplit[1].Trim());
                instanceCandidates.Add(new Tuple<int, List<string>>(id, candidates));
            });

            var xmldoc = new XmlDocument();
            xmldoc.Load(datasetPath);

            List<RankingTask> rankingTasks = new List<RankingTask>();

            var instances = xmldoc.SelectNodes("//instance");
            foreach (XmlNode instance in instances)
            {
                RankingTask task = new RankingTask();
                task.ID = Int32.Parse(instance.Attributes["id"].Value);

                var contextNode = instance.ChildNodes[0];
                task.Sentence = contextNode.InnerText.Trim();

                foreach(XmlNode node in contextNode.ChildNodes)
                {
                    if (node.Name == "head") task.Target = node.InnerText;
                }
                
                task.Candidates = instanceCandidates.Where(x => x.Item1 == task.ID).Single().Item2;

                rankingTasks.Add(task);
            }

            return rankingTasks;
        }

    }
}
