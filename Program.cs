using AniTyan.Data;
using AniTyan.Models.Entities;
using AniTyan.Models.Services.AniTyanService;
using AniTyan.Models.Services.KoikatsuCardService;
using Microsoft.EntityFrameworkCore;
using Minio;
using Minio.DataModel.Args;

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

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<AnimeGirlMaker>();
builder.Services.AddScoped<IKoikatsuCardService, KoikatsuCardService>();

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