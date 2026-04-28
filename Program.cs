using AniTyan.Models.Entities;
using Minio;
using Minio.DataModel.Args;
using AniTyan.Models.Services.KoikatsuCardService;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Добавляем Swagger генератор
builder.Services.AddEndpointsApiExplorer(); // необходимо для Swagger
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient<IKoikatsuCardService, KoikatsuCardService>(client =>
{
    client.BaseAddress = new Uri("http://koikatsu-card-service:8000/");
    client.Timeout = TimeSpan.FromSeconds(10);
});

var minioConfig = builder.Configuration.GetSection("MinIO");
builder.Services.AddMinio(
    accessKey: minioConfig["AccessKey"],
    secretKey: minioConfig["SecretKey"]
    );

var app = builder.Build();

// Настройка конвейера HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();          // генерирует swagger.json
    app.UseSwaggerUI();        // включает Swagger UI (доступен по /swagger)
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();