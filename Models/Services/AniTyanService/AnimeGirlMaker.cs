using Minio;
using Minio.DataModel.Args;
using System.IO;

namespace AniTyan.Models.Services.AniTyanService
{
    public class AnimeGirlMaker
    {
        public static async Task<byte[]> MakeAnimeGirl(IMinioClient minioClient, byte[] pngBytes)
        {            
            var bucketName = "anime-girls";
            var objectKey = $"anime-card-{Guid.NewGuid()}.png";

            var bucketExists = await minioClient.BucketExistsAsync(new BucketExistsArgs().WithBucket(bucketName));

            // Create bucket if it doesn't exist
            if (!bucketExists)
            {
                await minioClient.MakeBucketAsync(new MakeBucketArgs().WithBucket(bucketName));
            }

            // Load PNG-card into bucket
            using (var stream = new MemoryStream(pngBytes))
            {
                await minioClient.PutObjectAsync(new PutObjectArgs()
                    .WithBucket(bucketName)
                    .WithObject(objectKey)
                    .WithStreamData(stream)
                    .WithObjectSize(stream.Length)
                    .WithContentType("image/png"));
            }

            // Read PNG-card from bucket
            byte[] downloadedImageBytes;
            using (var memoryStream = new MemoryStream())
            {
                await minioClient.GetObjectAsync(new GetObjectArgs()
                    .WithBucket(bucketName)
                    .WithObject(objectKey)
                    .WithCallbackStream(stream => stream.CopyTo(memoryStream)));
                downloadedImageBytes = memoryStream.ToArray();
            }

            // Delete PNG-card from bucker
            await minioClient.RemoveObjectAsync(new RemoveObjectArgs()
                .WithBucket(bucketName)
                .WithObject(objectKey));            

            return downloadedImageBytes;
        }
    }
}
