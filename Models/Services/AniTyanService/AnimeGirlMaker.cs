using AniTyan.Models.Services.KoikatsuCardService;
using Microsoft.AspNetCore.Mvc;
using Minio;
using Minio.DataModel.Args;
using System.IO;
using static AniTyan.Models.Services.KoikatsuCardService.KoikatsuCardService;

namespace AniTyan.Models.Services.AniTyanService
{
    public class AnimeGirlMaker
    {
        private readonly IMinioClient _minioClient;
        private readonly IKoikatsuCardService _koikatsuCardService;

        public AnimeGirlMaker(IMinioClient minioClient, IKoikatsuCardService koikatsuCardService)
        {
            _minioClient = minioClient;
            _koikatsuCardService = koikatsuCardService;
        }


        public async Task MakeAnimeGirl(IFormFile cardFile)
        {
            /*
             * Извлечь поля latname, firstname, nickname из Koikatsu карточки
             */
            CharacterNameResponse character = await _koikatsuCardService.GetCharacterNameAsync(cardFile);

            /*
             * Записать карточку в MinIO, получить ссылку в MinIO
             */
            var bucketName = "koikatsu-cards";
            var objectKey = $"koikatsu-card-{Guid.NewGuid()}.png";

            var bucketExists = await _minioClient.BucketExistsAsync(
                new BucketExistsArgs().WithBucket(bucketName));

            if (!bucketExists)
            {
                await _minioClient.MakeBucketAsync(
                    new MakeBucketArgs().WithBucket(bucketName));
            }

            await using var stream = cardFile.OpenReadStream();
            await _minioClient.PutObjectAsync(new PutObjectArgs()
                .WithBucket(bucketName)
                .WithObject(objectKey)
                .WithStreamData(stream)
                .WithObjectSize(stream.Length)
                .WithContentType("image/png"));

            /*
             * Записать lastname, firstname, nickname, link в Базу Данных
             */
        }
    }
}
