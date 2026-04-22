var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Добавляем Swagger генератор
builder.Services.AddEndpointsApiExplorer(); // необходимо для Swagger
builder.Services.AddSwaggerGen();

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