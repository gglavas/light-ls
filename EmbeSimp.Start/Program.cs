using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using EmbeSimp.Core.IO;
using System.Threading;
using TakeLabCore.NLP.Lexics;
using TakeLabCore.NLP.Semantics.VectorSpace;
using EmbeSimp.Core.Simp;
using TakeLabCore.NLP.Annotations;
using TakeLabCore.NLP.Annotators;
using EmbeSimp.Core.Evaluation;
using TakeLabCore.NLP.Annotators.English;
using System.IO;
using EmbeSimp.Core.DataContainers;
using System.Configuration;

namespace EmbeSimp.Start
{
    public class Program
    {
        /// <summary>
        /// A place for experimenting/running code
        /// </summary>
        /// <param name="args">Command line arguments</param>
        public static void Main(string[] args)
        {
            Console.WriteLine("Loading resources...");

            var tmp = Console.Out;
            Console.SetOut(TextWriter.Null);

            // setting the culture to en-US (e.g., decimal point is used instead of decimal comma)
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");

            #region Usage example code

            // Loading the information contents based on word frequencies from a large corpus
            InformationContent ic = new InformationContent(ConfigurationManager.AppSettings["other-resources"] + "\\word-freqs.txt");

            // Loading the GloVe embeddings into an instance of WordVectorSpace class 
            WordVectorSpace wvs = new WordVectorSpace();
            wvs.Load(ConfigurationManager.AppSettings["other-resources"] + "\\glove-vectors-6b-200d.txt", null);

            // Preprocessing tools required for preprocessing the free-text document
            AnnotatorChain ac = new AnnotatorChain(TakeLabCore.NLP.Language.English, new List<AnnotatorType> { AnnotatorType.SentenceSplitter, AnnotatorType.POSTagger, AnnotatorType.Morphology, AnnotatorType.NamedEntities });

            // Instantiating the lexical simplifier
            LexicalSimplifier simplifier = new LexicalSimplifier();

            // setting the information content and word vector space properties
            simplifier.InformationContent = ic;
            simplifier.VectorSpace = wvs;
            simplifier.CandidateInPoSLookup = TakeLab.Utilities.IO.StringLoader.LoadDictionaryStrings(ConfigurationManager.AppSettings["other-resources"] + "\\candidate-in-pos-lookup.txt");

            // dummy simplification call to force loading of language model resources
            var dLoad = new Document { Text = "want wish" };
            dLoad.Annotate(ac);
            simplifier.Simplify(dLoad, 5, 0.6, 0.5);

            Console.SetOut(tmp);

            Console.WriteLine("Loading resources done! \n\n [Light-LS v.0.9 Copyright TakeLab]\n Enter the text to simplify...\n\n");

            while(true)
            {
                try
                {
                    var line = Console.ReadLine();
                    if (string.IsNullOrEmpty(line)) break;

                    var document = new Document { Text = line };
                    document.Annotate(ac);

                    var dSimple = simplifier.Simplify(document, 5, 0.6, 0.5);
                    var substitutions = simplifier.LastSubstitutions;
                    foreach (var v in substitutions)
                    {
                        Console.WriteLine(v.Item1.Text + " -> " + v.Item2);
                    }

                    Console.WriteLine("Simplified text: \n");
                    Console.WriteLine(dSimple.Text);

                    Console.WriteLine("\n\n [Light-LS v.0.9 Copyright TakeLab]\n Enter the text to simplify...\n\n");
                }
                catch {
                    Console.WriteLine("Something went wrong, check your text for oddities and try again...");
                }

            }
            
            
            #endregion


            #region Evaluations Code (whole code commented)


            //// Evaluating systems via human-assigned scores of grammaticality, simplicity, and meaning preservation
            //#region Human evaluation

            //Console.WriteLine("Grammaticality");
            ////var evalResSanja = HumanEvaluation.EvaluateHuman(@"C:\Goran\Repos\EmbeSimp\EmbeSimp\Data\human-evaluation\sanja\grammaticality.txt", @"C:\Goran\Repos\EmbeSimp\EmbeSimp\Data\codings.txt");
            ////var sanjaGrammaticality = HumanEvaluation.AllAnnotatorScores;
            ////var evalResGoran = HumanEvaluation.EvaluateHuman(@"C:\Goran\Repos\EmbeSimp\EmbeSimp\Data\human-evaluation\goran\grammaticality.txt", @"C:\Goran\Repos\EmbeSimp\EmbeSimp\Data\codings.txt");
            ////var goranGrammaticality = HumanEvaluation.AllAnnotatorScores;

            ////Console.WriteLine("\nSanja");
            ////evalResSanja.ToList().ForEach(er => Console.WriteLine(er.Key + " " + er.Value));
            ////Console.WriteLine("\nGoran");
            ////evalResGoran.ToList().ForEach(er => Console.WriteLine(er.Key + " " + er.Value));
            ////Console.WriteLine("\nAgreement: " + Pearson.Correlation(sanjaGrammaticality, goranGrammaticality));

            //var evalAvg = HumanEvaluation.EvaluateHumanDual(@"C:\Goran\Repos\EmbeSimp\EmbeSimp\Data\human-evaluation\sanja\grammaticality.txt", @"C:\Goran\Repos\EmbeSimp\EmbeSimp\Data\human-evaluation\goran\grammaticality.txt", @"C:\Goran\Repos\EmbeSimp\EmbeSimp\Data\codings.txt");
            //Console.WriteLine("\nAveraged");
            //evalAvg.ToList().ForEach(er => Console.WriteLine(er.Key + " " + er.Value));

            ////Console.WriteLine();

            //Console.WriteLine("\nSimplicity");
            ////evalResSanja = HumanEvaluation.EvaluateHuman(@"C:\Goran\Repos\EmbeSimp\EmbeSimp\Data\human-evaluation\sanja\simplicity.txt", @"C:\Goran\Repos\EmbeSimp\EmbeSimp\Data\codings.txt");
            ////var sanjaSimplicity = HumanEvaluation.AllAnnotatorScores;
            ////evalResGoran = HumanEvaluation.EvaluateHuman(@"C:\Goran\Repos\EmbeSimp\EmbeSimp\Data\human-evaluation\goran\simplicity.txt", @"C:\Goran\Repos\EmbeSimp\EmbeSimp\Data\codings.txt");
            ////var goranSimplicity = HumanEvaluation.AllAnnotatorScores;

            ////Console.WriteLine("\nSanja");
            ////evalResSanja.ToList().ForEach(er => Console.WriteLine(er.Key + " " + er.Value));
            ////Console.WriteLine("\nGoran");
            ////evalResGoran.ToList().ForEach(er => Console.WriteLine(er.Key + " " + er.Value));
            ////Console.WriteLine("\nAgreement: " + Pearson.Correlation(sanjaSimplicity, goranSimplicity));

            //evalAvg = HumanEvaluation.EvaluateHumanDual(@"C:\Goran\Repos\EmbeSimp\EmbeSimp\Data\human-evaluation\sanja\simplicity.txt", @"C:\Goran\Repos\EmbeSimp\EmbeSimp\Data\human-evaluation\goran\simplicity.txt", @"C:\Goran\Repos\EmbeSimp\EmbeSimp\Data\codings.txt");
            //Console.WriteLine("\nAveraged");
            //evalAvg.ToList().ForEach(er => Console.WriteLine(er.Key + " " + er.Value));

            ////Console.WriteLine();

            ////Console.WriteLine("Meaning preservation");
            ////evalResSanja = HumanEvaluation.EvaluateHuman(@"C:\Goran\Repos\EmbeSimp\EmbeSimp\Data\human-evaluation\sanja\meaning-preservation.txt", @"C:\Goran\Repos\EmbeSimp\EmbeSimp\Data\codings.txt", true);
            ////var sanjaMeaningPreservation = HumanEvaluation.AllAnnotatorScores;
            ////evalResGoran = HumanEvaluation.EvaluateHuman(@"C:\Goran\Repos\EmbeSimp\EmbeSimp\Data\human-evaluation\goran\meaning-preservation.txt", @"C:\Goran\Repos\EmbeSimp\EmbeSimp\Data\codings.txt", true);
            ////var goranMeaningPreservation = HumanEvaluation.AllAnnotatorScores;

            ////Console.WriteLine("\nSanja");
            ////evalResSanja.ToList().ForEach(er => Console.WriteLine(er.Key + " " + er.Value));
            ////Console.WriteLine("\nGoran");
            ////evalResGoran.ToList().ForEach(er => Console.WriteLine(er.Key + " " + er.Value));
            ////Console.WriteLine("\nAgreement: " + Pearson.Correlation(sanjaMeaningPreservation, goranMeaningPreservation));

            //evalAvg = HumanEvaluation.EvaluateHumanDual(@"C:\Goran\Repos\EmbeSimp\EmbeSimp\Data\human-evaluation\sanja\meaning-preservation.txt", @"C:\Goran\Repos\EmbeSimp\EmbeSimp\Data\human-evaluation\goran\meaning-preservation.txt", @"C:\Goran\Repos\EmbeSimp\EmbeSimp\Data\codings.txt", true);
            //Console.WriteLine("\nAveraged");
            //evalAvg.ToList().ForEach(er => Console.WriteLine(er.Key + " " + er.Value));

            //Console.WriteLine();

            ////Console.WriteLine("Changed");
            ////var evalResCh = HumanEvaluation.EvaluateHuman(@"C:\Goran\Repos\EmbeSimp\EmbeSimp\Data\human-evaluation\goran\changed.txt", @"C:\Goran\Repos\EmbeSimp\EmbeSimp\Data\codings.txt", true);
            ////evalResCh.ToList().ForEach(er => Console.WriteLine(er.Key + " " + er.Value));

            ////Console.WriteLine("Done!");
            ////Console.ReadLine();

            //#endregion

            //// Loading the information contents based on word frequencies from a large corpus
            //InformationContent ic = new InformationContent(@"C:\Goran\Repos\EmbeSimp\EmbeSimp\Corpora\word-freqs.txt");

            //// Loading the GloVe embeddings into an instance of WordVectorSpace class 
            //WordVectorSpace wvs = new WordVectorSpace();
            //wvs.Load(@"C:\Goran\Korpusi\GloVe-Vectors\glove-vectors-6b-200d\glove-vectors-6b-200d.txt", null);

            //// Instantiating the lexical simplifier
            //LexicalSimplifier simplifier = new LexicalSimplifier();

            //// setting the information content and word vector space properties
            //simplifier.InformationContent = ic;
            //simplifier.VectorSpace = wvs;
            //simplifier.CandidateInPoSLookup = TakeLab.Utilities.IO.StringLoader.LoadDictionaryStrings(@"C:\Goran\Repos\EmbeSimp\EmbeSimp\Data\candidate-in-pos-lookup.txt");

            //// Evaluating systems via human-assigned scores of grammaticality, simplicity, and meaning preservation
            //#region Human Evaluation Task

            //#region Preparing files for evaluation

            ////var dirPath = @"C:\Goran\Repos\EmbeSimp\EmbeSimp\Data\";
            ////var originalLines = (new StreamReader(dirPath + "HA-Wiki-original.txt")).ReadToEnd().Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            ////var humanLines = (new StreamReader(dirPath + "HA-Wiki-manualSimplification.txt")).ReadToEnd().Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            ////var embeSimpLines = (new StreamReader(dirPath + "HA-Wiki-EmbeSimp.txt")).ReadToEnd().Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            ////var biranLines = (new StreamReader(dirPath + "HA-Wiki-Biran-simplified.txt")).ReadToEnd().Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).ToList();

            ////var gramSimpWriter = new StreamWriter(dirPath + "simplification-grades.txt");
            ////var meanPresWriter = new StreamWriter(dirPath + "meaning-preservation-grades.txt");

            ////Dictionary<Guid, string> codings = new Dictionary<Guid, string>();
            ////for (int i = 0; i < originalLines.Count; i++)
            ////{
            ////    List<AnnotationEntry> entries = new List<AnnotationEntry>();
            ////    var entry = new AnnotationEntry { ID = Guid.NewGuid(), Sentence = originalLines[i], Grade = 0 };
            ////    entries.Add(entry);
            ////    codings.Add(entry.ID, "original");

            ////    entry = new AnnotationEntry { ID = Guid.NewGuid(), Sentence = humanLines[i], Grade = 0 };
            ////    entries.Add(entry);
            ////    codings.Add(entry.ID, "manual");

            ////    entry = new AnnotationEntry { ID = Guid.NewGuid(), Sentence = biranLines[i], Grade = 0 };
            ////    entries.Add(entry);
            ////    codings.Add(entry.ID, "biran");

            ////    entry = new AnnotationEntry { ID = Guid.NewGuid(), Sentence = embeSimpLines[i], Grade = 0 };
            ////    entries.Add(entry);
            ////    codings.Add(entry.ID, "embesimp");

            ////    entries = entries.OrderBy(x => x.ID).ToList();
            ////    entries.ForEach(x => gramSimpWriter.WriteLine(x.ID + " " + x.Grade + " \"" + x.Sentence + "\""));
            ////    gramSimpWriter.WriteLine();

            ////    var original = entries.Where(x => codings[x.ID] == "original").Single();
            ////    var other = entries.Where(x => codings[x.ID] != "original").ToList();
            ////    other.ForEach(x =>
            ////    {
            ////        meanPresWriter.WriteLine("Original: " + "\"" + original.Sentence + "\"");
            ////        meanPresWriter.WriteLine("Simplified: " + x.ID + " " + x.Grade + " \"" + x.Sentence + "\"");
            ////        meanPresWriter.WriteLine();
            ////    });
            ////    meanPresWriter.WriteLine();
            ////    meanPresWriter.WriteLine();

            ////}

            ////gramSimpWriter.Close();
            ////meanPresWriter.Close();

            ////StreamWriter codingsWriter = new StreamWriter(dirPath + "codings.txt");
            ////codings.ToList().ForEach(c =>
            ////{
            ////    codingsWriter.WriteLine(c.Key.ToString() + " " + c.Value);
            ////});
            ////codingsWriter.Close();

            ////Console.WriteLine("Done!");
            ////Console.ReadLine();

            //#endregion

            ////var wikiSentences = TakeLab.Utilities.IO.StringLoader.LoadList(@"C:\Goran\Repos\EmbeSimp\EmbeSimp\Data\HA-Wiki-original.txt");
            ////var wikiWriter = new StreamWriter(@"C:\Goran\Repos\EmbeSimp\EmbeSimp\Data\HA-Wiki-EmbeSimp.txt");

            ////AnnotatorChain annChain = new AnnotatorChain(TakeLabCore.NLP.Language.English, new List<AnnotatorType> { AnnotatorType.SentenceSplitter, AnnotatorType.POSTagger, AnnotatorType.Morphology, AnnotatorType.NamedEntities });

            //////wikiSentences = wikiSentences.OrderBy(x => Guid.NewGuid()).ToList();
            ////for (int i = 0; i < wikiSentences.Count; i++)
            ////{
            ////    var document = new Document { Text = wikiSentences[i] };
            ////    document.Annotate(annChain);

            ////    var dSimple = simplifier.Simplify(document, 5, 0.55, 0.55);
            ////    wikiWriter.WriteLine(dSimple.Text);

            ////    Console.WriteLine(document.Text);
            ////    Console.WriteLine(dSimple.Text);
            ////    Console.WriteLine();
            ////}

            ////wikiWriter.Close();

            ////Console.WriteLine("Done!");
            ////Console.ReadLine();

            //#endregion


            //// Automated evaluation on the ranking lexical simplification task from SemEval 2012
            //#region Ranking task

            ////StreamWriter rankingsWriter = new StreamWriter(@"C:\Goran\Repos\EmbeSimp\EmbeSimp\Data\ranking-task-semeval\test-dataset\gorankings.txt");

            ////var datasetRanking = DatasetLoader.LoadRankingTasksDataset(@"C:\Goran\Repos\EmbeSimp\EmbeSimp\Data\ranking-task-semeval\test-dataset\contexts.xml", @"C:\Goran\Repos\EmbeSimp\EmbeSimp\Data\ranking-task-semeval\test-dataset\substitutions.gold-rankings");
            ////datasetRanking = datasetRanking.OrderBy(x => x.ID).ToList();
            ////datasetRanking.ForEach(rt => {
            ////    Console.WriteLine("Processing example: " + datasetRanking.IndexOf(rt));
            ////    var candidateRanking = simplifier.RankSimplificationCandidates(rt, 5);
            ////    rankingsWriter.Write("Sentence " + rt.ID.ToString() + " rankings: ");
            ////    for (int i = 0; i < candidateRanking.Count; i++)
            ////    {
            ////        rankingsWriter.Write("{" + candidateRanking[i] + "} ");
            ////        //Console.WriteLine(candidateRanking[i]);
            ////    }
            ////    rankingsWriter.WriteLine();
            ////    //Console.WriteLine();
            ////    //Console.ReadLine();
            ////});

            ////rankingsWriter.Close();

            ////Console.WriteLine("Done!");
            ////Console.ReadLine();

            //#endregion


            //// Automated evaluation on the crowdsourced replacement dataset (Kavuhcuk et al., ACL 2014)
            //#region Replacement task
            
            //AnnotatorChain ac = new AnnotatorChain(TakeLabCore.NLP.Language.English, new List<AnnotatorType> { AnnotatorType.SentenceSplitter, AnnotatorType.POSTagger, AnnotatorType.Morphology, AnnotatorType.NamedEntities });
            //var dataset = DatasetLoader.LoadMechanicalTurkTDataset(@"C:\Goran\Repos\EmbeSimp\EmbeSimp\Data\lex.mturk.txt");//.OrderBy(x => Guid.NewGuid()).ToList();
            
            //List<string> systemReplacements = new List<string>();
            //List<List<string>> systemReplacementCandidates = new List<List<string>>();

            //StreamWriter replacementOutputWriter = new StreamWriter(@"C:\Goran\Repos\EmbeSimp\EmbeSimp\Output\replacements.txt");
            
            //for(int i = 0; i < dataset.Count; i++)
            //{
            //    Console.WriteLine("Simplifying sentence {0}: ", i);
            //    Document d = new Document { Text = dataset[i].Item1 };
            //    ac.Apply(d);

            //    var dSimple = simplifier.Simplify(d, 5, 0, 0, dataset[i].Item2);
            //    Console.WriteLine("Original: " + d.Text);
            //    Console.WriteLine("Simplified: " + dSimple.Text);
            //    Console.WriteLine();
                
            //    var targetSimplification = simplifier.LastSubstitutions.Where(s => s.Item1.Text.ToLower() == dataset[i].Item2.ToLower()).FirstOrDefault();
            //    if (targetSimplification == null)
            //    {
            //        systemReplacements.Add(string.Empty);
            //        systemReplacementCandidates.Add(new List<string>());
            //    }
            //    else
            //    {
            //        systemReplacements.Add(targetSimplification.Item2);
            //        systemReplacementCandidates.Add(simplifier.LastSubstitutionCandidates[0].Item2);
            //    }

            //    Console.WriteLine("Change: {0} -> {1}", dataset[i].Item2, targetSimplification != null ?  targetSimplification.Item2 : "<empty>");
            //    //Console.ReadLine();

            //    replacementOutputWriter.WriteLine(d.Text);
            //    replacementOutputWriter.WriteLine("Target: " + dataset[i].Item2);
            //    replacementOutputWriter.WriteLine("Simplification: " + ((targetSimplification != null) ? targetSimplification.Item2  : "<empty>"));
            //    replacementOutputWriter.WriteLine("All candidates: ");
            //    simplifier.LastSubstitutionCandidates[0].Item2.ForEach(x => replacementOutputWriter.WriteLine(x));
            //    replacementOutputWriter.WriteLine();
            //}

            //replacementOutputWriter.Close();

            //var evaluator = new ReplacementEvaluator();
            
            //var precision = evaluator.EvaluatePrecision(dataset, systemReplacements);
            //var accuracy = evaluator.EvaluateAccuracy(dataset, systemReplacements);
            //var changed = evaluator.EvaluateChanged(dataset, systemReplacements);

            //var precisionCandidate = evaluator.EvaluateCandidatePrecision(dataset, systemReplacementCandidates);
            //var accuracyCandidate = evaluator.EvaluateCandidateAccuracy(dataset, systemReplacementCandidates);


            //Console.WriteLine("Precision: {0}\nAccuracy: {1}\nChanged: {2}\n", precision, accuracy, changed);
            //Console.WriteLine("Soft precision: {0}\nSoft accuracy: {1}\n Changed: {2}\n ", precisionCandidate, accuracyCandidate, changed);

            //TakeLab.Utilities.IO.StringWriter<string>.WriteDictionary<string>(simplifier.CandidateInPoSLookup, @"C:\Goran\Repos\EmbeSimp\EmbeSimp\Data\candidate-in-pos-lookup.txt");

            //#endregion

            //#region Wiki preprocessing

            ////WikipediaPreprocessing wpp = new WikipediaPreprocessing();
            ////wpp.Preprocess(@"C:\Goran\Korpusi\Wikipedia\preprocessed");

            ////TakeLabCore.NLP.Annotations.Document doc = new TakeLabCore.NLP.Annotations.Document{ Text = "he doctor who pulled Harrison Ford from the wreck of a plane crash has described how he feared a fireball from the aircraft's leaking fuel. Spine surgeon Sanjay Khurana was playing golf in Los Angeles when the American actor's vintage plane \"belly-flopped\" down on to the eighth hole." };
            ////(new TakeLabCore.NLP.Annotators.English.EngNER()).Annotate(doc);
            ////doc.NamedEntities.ForEach(ne => Console.WriteLine(ne.Text + " " + ne.NEType + " " + ne.StartPosition));

            ////var simnet = new SimilarityNet();
            ////simnet.SimpleLoad(@"C:\Goran\Korpusi\GloVe-Vectors\glove-vectors-6b-200d\glove-vectors-6b-200d.txt");
            
            ////while (true)
            ////{
            ////    var line = Console.ReadLine();
            ////    if (string.IsNullOrEmpty(line)) break;
            ////    else
            ////    {
            ////        var mostSimilar = simnet.GetMostSimilar(line.Trim(), 20);
            ////        if (mostSimilar != null) mostSimilar.ForEach(ms => Console.WriteLine(ms.Item1 + " " + ms.Item2));
            ////        else Console.WriteLine("Word not found in vocabulary, try again!");
            ////        Console.WriteLine();
            ////    }
            ////}

            //#endregion

            #endregion
        }
    }
}
