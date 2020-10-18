using Microsoft.Azure.Documents;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace HGV.Warhorn.Api.Functions
{
    public class VotesFeedFunction
    {
        public VotesFeedFunction()
        {
        }

        [FunctionName("VotesFeed")]
        public void Run([CosmosDBTrigger(
            databaseName: "hgv-puzzles",
            collectionName: "votes",
            ConnectionStringSetting = "CosmosConnectionString",
            LeaseCollectionName = "leases", CreateLeaseCollectionIfNotExists = true)]IReadOnlyList<Document> input, ILogger log)
        {
            if (input != null && input.Count > 0)
            {
                log.LogInformation("Documents modified " + input.Count);
                log.LogInformation("First document Id " + input[0].Id);
            }
        }
    }
}
