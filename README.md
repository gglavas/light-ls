# Light-LS

1. DESCRIPTION

In this git repository you'll find the source code of the tool Light-LS. 
This is a Visual Studio solution and the source code is written in C#. 

The details of the model for lexical simplification implemented in this tool
can be found in the following publication: 

Glavaš, G. and Štajner S. Simplifying Lexical Simplification:  Do We Need Simplified Corpora?. 
Proceedings of the 53rd Annual Meeting of the Association for Computational Linguistics (ACL 2015), pages 63-68.
Beijing, 2015.   

or, in the bibtex format:

@inproceedings{glavavs2015simplifying,
  title={Simplifying Lexical Simplification: Do We Need Simplified Corpora?},
  author={Glava{\v{s}}, Goran and {\v{S}}tajner, Sanja},
  booktitle={Proceedings of the 53rd Annual Meeting of the Association for Computational Linguistics (ACL 2015)},
  pages={63--68},
  year={2015},
  organization={ACL}
}

Please cite the abovementioned paper if you use the tool or the source code in your research. 

2. SIMPLIFICATION

The simplification algorithm from the paper is implemented in the class "EmbeSimp.Core.Simp.LexicalSimplifier",
particularly in public method "Simplify" and, in turn, in the public method "GetSubstitutions". 

The simplification algorithm uses external resources and classes for:
1. Representing the textual document (class TakeLabCore.NLP.Annotations.Document)
2. Preprocessing the document (class TakeLabCore.NLP.Annotators.AnnotatorChain)
3. Loading the information contents of words (class TakeLabCore.NLP.Lexics.InformationContent)
4. Loading and fetching GloVe word vector representations (class TakeLabCore.NLP.Semantics.VectorSpace.WordVectorSpace)   

The resources required for these dependencies (necessary for running the tool) can be downloaded together with the
executable version of the tool from: 

takelab.fer.hr/light-ls

In case of any questions, please contact Goran Glavaš at
gogo.glavas@gmail.com or
goran.glavas@fer.hr

3. LICENSE

The Light-LS tool and the source code can be used under the 
GNU GENERAL PUBLIC LICENSE Version 2. For more information check out the LICENSE file. 