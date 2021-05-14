using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;

namespace MachineLearning 
{
    class DecisionTree 
    {
        private Nodes RootNode;
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
        private Nodes leftNode;
        private Nodes rightNode;
        private int checkCol;
        private string[] condition;

        public Node(DataSet nodeDataSet) 
        {
            float nodeImpurity = CalculateImpurity(nodeDataSet);

            (int col, string[] split, float impurity) = FindBestSplit(nodeDataSet);

            checkCol = col;
            condition = split;

            if (nodeDataSet.GetNumCol() > 1) 
            {
                if (impurity < nodeImpurity) 
                {
                    (DataSet _leftNode, DataSet _rightNode) = SeperateData(checkCol, nodeDataSet, condition);

                    if (CalculateImpurity(_leftNode) != 0) 
                    {
                        leftNode = new Node(_leftNode);
                    }
                    else 
                    {
                        string decision = _leftNode.MostCommonColValue(_leftNode.GetNumCol() - 1);
                        leftNode = new Leaf(decision);
                    }

                    if (CalculateImpurity(_rightNode) != 0) 
                    {
                        rightNode = new Node(_rightNode);
                    }
                    else 
                    {
                        string decision = _rightNode.MostCommonColValue(_rightNode.GetNumCol() - 1);
                        rightNode = new Leaf(decision);
                    }
                }
                else 
                {
                    string decision = nodeDataSet.MostCommonColValue(nodeDataSet.GetNumCol() - 1);

                    leftNode = new Leaf(decision);
                    rightNode = new Leaf(decision);
                }  
            }
            else 
            {
                string decision = nodeDataSet.MostCommonColValue(nodeDataSet.GetNumCol() - 1);

                leftNode = new Leaf(decision);
                rightNode = new Leaf(decision);
            }

                      
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
                    return leftNode.GetDecision(input);
                }
                else 
                {
                    return rightNode.GetDecision(input);
                }
            }
            catch 
            {
                if (condition.Contains(input[checkCol])) 
                {
                    return leftNode.GetDecision(input);
                }
                else 
                {
                    return rightNode.GetDecision(input);
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

        private (int, string[], float) FindBestSplit(DataSet dataSet) 
        {
            float bestImpurity = 1;
            int bestCol = 0;
            string[] bestSplit = new string[0];

            for (int col = 0; col < dataSet.GetNumCol() - 1; col++) 
            {
                foreach (string[] comb in FindAllCombinations(dataSet.GetColValues(col))) 
                {
                    (DataSet leftNode, DataSet rightNode) = SeperateData(col, dataSet, comb);

                    float weightedImpurity = 
                        ((float)leftNode.GetNumEntries() / (float)dataSet.GetNumEntries()) * CalculateImpurity(leftNode)
                        + ((float)rightNode.GetNumEntries() / (float)dataSet.GetNumEntries()) * CalculateImpurity(rightNode)
                    ;

                    Console.WriteLine(@"Splitting using {0} == {1}", dataSet.GetHeaders()[col], string.Join(" OR ", comb));
                    Console.WriteLine(@"Impurity is : {0}", weightedImpurity);
                    Console.WriteLine();

                    if (weightedImpurity < bestImpurity) 
                    {
                        bestImpurity = weightedImpurity;
                        bestCol = col;
                        bestSplit = comb;
                    }
                }                
            }

            Console.WriteLine(@"Best split using {0} == {1}", dataSet.GetHeaders()[bestCol], string.Join(" OR ", bestSplit));
            Console.WriteLine(@"Impurity is : {0}", bestImpurity);
            Console.WriteLine();

            return (bestCol, bestSplit, bestImpurity);
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