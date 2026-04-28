// Importiere deine Schichten
using InfraDesk.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Hier rufen wir die Methode auf, die wir vorhin in der Infrastructure erstellt haben
builder.Services.AddInfrastructure(builder.Configuration);

// ... restlicher Standardcode (AddControllers, etc.)

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
