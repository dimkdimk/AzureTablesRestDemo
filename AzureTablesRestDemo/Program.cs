using System.Collections.Generic;
using Newtonsoft.Json;

namespace AzureTablesRestDemo
{

    public class TestModel
    {
        public string PartitionKey;
        public string RowKey;

        public string X1;
        public string X2;
        public string X3;
        public string X4;

        public string Y;
    }


    class Program
    {
        static void Main(string[] args)
        {
            int responseCode;

            List<TestModel> data = new List<TestModel>()
                                        {
                                            new TestModel()
                                                {
                                                    PartitionKey = "sample1",
                                                    RowKey = "0001",
                                                    X1 = "12.34",
                                                    X2 = "15",
                                                    X3 = "13",
                                                    X4 = "true",
                                                    Y = "1"
                                                },
                                            new TestModel()
                                                {
                                                    PartitionKey = "sample1",
                                                    RowKey = "0002",
                                                    X1 = "10.34",
                                                    X2 = "14",
                                                    X3 = "16",
                                                    X4 = "false",
                                                    Y = "0"
                                                },
                                            new TestModel()
                                                {
                                                    PartitionKey = "sample1",
                                                    RowKey = "0003",
                                                    X1 = "12.5",
                                                    X2 = "11",
                                                    X3 = "17",
                                                    X4 = "true",
                                                    Y = "1"
                                                },
                                            new TestModel()
                                                {
                                                    PartitionKey = "sample1",
                                                    RowKey = "0004",
                                                    X1 = "11.55",
                                                    X2 = "12",
                                                    X3 = "10",
                                                    X4 = "false",
                                                    Y = "1"
                                                }
                                        };

            foreach (var datasample in data)
            {
                responseCode = AzureTableHelper.InsertEntity(
                    "atdemo",
                    "LPhE7jj52iGFwESTWHfupKJhp/36W2Fv9UNzsQbVWq/VveGGF39Sx930U/GjlZFliWgzgMQ1p68aW9lmb+H+Ag==",
                    "mytable",
                    JsonConvert.SerializeObject(datasample));
                System.Console.WriteLine("InsertEntity response = {0}", responseCode);
            }

            string jsonData = "";

            responseCode = AzureTableHelper.RequestResource(
                "atdemo",
                "LPhE7jj52iGFwESTWHfupKJhp/36W2Fv9UNzsQbVWq/VveGGF39Sx930U/GjlZFliWgzgMQ1p68aW9lmb+H+Ag==",
                "mytable()",
                out jsonData);

            System.Console.WriteLine("Request table response = {0}", responseCode);
            System.Console.WriteLine("JSON response = {0}", jsonData);
        }
    }
}
