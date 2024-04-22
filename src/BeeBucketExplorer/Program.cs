using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Etherna.BeeBucketExplorer;

sealed class Program
{
    static async Task Main(string[] args)
    {
        var beeUrl = args[0];

        using var httpClient = new HttpClient();
        var beeClient = new BeeNet.Clients.DebugApi.BeeDebugClient(new Uri(beeUrl, UriKind.Absolute), httpClient);
        
        // Get postage stamps.
        var postageBatches = await beeClient.GetOwnedPostageBatchesByNodeAsync();

        foreach (var postage in postageBatches)
        {
            var bucketBatch = await beeClient.GetStampsBucketsForBatchAsync(postage.Id);

            Console.WriteLine($"postage Id: {postage.Id}, depth: {postage.Depth}");
            
            var maxCollisionsBucket = bucketBatch.Buckets.MaxBy(b => b.Collisions)!;
            Console.WriteLine($"  max collisions bucket: {maxCollisionsBucket.Collisions}");
        }
    }
}