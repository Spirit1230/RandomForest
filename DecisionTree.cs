using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;

namespace MachineLearning 
{
    class DecisionTree 
    {
        private Node RootNode;
        private DataSet dataSet;

        public DecisionTree(string dataSouce) 
        {
            dataSet = new DataSet(dataSouce);
            string[] outputDecision = dataSet.GetColValues(dataSet.GetNumCol() - 1);

            dataSet.PrintDataSet();
            Console.WriteLine();

            RootNode = new Node(dataSet);
        }

        public string GetDecision(string[] inputData) 
        {
            return RootNode.GetDecision(inputData);
        }

        private void CreateDecisionTree() 
        {

        }
    }

    abstract class Nodes 
    {
        abstract public string GetDecision(string[] input);
    }

    class Node : Nodes 
    {
        private Node leftNode;
        private Node rightNode;
        private int checkCol;
        private string condition;
        private float nodeImpurity;

        public Node(DataSet nodeDataSet) 
        {
            nodeImpurity = CalculateImpurity(nodeDataSet);

            Dictionary<int, float> colImpurities = new Dictionary<int, float>();

            for (int col = 0; col < nodeDataSet.GetNumCol() - 1; col++) 
            {
                Dictionary<string, DataSet> splitDataSets = SeperateData(col, nodeDataSet);
                float weightedImpurity = 0;

                foreach (KeyValuePair<string, DataSet> x in splitDataSets) 
                {
                    float splitDataImpurity = CalculateImpurity(x.Value);

                    Console.WriteLine(x.Key);
                    x.Value.PrintDataSet();
                    Console.WriteLine("Impurity = " + splitDataImpurity.ToString());
                    Console.WriteLine();

                    weightedImpurity += ((float)x.Value.GetNumEntries() / (float)nodeDataSet.GetNumEntries()) * splitDataImpurity;
                }

                Console.WriteLine(@"Impurity for {0} = {1}", nodeDataSet.GetHeaders()[col], weightedImpurity);
                Console.WriteLine();

                colImpurities.Add(col, weightedImpurity);                
            }

            checkCol = FindBestSplit(colImpurities);
            Console.WriteLine(@"Best split using {0}", nodeDataSet.GetHeaders()[checkCol]);

            Console.WriteLine();
            Test();
        }

        public void Test() 
        {
            string[] testVals = new string[] {"APPLES", "PEARS", "ORANGES", "BANNANAS", "GRAPES", "AUBERGINE"};

            foreach (string[] cond in FindAllCombinations(testVals)) 
            {
                Console.WriteLine(string.Join(" OR ", cond));
            }
        }

        override public string GetDecision(string[] input) 
        {
            try 
            {
                if (Convert.ToInt64(input[checkCol]) <= Convert.ToInt64(condition)) 
                {
                    return leftNode.GetDecision(new string[] {"1"});
                }
                else 
                {
                    return rightNode.GetDecision(new string[] {"1"});
                }
            }
            catch 
            {
                if (input[checkCol] == condition) 
                {
                    return leftNode.GetDecision(new string[] {"1"});
                }
                else 
                {
                    return rightNode.GetDecision(new string[] {"1"});
                }
            }
        }

        private List<string[]> FindAllCombinations(string[] allPosValues) 
        {
            List<string[]> posConds = new List<string[]>();

            int maxNumConds = allPosValues.Length - 1;
            int currentMaxConds = 1;
            
            int startPos = 0;
            int posIndex = startPos;

            int numFound = 0;
            int totalComb = CalculateTotalCombinations(currentMaxConds, allPosValues.Length);

            List<string> posCond = new List<string>();

            while (currentMaxConds <= maxNumConds) 
            {
                string nextValue = allPosValues[posIndex++];

                if (posCond.Count < currentMaxConds && !posCond.Contains(nextValue)) 
                {
                    posCond.Add(nextValue);
                }
                else 
                {
                    string valToRemove;
                    int posInAllValues;

                    do 
                    {
                        valToRemove = posCond[posCond.Count - 1];

                        posInAllValues = 0;

                        for (int i = 0; i < allPosValues.Length; i++) 
                        {
                            if (valToRemove == allPosValues[i]) 
                            {
                                posInAllValues = i;
                                break;
                            }
                        }

                        startPos = posInAllValues + 1;
                        posCond.Remove(valToRemove);

                    } while (allPosValues.Length - startPos < currentMaxConds - posCond.Count);

                    posIndex = startPos;
                }

                if (posCond.Count == currentMaxConds)
                {
                    posConds.Add(posCond.ToArray());
                    posCond.RemoveAt(posCond.Count - 1);
                    numFound++;
                }

                if (posIndex >= allPosValues.Length) 
                {
                    posIndex = startPos;
                }

                if (numFound == totalComb) 
                {
                    numFound = 0;
                    totalComb = CalculateTotalCombinations(++currentMaxConds, allPosValues.Length);
                    startPos = 0;
                    posIndex = startPos;
                    posCond.Clear();
                }
            }

            return posConds;
        }

