using Cassandra;
using CassandraDemo;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

await RegisterCassandraAndCreateUsersTable(builder.Services);

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

static async Task RegisterCassandraAndCreateUsersTable(IServiceCollection serviceCollection)
{
    var cluster = Cluster.Builder()
        .AddContactPoint("127.0.0.1")
        .WithPort(9042)
        .Build();

    using var defaultSession = await cluster.ConnectAsync();
    await defaultSession.ExecuteAsync(new SimpleStatement(
        "CREATE KEYSPACE IF NOT EXISTS users WITH REPLICATION = { 'class' : 'SimpleStrategy', 'replication_factor' : '1' };"));
    var usersSession = defaultSession.Cluster.Connect("users");

    usersSession.Execute(
        "CREATE TABLE IF NOT EXISTS users.users (id uuid PRIMARY KEY,email text,firstname text,lastname text,age int,country text)");

    serviceCollection.AddSingleton(usersSession);
}