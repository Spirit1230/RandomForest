using System;
using System.Collections.Generic;

namespace MachineLearning 
{
    class RandomForest 
    {
        List<RandomTree> forest = new List<RandomTree>();
        double forestError = 1.0;

        public RandomForest(string dataSource)  
        {
            DataSet dataSet = new DataSet(dataSource);

            const int numTrees = 10;

            List<RandomTree> bestForest = new List<RandomTree>();
            double bestForestError = 1.0;

            //iterates through all total number of random columns to use to find the one that produces the most accurate results
            for (int numCols = 1; numCols < dataSet.GetNumCol() - 1; numCols++) 
            {
                this.forest.Clear();

                DataSet outOfBagDataSet = new DataSet(dataSet.CloneData());

                for (int i = 0; i < numTrees; i++) 
                {
                    (DataSet bootStrappedDataSet, DataSet tempOutOfBagDataSet) = dataSet.CreateBootstrapedDataSet();
                    this.forest.Add(new RandomTree(bootStrappedDataSet, numCols));

                    outOfBagDataSet = new DataSet(outOfBagDataSet.CloneData(rowToRemove : outOfBagDataSet.CompareDataSets(tempOutOfBagDataSet)));
                }

                this.forestError = CalculateOutOfBagError(outOfBagDataSet);

                //keeps the forest with the lowest out of bag error
                if (this.forestError < bestForestError) 
                {
                    bestForest.Clear();

                    bestForestError = forestError;

                    foreach (RandomTree tree in this.forest) 
                    {
                        bestForest.Add(tree);
                    }
                }
            }

            if (bestForest.Count > 0) 
            {
                this.forest.Clear();

                //assigns the forest with the lowest error
                foreach (RandomTree tree in bestForest) 
                {
                    this.forest.Add(tree);
                }
            }
            

            Console.WriteLine(@"Out of bag error : {0}%", forestError * 100);
            Console.WriteLine();
        }

        public string GetDecision(string[] entryInput) 
        {
            Dictionary<string, int> results = new Dictionary<string, int>();

            foreach (RandomTree tree in this.forest) 
            {
                string treeDecision = tree.GetDecision(entryInput);

                if (results.ContainsKey(treeDecision)) 
                {
                    results[treeDecision]++;
                }
                else 
                {
                    results.Add(treeDecision, 1);
                }
            }

            string decision = "";
            int bestScore = 0;

            foreach (KeyValuePair<string, int> vote in results) 
            {
                if (vote.Value > bestScore) 
                {
                    bestScore = vote.Value;
                    decision = vote.Key;
                }
            }

            return decision;
        }

        private double CalculateOutOfBagError(DataSet outOfBagDataSet) 
        {
            //calculates the out of bag error for the random forest
            double outOfBagError = 0;

            if (outOfBagDataSet.GetNumEntries() > 0)
            {
                //splits out of bag data into results and data
                List<string[]> outOfBagData = new List<string[]>();
                List<string> results = new List<string>();

                for (int entryIndex = 0; entryIndex < outOfBagDataSet.GetNumEntries(); entryIndex++) 
                {
                    string[] entry = outOfBagDataSet.GetEntry(entryIndex);
                    string[] data = new string[entry.Length - 1];

                    for (int i = 0; i < entry.Length; i++) 
                    {
                        if (i < entry.Length - 1) 
                        {
                            data[i] = entry[i];
                        }
                        else 
                        {
                            //takes last column in entry as the result
                            results.Add(entry[i]);
                        }
                    }

                    outOfBagData.Add(data);
                }

                //gets the random forests decision and compares it the expected result for each set of data
                int numCorrect = 0;

                for (int outOfBagIndex = 0; outOfBagIndex < outOfBagData.Count; outOfBagIndex++) 
                {
                    if (this.GetDecision(outOfBagData[outOfBagIndex]) == results[outOfBagIndex]) 
                    {
                        numCorrect++;
                    }
                }

                //calculates error
                outOfBagError = 1.0 - (double)numCorrect / (double)outOfBagData.Count;
            }

            return outOfBagError;
        }
    }
}
