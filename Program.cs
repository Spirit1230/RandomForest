using System;
using System.Collections.Generic;

namespace MachineLearning
{
    class Program
    {
        static void Main(string[] args)
        {
            MultChoiceTest();            

            Console.ReadKey();
        }

        static void BinaryTest() 
        {
            DecisionTree dT = new DecisionTree("TestData\\IsSunnyTest.csv");

            List<string[]> testEntries = new List<string[]>();

            testEntries.Add(new string[] {"TRUE", "FALSE"}); //TRUE
            testEntries.Add(new string[] {"FALSE", "FALSE"}); //FALSE
            testEntries.Add(new string[] {"TRUE", "TRUE"}); //TRUE

            foreach (string[] entry in testEntries) 
            {
                Console.WriteLine(@"For entry {0} : decision is {1}", string.Join(", ", entry), dT.GetDecision(entry));
            }

            Console.WriteLine();
        }

        static void MultChoiceTest() 
        {
            DecisionTree dT = new DecisionTree("TestData\\DessertTest.csv");

            List<string[]> testEntries = new List<string[]>();

            testEntries.Add(new string[] {"APPLE", "YES"}); //YES
            testEntries.Add(new string[] {"PEAR", "YES"}); //YES
            testEntries.Add(new string[] {"PEAR", "NO"}); //NO
            testEntries.Add(new string[] {"PINAPPLE", "NO"}); //YES
            testEntries.Add(new string[] {"AUBERGINE", "YES"}); //NO
            testEntries.Add(new string[] {"BANNANA", "YES"}); //YES
            testEntries.Add(new string[] {"LEMON", "NO"}); //NO
            testEntries.Add(new string[] {"LEMON", "YES"}); //NO
            

            foreach (string[] entry in testEntries) 
            {
                Console.WriteLine(@"For entry {0} : decision is {1}", string.Join(", ", entry), dT.GetDecision(entry));
            }

            Console.WriteLine();
        }

        static void RankedTest() 
        {
            DecisionTree dT = new DecisionTree("TestData\\CorD.csv");

            List<string[]> testEntries = new List<string[]>();

            testEntries.Add(new string[] {"2", "4"}); //CURRY
            testEntries.Add(new string[] {"1", "5"}); //CURRY
            testEntries.Add(new string[] {"3", "3"}); //DESSERT
            testEntries.Add(new string[] {"3", "4"}); //CURRY
            testEntries.Add(new string[] {"5", "1"}); //DESSERT
            testEntries.Add(new string[] {"4", "2"}); //DESSERT
            testEntries.Add(new string[] {"4", "3"}); //DESSERT
            testEntries.Add(new string[] {"3", "5"}); //CURRY
            

            foreach (string[] entry in testEntries) 
            {
                Console.WriteLine(@"For entry {0} : decision is {1}", string.Join(", ", entry), dT.GetDecision(entry));
            }

            Console.WriteLine();
        }

        static void NumTest() 
        {
            DecisionTree dT = new DecisionTree("TestData\\LikesFruit.csv");

            List<string[]> testEntries = new List<string[]>();

            testEntries.Add(new string[] {"YES", "100"}); //NO
            testEntries.Add(new string[] {"YES", "125"}); //YES
            testEntries.Add(new string[] {"NO", "600"}); //YES
            testEntries.Add(new string[] {"NO", "300"}); //YES
            testEntries.Add(new string[] {"YES", "40"}); //NO
            testEntries.Add(new string[] {"NO", "150"}); //NO
            testEntries.Add(new string[] {"NO", "325"}); //YES
            testEntries.Add(new string[] {"YES", "70"}); //NO
            

            foreach (string[] entry in testEntries) 
            {
                Console.WriteLine(@"For entry {0} : decision is {1}", string.Join(", ", entry), dT.GetDecision(entry));
            }

            Console.WriteLine();
        }

        static void DataSetTest() 
        {
            DataSet dS = new DataSet("TestData\\IsSunnyTest.csv");
            dS.PrintDataSet();

            Console.WriteLine();

            int[] selectedEntries = dS.SelectEntries(0, "TRUE");
            Console.WriteLine(selectedEntries.Length);

            Console.WriteLine();

            int[] colToRemove = new int[] {};
            int[] rowToRemove = new int[] {};

            DataSet dS2 = new DataSet(dS.CloneData(colToRemove, selectedEntries));
            dS2.PrintDataSet();
        }
    }
}
