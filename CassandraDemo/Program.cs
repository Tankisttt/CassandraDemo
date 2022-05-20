using Cassandra;
using CassandraDemo.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

await RegisterCassandraAndCreateTables(builder.Services);
builder.Services.AddSingleton<UsersRepository>();

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

static async Task RegisterCassandraAndCreateTables(IServiceCollection serviceCollection)
{
    var cluster = Cluster.Builder()
        .AddContactPoint("127.0.0.1")
        .WithPort(9042)
        .Build();

    var defaultSession = await cluster.ConnectAsync();
    await defaultSession.ExecuteAsync(new SimpleStatement(
        "CREATE KEYSPACE IF NOT EXISTS exams WITH REPLICATION = { 'class' : 'SimpleStrategy', 'replication_factor' : '1' };"));

    var examsSession = await defaultSession.Cluster.ConnectAsync("exams");
    await examsSession.ExecuteAsync(new SimpleStatement(
        "CREATE TABLE IF NOT EXISTS exams.users (id uuid PRIMARY KEY,email text,firstname text,lastname text,age int,country text)"));

    serviceCollection.AddSingleton(examsSession);
}