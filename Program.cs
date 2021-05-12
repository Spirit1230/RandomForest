using System;

namespace MachineLearning
{
    class Program
    {
        static void Main(string[] args)
        {
            DecisionTree dT = new DecisionTree("IsSunnyTest.csv");
            
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
