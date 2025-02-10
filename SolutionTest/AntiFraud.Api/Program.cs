using AntiFraud.Api.Mappers;
using AntiFraud.Core.Interface;
using AntiFraud.Infrastructure;
using AntiFraud.Infrastructure.Config;
using AntiFraud.Infrastructure.Kafka;
using AntiFraud.Infrastructure.Services;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using System.Reflection;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddFastEndpoints();
builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AntiFraudDbContext>(options =>
    options.UseNpgsql(connectionString));


string kafkaBootstrapServers = builder.Configuration["Kafka:BootstrapServers"] ?? "localhost:9092";
builder.Services.AddSingleton(new KafkaProducer(kafkaBootstrapServers));
builder.Services.AddScoped<ITransactionService, TransactionService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<KafkaSettings>(builder.Configuration.GetSection("KafkaSettings"));

builder.Services.AddHostedService<KafkaConsumer>();//TO DO: en caso de no encontrar configuracion (ambiente kafka) comentar

var app = builder.Build();

app.UseFastEndpoints();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
