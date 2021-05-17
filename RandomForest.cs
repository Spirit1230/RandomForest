using System;
using System.Collections.Generic;

namespace MachineLearning 
{
    class RandomForest 
    {
        List<RandomTree> forest = new List<RandomTree>();

        public RandomForest(string dataSource, int numColsToUse = 0)  
        {
            DataSet dataSet = new DataSet(dataSource);

            const int numTrees = 100;

            for (int i = 0; i < numTrees; i++) 
            {
                forest.Add(new RandomTree(dataSet.CreateBootstrapedDataSet(), 1));
            }
        }

        public string GetDecision(string[] entryInput) 
        {
            Dictionary<string, int> results = new Dictionary<string, int>();

            foreach (RandomTree tree in forest) 
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
    }
}
