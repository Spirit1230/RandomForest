using System;

namespace MachineLearning
{
    class Program
    {
        static void Main(string[] args)
        {
            DecisionTree dT = new DecisionTree("IsSunnyTest.csv");

            string[] testEntry1 = new string[] {"TRUE", "FALSE"}; //TRUE
            string[] testEntry2 = new string[] {"FALSE", "FALSE"}; //FALSE
            string[] testEntry3 = new string[] {"TRUE", "TRUE"}; //TRUE

            Console.WriteLine(dT.GetDecision(testEntry1));
            Console.WriteLine(dT.GetDecision(testEntry2));
            Console.WriteLine(dT.GetDecision(testEntry3));
            
            // DataSet dS = new DataSet("IsSunnyTest.csv");
            // dS.PrintDataSet();

            // Console.WriteLine();

            // int[] selectedEntries = dS.SelectEntries(0, "TRUE");
            // Console.WriteLine(selectedEntries.Length);

            // Console.WriteLine();

            // int[] colToRemove = new int[] {};
            // int[] rowToRemove = new int[] {};

            // DataSet dS2 = new DataSet(dS.CloneData(colToRemove, selectedEntries));
            // dS2.PrintDataSet();

            Console.ReadKey();
        }
    }
}
