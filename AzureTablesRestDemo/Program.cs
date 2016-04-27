using Newtonsoft.Json;

namespace AzureTablesRestDemo
{
    using System;

    public class TestModel
    {
        public string PartitionKey;

        public string RowKey;

        public string Price;
    }


    class Program
    {
        static void Main(string[] args)
        {
            int responseCode;

            TestModel data1 = new TestModel() { PartitionKey = "partition4", RowKey = "0000", Price = "1234" };

            for (int i = 0; i < 10; i++)
            {
                data1.RowKey = string.Format("{0:00000}", i);
                responseCode = AzureTableHelper.InsertEntity(
                    "atdemo",
                    "LPhE7jj52iGFwESTWHfupKJhp/36W2Fv9UNzsQbVWq/VveGGF39Sx930U/GjlZFliWgzgMQ1p68aW9lmb+H+Ag==",
                    "mytable",
                    JsonConvert.SerializeObject(data1));
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
