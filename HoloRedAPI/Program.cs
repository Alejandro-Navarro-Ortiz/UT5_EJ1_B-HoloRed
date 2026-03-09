using Cassandra;
using Neo4j.Driver;
using StackExchange.Redis;
using HoloRedAPI.Repositories;
using HoloRedAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Configuración básica de la API y de la documentación de Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// =========================================================
// --- 1. CONEXIÓN A MOTORES POLÍGLOTAS (Bases de Datos) ---
// =========================================================

// REDIS (Clave-Valor): Base de datos en memoria para caché y operaciones de radar.
// 'abortConnect=false' permite que la API arranque y funcione aunque Redis esté temporalmente caído.
builder.Services.AddSingleton<IConnectionMultiplexer>(
    ConnectionMultiplexer.Connect("localhost:6379,password=ImperioCaido2026*,abortConnect=false"));

// CASSANDRA (Columnar): Sistema distribuido para alto volumen de escrituras (telemetría).
// Se inyecta el 'ICluster' para permitir una conexión diferida (Lazy-Load).
var cassandraCluster = Cluster.Builder()
                              .AddContactPoint("127.0.0.1")
                              .WithCredentials("admin", "TelemetriaSegura2026*")
                              .Build();
builder.Services.AddSingleton<ICluster>(cassandraCluster);

// NEO4J (Grafos): Base de datos para trazar relaciones complejas (red de inteligencia y traidores).
// Utiliza conexión nativa a través del protocolo Bolt.
builder.Services.AddSingleton<IDriver>(
    GraphDatabase.Driver("bolt://localhost:7687", AuthTokens.Basic("neo4j", "InteligenciaGrafo2026*")));


// =========================================================
// --- 2. INYECCIÓN DE DEPENDENCIAS (IoC) ------------------
// =========================================================

// CRÍTICO: 'FlotaService' DEBE ser 'Singleton' para que todas las peticiones (hilos) 
// compartan la misma instancia en memoria y, por tanto, el mismo objeto 'lock' para evitar colisiones.
builder.Services.AddSingleton<FlotaService>();

// Los repositorios se configuran como 'Scoped' (se crea una instancia por cada petición HTTP).
builder.Services.AddScoped<RedisRepository>();
builder.Services.AddScoped<CassandraRepository>();
builder.Services.AddScoped<Neo4jRepository>();

var app = builder.Build();

// Configuración del pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();

// Arranca la aplicación
app.Run();