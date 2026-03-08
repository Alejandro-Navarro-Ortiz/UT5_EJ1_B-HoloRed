using Cassandra;
using Microsoft.Extensions.DependencyInjection;
using Neo4j.Driver;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Añadir soporte para Controladores y Swagger (interfaz visual para probar la API)
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// --- 1. CONEXIONES A LAS BASES DE DATOS ---

// Redis (Radar)
builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect("localhost:6379,password=ImperioCaido2026*"));

// Cassandra (Telemetría)
var cassandraCluster = Cluster.Builder()
                              .AddContactPoint("127.0.0.1")
                              .WithCredentials("admin", "TelemetriaSegura2026*")
                              .Build();
builder.Services.AddSingleton<Cassandra.ISession>(cassandraCluster.Connect());

// Neo4j (Inteligencia)
builder.Services.AddSingleton<IDriver>(GraphDatabase.Driver("bolt://localhost:7687", AuthTokens.Basic("neo4j", "InteligenciaGrafo2026*")));


// --- 2. INYECCIÓN DE SERVICIOS Y REPOSITORIOS ---

// ATENCIÓN CRÍTICA: FlotaService DEBE ser Singleton para que el 'lock' (hilos) 
// sea el mismo para todas las peticiones concurrentes y no choquen las naves.
builder.Services.AddSingleton<FlotaService>();

builder.Services.AddScoped<RedisRepository>();
builder.Services.AddScoped<CassandraRepository>();
builder.Services.AddScoped<Neo4jRepository>();

var app = builder.Build();

// Activar Swagger para poder probar la API desde el navegador
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();

app.Run();