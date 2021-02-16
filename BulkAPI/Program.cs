
using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkAPI
{
    class Program
    {
        static async Task Main(string[] args)
        {
            String connectionString = "AccountEndpoint=https://<YourAccountName>.documents.azure.com:443/;AccountKey=<YourAccountKey>;";
            CosmosClientOptions options = new CosmosClientOptions() { AllowBulkExecution = true , ConnectionMode=ConnectionMode.Direct, ConsistencyLevel=ConsistencyLevel.Eventual};
            CosmosClient cosmosClient = new CosmosClient(connectionString, options);
            Container container = cosmosClient.GetContainer("RetailDemo", "WebsiteData1");

            // Assuming your have your data available to be inserted or read
            List<Task> concurrentTasks = null; DateTime start=DateTime.Now;
            for (int iteration = 0; iteration < 3; iteration++)
            {
                concurrentTasks= new List<Task>();
                 start = DateTime.Now;
                foreach (Item itemToInsert in ReadYourData())
                {
                    concurrentTasks.Add(container.CreateItemAsync(itemToInsert, new PartitionKey(itemToInsert.CartID),new ItemRequestOptions() { EnableContentResponseOnWrite=false }));
                }

                await Task.WhenAll(concurrentTasks);
                
            }
            System.Console.WriteLine("Record inserted=" + concurrentTasks.Count + " in total time= " + DateTime.Now.Subtract(start).TotalMilliseconds + " ms");
            System.Console.ReadLine();

        }

        private static IEnumerable<Item> ReadYourData()
        {
            List<Item> lstItems = new List<Item>();
            for(int j=0;j<10;j++)
            for (int i=0;i<1000;i++)
            {
                    lstItems.Add(new Item() { id = Guid.NewGuid().ToString(), name = "MNt" + i, CartID = "pk" + j });
            }
            return lstItems;
        }
    }
}
