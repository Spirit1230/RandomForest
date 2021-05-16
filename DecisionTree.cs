using System;
using System.Linq;
using System.Collections.Generic;

namespace MachineLearning 
{
    class DecisionTree 
    {
        private Nodes RootNode;
        private DataSet dataSet;

        public DecisionTree(string dataSouce) 
        {
            dataSet = new DataSet(dataSouce);
            RootNode = new Node(dataSet);
        }

        public string GetDecision(string[] inputData) 
        {
            return RootNode.GetDecision(inputData);
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
        private string type;
        private string[] condition;

        public Node(DataSet nodeDataSet) 
        {
            float nodeImpurity = CalculateImpurity(nodeDataSet);

            (int col, string[] split, float impurity) = FindBestSplit(nodeDataSet);

            checkCol = col;
            type = nodeDataSet.GetColType(col);
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
                        //data split perfectly so only one decision can be made
                        string decision = _leftNode.MostCommonColValue(_leftNode.GetNumCol() - 1);
                        leftNode = new Leaf(decision);
                    }

                    if (CalculateImpurity(_rightNode) != 0) 
                    {
                        rightNode = new Node(_rightNode);
                    }
                    else 
                    {
                        //data split perfectly so only one decision can be made
                        string decision = _rightNode.MostCommonColValue(_rightNode.GetNumCol() - 1);
                        rightNode = new Leaf(decision);
                    }
                }
                else 
                {
                    //splitting the data further doesn't result in more accurate decisions
                    //adds two of the same leaf nodes effectively making the current node a leaf node
                    string decision = nodeDataSet.MostCommonColValue(nodeDataSet.GetNumCol() - 1);

                    leftNode = new Leaf(decision);
                    rightNode = new Leaf(decision);
                }  
            }
            else 
            {
                //no more columns to make decisions from
                //adds two of the same leaf nodes effectively making the current node a leaf node
                string decision = nodeDataSet.MostCommonColValue(nodeDataSet.GetNumCol() - 1);

                leftNode = new Leaf(decision);
                rightNode = new Leaf(decision);
            }

                      
        }

        override public string GetDecision(string[] input) 
        {
            if (input.Length != 0) 
            {
                string toCheck = input[checkCol];

                //removes column used from input to reflect how the decision tree is formed
                string[] toPass = new string[input.Length - 1];

                int toPassIndex = 0;

                for (int col = 0; col < input.Length; col++) 
                {
                    if (col != checkCol) 
                    {
                        toPass[toPassIndex++] = input[col];
                    }
                }

                if (type == "double")
                {
                    //data being looked at is numeric
                    if (Convert.ToDouble(toCheck) <= Convert.ToDouble(condition[0])) 
                    {
                        return leftNode.GetDecision(toPass);
                    }
                    else 
                    {
                        return rightNode.GetDecision(toPass);
                    }
                }
                else
                {
                    if (condition.Contains(toCheck)) 
                    {
                        return leftNode.GetDecision(toPass);
                    }
                    else 
                    {
                        return rightNode.GetDecision(toPass);
                    }
                }
            } 
            else 
            {
                //no data to make a decision from, next nodes must be leaf nodes
                return leftNode.GetDecision(new string[0]);
            }
            
        }

        private double[] FindAllNumericConditions(double[] allValues) 
        {
            //sorts each unique value and finds the midpoints between them
            List<double> allCond = new List<double>();

            Array.Sort(allValues);

            for (int i = 0; i < allValues.Length - 1; i++) 
            {
                allCond.Add((allValues[i] + allValues[i + 1]) / 2);
            }

            return allCond.ToArray();
        }

        private string[][] FindAllCombinations(string[] allPosValues) 
        {
            //finds all combinations of unique values 
            List<string[]> posConds = new List<string[]>();

            //finds combinations using 1 value then 2, 3 and so on till it reaches maxNumConds
            int maxNumConds = allPosValues.Length - 1;
            int currentMaxConds = 1;
            
            //position to take values from allPosValues
            int startPos = 0;
            int posIndex = startPos;

            //records how many possible combinations of a certain length can be found
            int numFound = 0;
            int totalComb = CalculateTotalCombinations(currentMaxConds, allPosValues.Length);

            List<string> posCond = new List<string>();

            while (currentMaxConds <= maxNumConds) 
            {
                //iteratest through all values to find different combinations
                string nextValue = allPosValues[posIndex++];

                if (posCond.Count < currentMaxConds && !posCond.Contains(nextValue)) 
                {
                    posCond.Add(nextValue);
                }
                else 
                {
                    //removes values till a start position is found in allPosValues that can fill posCond
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
                    //found a combination of conditions with the required length
                    posConds.Add(posCond.ToArray());
                    posCond.RemoveAt(posCond.Count - 1);    //removes final condition to prepare for next combination
                    numFound++;
                }

                if (posIndex >= allPosValues.Length) 
                {
                    //reached end of allPosValues array so loops back to specified start position
                    posIndex = startPos;
                }

                if (numFound == totalComb) 
                {
                    //all possible conditions of the required lenght have been found so resets to find the next set
                    numFound = 0;
                    totalComb = CalculateTotalCombinations(++currentMaxConds, allPosValues.Length);
                    startPos = 0;
                    posIndex = startPos;
                    posCond.Clear();
                }
            }

            return posConds.ToArray();
        }

        private int CalculateTotalCombinations(int numChosen, int totalOptions) 
        {
            //total combinations without repition = n!/(r!(n-r)!)
            int numComb = CalcFactorial(totalOptions) / (CalcFactorial(numChosen) * CalcFactorial(totalOptions - numChosen));

            return numComb;
        }

        private int CalcFactorial(int n) 
        {
            //calculates the factorial eg: 5! = 5 * 4 * 3 * 2 * 1
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
            int[] selectedData;

            if (dataSet.GetColType(col) == "double") 
            {
                //converts to double if dealing with numeric data
                selectedData = dataSet.SelectEntries(col, Convert.ToDouble(conditionToSeperate[0]));
            }
            else 
            {
                selectedData = dataSet.SelectEntries(col, conditionToSeperate);
            }

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
            //finds best way to split a data set to find lowest impurity

            float bestImpurity = 1; //highest possible impurity
            int bestCol = 0;
            string[] bestSplit = new string[0];

            for (int col = 0; col < dataSet.GetNumCol() - 1; col++) 
            {
                string[][] toCheck;
                string[] uniqueVals = dataSet.GetColValues(col);

                if (dataSet.GetColType(col) == "double") 
                {
                    //handles numeric data
                    double[] allNums = new double[uniqueVals.Length];

                    for (int i = 0; i < uniqueVals.Length; i++) 
                    {
                        allNums[i] = Convert.ToDouble(uniqueVals[i]);
                    }

                    double[] allCond = FindAllNumericConditions(allNums);

                    //all data handled as strings so numeric conditions converted back
                    toCheck = new string[allCond.Length][];

                    for (int i = 0; i < toCheck.Length; i++) 
                    {
                        toCheck[i] = new string[] { Convert.ToString(allCond[i]) };
                    }
                } 
                else 
                {
                    toCheck = FindAllCombinations(uniqueVals);
                }

                foreach (string[] comb in toCheck) 
                {
                    //calculates weighted impurity of split data and records conditions that improve impurity
                    (DataSet leftNode, DataSet rightNode) = SeperateData(col, dataSet, comb);

                    float weightedImpurity = 
                        ((float)leftNode.GetNumEntries() / (float)dataSet.GetNumEntries()) * CalculateImpurity(leftNode)
                        + ((float)rightNode.GetNumEntries() / (float)dataSet.GetNumEntries()) * CalculateImpurity(rightNode)
                    ;

                    if (weightedImpurity < bestImpurity) 
                    {
                        bestImpurity = weightedImpurity;
                        bestCol = col;
                        bestSplit = comb;
                    }
                }             
            }

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