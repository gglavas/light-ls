using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using TakeLabCore.NLP.Annotators;
using System.Threading.Tasks;
using TakeLabCore.NLP.Annotators.English;
using TakeLabCore.NLP.Annotations;
using System.Diagnostics;

namespace EmbeSimp.Core.IO
{
    /// <summary>
    /// Helper class for preprocessing Wikipedia articles for lexical simplification purposes. 
    /// In this case, it is merely an (parallel) implementation of counting word frequencies on the entire English Wikipedia. 
    /// </summary>
    public class WikipediaPreprocessing
    {
        public void Preprocess(string outputDirPath)
        {
            ConcurrentDictionary<string, int> wordCount = new ConcurrentDictionary<string, int>();
            ConcurrentDictionary<string, int> wordPageCount = new ConcurrentDictionary<string, int>();

            WikiDataProvider wdp = new WikiDataProvider();
            EngTokenizer tokenizer = new EngTokenizer();
            EngMorphology morph = new EngMorphology();

            Stopwatch timer = Stopwatch.StartNew();

            Parallel.For(0, 13000, i => {
            //for(int i = 0; i < 5 /*1300*/; i++)
            //{
                Console.WriteLine("Thread taking iteration: " + i);
                Console.WriteLine("Loading 1K pages from DB");
                var pages = wdp.GetPageByIDInRange(i * 1000, (i+1)*1000);
                pages.ForEach(page => {
                    if (page.Language == "en")
                    {
                        if (page.PageID % 100 == 0) Console.WriteLine("Processing page: " + page.PageID);
                        var tokens = tokenizer.Annotate(page.Text).Select(x => ((TokenAnnotation)x)).ToList();

                        var distWords = tokens.Select(x => x.Text.Trim()).Distinct().ToList();
                        distWords.ForEach(dw => {
                            if (!wordPageCount.ContainsKey(dw)) wordPageCount.TryAdd(dw, 0);
                            wordPageCount[dw] += 1;
                        });
 
                        tokens.ForEach(t =>
                        {
                            if (!string.IsNullOrEmpty(t.Text))
                            {
                                if (!wordCount.ContainsKey(t.Text.ToLower())) wordCount.TryAdd(t.Text.ToLower(), 0);
                                wordCount[t.Text.ToLower()]++;
                            }
                        });
                    }
                });              
            });

            timer.Stop();
            Console.WriteLine("Processing finished in: " + timer.Elapsed.ToString());

            Console.WriteLine("Finished processing, writing freqs...");
            TakeLab.Utilities.IO.StringWriter<string>.WriteDictionary<int>(wordCount.OrderByDescending(x => x.Value).ToList(), outputDirPath + "\\wikipedia-english-word-freq.txt");

            Console.WriteLine("Finished processing, writing page freqs...");
            TakeLab.Utilities.IO.StringWriter<string>.WriteDictionary<int>(wordPageCount.OrderByDescending(x => x.Value).ToList(), outputDirPath + "\\wikipedia-english-word-page-freq.txt");
        }
    }
}
