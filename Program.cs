using Cassandra;
using Neo4j.Driver;
using StackExchange.Redis;
using HoloRedAPI.Repositories;
using HoloRedAPI.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// --- 1. CONEXIÓN A MOTORES POLÍGLOTAS ---

// REDIS: abortConnect=false permite que la API arranque aunque el radar esté apagado
builder.Services.AddSingleton<IConnectionMultiplexer>(
    ConnectionMultiplexer.Connect("localhost:6379,password=ImperioCaido2026*,abortConnect=false"));

// CASSANDRA: Se inyecta el ICluster para permitir la conexión diferida (Lazy-Load)
var cassandraCluster = Cluster.Builder()
                              .AddContactPoint("127.0.0.1")
                              .WithCredentials("admin", "TelemetriaSegura2026*")
                              .Build();
builder.Services.AddSingleton<ICluster>(cassandraCluster);

// NEO4J: Conexión nativa Bolt
builder.Services.AddSingleton<IDriver>(
    GraphDatabase.Driver("bolt://localhost:7687", AuthTokens.Basic("neo4j", "InteligenciaGrafo2026*")));


// --- 2. INYECCIÓN DE DEPENDENCIAS (IoC) ---

// CRÍTICO: FlotaService DEBE ser Singleton para que todos los hilos compartan el mismo 'lock'
builder.Services.AddSingleton<FlotaService>();

builder.Services.AddScoped<RedisRepository>();
builder.Services.AddScoped<CassandraRepository>();
builder.Services.AddScoped<Neo4jRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();

app.Run();