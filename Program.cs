using GarageApp.Database;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//можеш так запускати, тільки треба заново зробити міграцію просто пишеш dotnet ef database update, може навіть без цього запуститься
//якщо ні то напиши builder.Configuration.GetConnectionString("DBContextLocalHost")
builder.Services.AddDbContext<AppDbContext>(optionsBuilder => optionsBuilder.UseSqlServer(builder.Configuration.GetConnectionString("DBContextLocalDB"))); 

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
