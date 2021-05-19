using System;
using System.Collections.Generic;

namespace MachineLearning
{
    class Program
    {
        static void Main(string[] args)
        {
            // BinaryTest();
            // MultChoiceTest();
            // RankedTest();
            // NumTest();

            // RandomTest();
            RandomForestTest();

            // DataSetTest();

            Console.ReadKey();
        }

        static void BinaryTest() 
        {
            Console.WriteLine("Binary Test\n");

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
            Console.WriteLine("Multiple Choice Test\n");

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
            Console.WriteLine("Numeric Ranked Data Test\n");

            DecisionTree dT = new DecisionTree("TestData\\CorDTest.csv");

            List<string[]> testEntries = new List<string[]>();

            testEntries.Add(new string[] {"2", "4"}); //CURRY
            testEntries.Add(new string[] {"1", "5"}); //CURRY
            testEntries.Add(new string[] {"3", "3"}); //DESSERT
            testEntries.Add(new string[] {"3", "4"}); //CURRY
            testEntries.Add(new string[] {"5", "1"}); //DESSERT
            testEntries.Add(new string[] {"4", "2"}); //DESSERT
            testEntries.Add(new string[] {"4", "3"}); //DESSERT
            testEntries.Add(new string[] {"3", "5"}); //CURRY
            testEntries.Add(new string[] {"1", "2"}); //DESSERT
            testEntries.Add(new string[] {"2", "4"}); //CURRY
            testEntries.Add(new string[] {"1", "3"}); //CURRY
            

            foreach (string[] entry in testEntries) 
            {
                Console.WriteLine(@"For entry {0} : decision is {1}", string.Join(", ", entry), dT.GetDecision(entry));
            }

            Console.WriteLine();
        }

        static void NumTest() 
        {
            Console.WriteLine("Numeric Data Test\n");

            DecisionTree dT = new DecisionTree("TestData\\LikesFruitTest.csv");

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

        static void RandomTest() 
        {
            Console.WriteLine("Random Test\n");

            DataSet dS = new DataSet("TestData\\LikesFruitTest.csv");

            List<string[]> testEntries = new List<string[]>();

            testEntries.Add(new string[] {"YES", "100"});
            testEntries.Add(new string[] {"YES", "125"});
            testEntries.Add(new string[] {"NO", "600"});
            testEntries.Add(new string[] {"NO", "300"});
            testEntries.Add(new string[] {"YES", "40"});
            testEntries.Add(new string[] {"NO", "150"});
            testEntries.Add(new string[] {"NO", "325"});
            testEntries.Add(new string[] {"YES", "70"});
            
            for (int i = 0; i < 10; i++) 
            {
                (DataSet bootStrappedDS, DataSet outOfBagDS) = dS.CreateBootstrapedDataSet();

                RandomTree rT = new RandomTree(bootStrappedDS, 0);

                foreach (string[] entry in testEntries) 
                {
                    Console.WriteLine(@"For entry {0} : decision is {1}", string.Join(", ", entry), rT.GetDecision(entry));
                }

                Console.WriteLine();
            }
            
        }

        static void RandomForestTest() 
        {
            Console.WriteLine("Random Forest Test\n");

            RandomForest rF = new RandomForest("TestData\\LikesFruitTest.csv");

            List<string[]> testEntries = new List<string[]>();

            testEntries.Add(new string[] {"YES", "100"});
            testEntries.Add(new string[] {"YES", "125"});
            testEntries.Add(new string[] {"NO", "600"});
            testEntries.Add(new string[] {"NO", "300"});
            testEntries.Add(new string[] {"YES", "40"});
            testEntries.Add(new string[] {"NO", "150"});
            testEntries.Add(new string[] {"NO", "325"});
            testEntries.Add(new string[] {"YES", "70"});

            foreach (string[] entry in testEntries) 
            {
                Console.WriteLine(@"For entry {0} : decision is {1}", string.Join(", ", entry), rF.GetDecision(entry));
            }

            Console.WriteLine();
            
        }

        static void LargeRandomTest() 
        {
            Console.WriteLine("Testing Random Forest With Large DataSet\n");

            RandomForest rF = new RandomForest("TestData\\LargeRandomTest.csv");

            Random rnd = new Random();

            List<string[]> testData = new List<string[]>();
            List<string> results = new List<string>();

            for (int i = 0; i < 100; i++) 
            {
                double[] testEntry = new double[] { rnd.Next(0, 100), rnd.Next(0, 10), rnd.Next(0, 10), rnd.Next(0, 1) };

                if (testEntry[2] > testEntry[1] && testEntry[2] * testEntry[1] / testEntry[0] < testEntry[3]) 
                {
                    results.Add("TRUE");
                }
                else 
                {
                    results.Add("FALSE");
                }

                string[] entrytoAdd = new string[testEntry.Length];

                for (int j = 0; j < testEntry.Length; j++) 
                {
                    entrytoAdd[j] = testEntry[j].ToString();
                }

                testData.Add(entrytoAdd);
            }

            for (int i = 0; i < testData.Count; i++) 
            {
                string[] test = testData[i];

                string decision = rF.GetDecision(test);

                string toWrite = string.Format("Test: {0}, result is {1}", string.Join(", ", test), decision);

                if (decision == results[i]) 
                {
                    Console.WriteLine(toWrite + ": PASSED");
                }
                else 
                {
                    Console.WriteLine(toWrite + ": FAILED");
                }
            }
        }

        static void DataSetTest() 
        {
            Console.WriteLine("Testing DataSet Functionality\n");

            DataSet dS = new DataSet("TestData\\LikesFruitTest.csv");
            dS.PrintDataSet();

            Console.WriteLine();

            for (int i = 0; i < dS.GetNumCol(); i++) 
            {
                Console.Write("\t\t" + dS.GetColType(i));
            }

            Console.WriteLine("\n");
            Console.WriteLine("Testing Selecting Entries\n");

            int[] selectedEntries = dS.SelectEntries(0, "TRUE");
            Console.WriteLine(selectedEntries.Length);

            Console.WriteLine();
            Console.WriteLine("Testing Cloning DataSet\n");

            int[] colToRemove = new int[] {};
            int[] rowToRemove = new int[] {};

            DataSet dS2 = new DataSet(dS.CloneData(colToRemove, selectedEntries));
            dS2.PrintDataSet();

            Console.WriteLine("\n");
            Console.WriteLine("Testing Boot Strapping data set\n");

            (DataSet bootStrappedDS, DataSet outOfBagDS) = dS.CreateBootstrapedDataSet();
            Console.WriteLine("Bootstrapped data :");
            bootStrappedDS.PrintDataSet();

            Console.WriteLine("\nOut of Bag Data :");
            outOfBagDS.PrintDataSet();

            Console.WriteLine("\n");

        }
    }
}
