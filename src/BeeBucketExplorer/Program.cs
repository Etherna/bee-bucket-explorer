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
            var maxCollisions = (int)Math.Pow(2, postage.Depth - postage.BucketDepth);
            var bucketBatch = await beeClient.GetStampsBucketsForBatchAsync(postage.Id);

            Console.WriteLine($"postage Id: {postage.Id}, depth: {postage.Depth}, max collisions: {maxCollisions}");
            
            var maxCollisionsBucket = bucketBatch.Buckets.MaxBy(b => b.Collisions)!;
            Console.WriteLine($"  max collisions bucket: {maxCollisionsBucket.BucketId} with {maxCollisionsBucket.Collisions}");

            if (maxCollisionsBucket.Collisions == maxCollisions)
                Console.WriteLine("  WARNING: batch fulfilled!");
            else if (maxCollisionsBucket.Collisions > maxCollisions)
                Console.WriteLine("  ERROR: bucket over collided!!");
            
            Console.WriteLine("-----");
        }
    }
}