        private int CalculateTotalCombinations(int numChosen, int totalOptions) 
        {
            int numComb = CalcFactorial(totalOptions) / (CalcFactorial(numChosen) * CalcFactorial(totalOptions - numChosen));

            return numComb;
        }

        private int CalcFactorial(int n) 
        {
            int result = 1;

            for (int i = 1; i <= n; i++) 
            {
                result *= i;
            }

            return result;
        }

        private Dictionary<string, DataSet> SeperateData(int col, DataSet dataSet) 
        {
            Console.WriteLine(dataSet.GetHeaders()[col]);

            //finds each entry corresponding to each possible column value
            //so if column can be TRUE or FALSE, finds all entries where column is TRUE and all entries where column is FALSE
            Dictionary<string, int[]> splitData = new Dictionary<string, int[]>();

            string[] values = dataSet.GetColValues(col);

            foreach (string value in values) 
            {
                splitData.Add(value, dataSet.SelectEntries(col, value));
            }

            //splits the data set into new data sets corresponding to each possible value
            //so if column can be TRUE or FALSE, creates new data sets for entries where column is TRUE and removes all FALSE entries and likewise for FALSE
            Dictionary<string, DataSet> splitDataSets = new Dictionary<string, DataSet>();

            foreach (KeyValuePair<string, int[]> data in splitData) 
            {
                List<int> toRemove = new List<int>();

                for (int toRemoveIndex = 0; toRemoveIndex < dataSet.GetNumEntries(); toRemoveIndex++) 
                {
                    if (!data.Value.Contains(toRemoveIndex)) 
                    {
                        toRemove.Add(toRemoveIndex);
                    }
                }

                splitDataSets.Add(data.Key, new DataSet(dataSet.CloneData(new int[] {col}, toRemove.ToArray())));
            }

            return splitDataSets;
        }

        private (DataSet, DataSet) SeperateData(int col, DataSet dataSet, string[] conditionToSeperate) 
        {
            //finds all entries that meet the specified conditions
            int[] selectedData = dataSet.SelectEntries(col, conditionToSeperate);

            //finds all entries that don't meet the specified conditions
            List<int> toRemove = new List<int>();

            for (int toRemoveIndex = 0; toRemoveIndex < dataSet.GetNumEntries(); toRemoveIndex++) 
            {
                if (!selectedData.Contains(toRemoveIndex)) 
                {
                    toRemove.Add(toRemoveIndex);
                }
            }

            //creates two data sets corresponding to meeting the specified conditions and not
            DataSet trueData = new DataSet(dataSet.CloneData(new int[] {col}, toRemove.ToArray()));
            DataSet falseData = new DataSet(dataSet.CloneData(new int[] {col}, selectedData));

            return (trueData, falseData);
        }

        private int FindBestSplit(Dictionary<int, float> splitData) 
        {
            //finds the best column to split the data

            float bestSplit = 1; //this is the highest possible impurity
            int toSplit = 0;

            foreach (KeyValuePair<int, float> toCheck in splitData) 
            {
                if (toCheck.Value <= bestSplit) 
                {
                    toSplit = toCheck.Key;
                    bestSplit = toCheck.Value;
                }
            }

            return toSplit;
        }

        private string FindBestSplit(Dictionary<string, float> splitData) 
        {
            //finds the best column to split the data

            float bestSplit = 1; //this is the highest possible impurity
            string toSplit = "";

            foreach (KeyValuePair<string, float> toCheck in splitData) 
            {
                if (toCheck.Value <= bestSplit) 
                {
                    toSplit = toCheck.Key;
                    bestSplit = toCheck.Value;
                }
            }

            return toSplit;
        }

        private float CalculateImpurity(DataSet toCalc) 
        {
            //calculates the spread of results to find the data sets impurity

            Dictionary<string, int> resultSpread = new Dictionary<string, int>();

            int resultsCol = toCalc.GetNumCol() - 1; //the final column in the data set

            string[] resultValues = toCalc.GetColValues(resultsCol);

            foreach (string result in resultValues) 
            {
                int numOccurences = toCalc.SelectEntries(resultsCol, result).Length;
                resultSpread.Add(result, numOccurences);
            }

            return CalculateGiniImpurity(resultSpread.Values.ToArray());
        }

        private float CalculateGiniImpurity(int[] values) 
        {
            //takes the spread of all possible results and calculates the Gini impurity

            float giniImpurity = 1;
            float totalValue = values.Sum();

            foreach (float value in values) 
            {
                giniImpurity -= (float)Math.Pow(value / totalValue, 2);
            }

            return giniImpurity;
        }

    }

    class Leaf : Nodes
    {
        private string decision;

        public Leaf(string _decision)
        {
            decision = _decision;
        }

        override public string GetDecision(string[] input) 
        {
            return decision;
        }
    }
}