using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;

namespace MachineLearning 
{
    class DataSet 
    {
        string[] headers = null;
        bool[] isColNumeric = null;
        List<string[]> dataSet = new List<string[]>();

        public DataSet(string dataSouce) 
        {
            //takes data from a .csv file and stores it as a list of string arrays
            //takes the topline as the files headers

            if (File.Exists(dataSouce)) 
            {
                foreach (string line in File.ReadAllLines(dataSouce)) 
                {
                    string[] dataInput = line.Split(',');
                    string result = dataInput[dataInput.Length - 1];

                    if (headers == null) 
                    {
                        headers = dataInput;
                        isColNumeric = new bool[dataInput.Length];
                    }
                    else 
                    {
                        dataSet.Add(dataInput);
                    }                    
                }

                for (int col = 0; col < this.headers.Length; col++) 
                {
                    isColNumeric[col] = IsColNumeric(col);
                }
            }
            else 
            {
                Console.WriteLine("File does not exist");
            }
        }

        public DataSet((string[] _headers, bool[] _isColNumeric, List<string[]> data) dataSource) 
        {
            //takes a tuple of data and headers and appropriatly assigns them
            //this is the output from the CloneData() function
            headers = dataSource._headers;
            isColNumeric = dataSource._isColNumeric;
            dataSet = dataSource.data;
        }

        public int GetNumCol() 
        {
            return this.headers.Length;
        }

        public int GetNumEntries() 
        {
            return this.dataSet.Count;
        }

        public string[] GetHeaders() 
        {
            return this.headers;
        }

        public string[] GetColValues(int col) 
        {
            //returns all unique values in a column            
            List<string> values = new List<string>();

            foreach (string[] entry in this.dataSet) 
            {
                string value = entry[col];

                if (!values.Contains(value)) 
                {
                    values.Add(value);
                }
            }

            return values.ToArray();
        }

        public string GetColType(int col) 
        {
            //returns whether the column has numeric/double data or strings
            //ranked data treated as numeric data for simplicity
            string colType;

            if (isColNumeric[col]) 
            {
                colType = "double";
            }
            else 
            {
                colType = "string";
            }

            return colType;
        }

        public (string[], bool[], List<string[]>) CloneData(int[] colToRemove = null, int[] rowToRemove = null) 
        {
            //clones the data whilst removing any specifed entries and/or columns

            if (colToRemove == null) 
            {
                colToRemove = new int[0];
            }

            if (rowToRemove == null) 
            {
                rowToRemove = new int[0];
            }

            //clones the data sets headers removing any specified headers
            List<string> clonedHeaders = new List<string>();
            List<bool> clonedColType = new List<bool>();

            for (int col = 0; col < this.headers.Length; col++) 
            {
                if (!colToRemove.Contains(col)) 
                {
                    clonedHeaders.Add(this.headers[col]);
                    clonedColType.Add(this.isColNumeric[col]);
                }
            }

            //clones the data removing any specified entries and/or columns
            List<string[]> clonedData = new List<string[]>();

            for (int row = 0; row < this.dataSet.Count; row++) 
            {
                if (!rowToRemove.Contains(row)) 
                {
                    string[] entry = this.dataSet[row];
                    string[] clonedEntry = new string[entry.Length - colToRemove.Length];

                    int posToAdd = 0;

                    for (int col = 0; col < entry.Length; col++) 
                    {
                        if (!colToRemove.Contains(col)) 
                        {
                            clonedEntry[posToAdd++] = entry[col];
                        }
                    }

                    clonedData.Add(clonedEntry);
                }                
            }

            return (clonedHeaders.ToArray(), clonedColType.ToArray(), clonedData);
        }

        public int[] SelectEntries(int checkCol, string entryCheck) 
        {
            //selects any entries who's column is a specified value

            List<int> selectedEntries = new List<int>();

            for (int row = 0; row < this.dataSet.Count; row++) 
            {
                string[] entry = this.dataSet[row];

                if (entry[checkCol] == entryCheck) 
                {
                    selectedEntries.Add(row);
                }
            }

            return selectedEntries.ToArray();
        }

        public int[] SelectEntries(int checkCol, string[] entryCheck) 
        {
            //selects any entries who's column is a specified value

            List<int> selectedEntries = new List<int>();

            for (int row = 0; row < this.dataSet.Count; row++) 
            {
                string[] entry = this.dataSet[row];

                if (entryCheck.Contains(entry[checkCol])) 
                {
                    selectedEntries.Add(row);
                }
            }

            return selectedEntries.ToArray();
        }

        public int[] SelectEntries(int checkCol, double entryCheck) 
        {
            //selects any entries who's column is less than or equal to a specified value

            List<int> selectedEntries = new List<int>();

            for (int row = 0; row < this.dataSet.Count; row++) 
            {
                string[] entry = this.dataSet[row];

                if (Convert.ToDouble(entry[checkCol]) <= entryCheck) 
                {
                    selectedEntries.Add(row);
                }
            }

            return selectedEntries.ToArray();
        }

        public void PrintDataSet() 
        {
            //prints the data and headers onto the console

            foreach (string header in this.headers) 
            {
                Console.Write("\t\t" + header);
            }

            Console.Write("\n");

            foreach (string[] entry in this.dataSet) 
            {
                string toOutput = "";

                foreach (string value in entry) 
                {
                    toOutput += "\t\t" + value;
                }

                Console.WriteLine(toOutput);
            }
        }

        public string MostCommonColValue(int col) 
        {
            //finds the most common value within a column
            string mostComVal = "";
            
            int highestValNum = 0;

            foreach (string val in GetColValues(col)) 
            {
                int valNum = SelectEntries(col, val).Length;

                if (valNum > highestValNum) 
                {
                    mostComVal = val;
                    highestValNum = valNum;
                }
            }

            return mostComVal;
        }

        private bool IsColNumeric(int col) 
        {
            //determines whether a column is numeric or not
            bool isNumeric = true;

            foreach (string value in GetColValues(col)) 
            {
                if (!Regex.IsMatch(value, @"\d+")) 
                {
                    isNumeric = false;
                    break;
                }
            }

            return isNumeric;
        }
    }
}