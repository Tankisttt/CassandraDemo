using Cassandra;

namespace CassandraDemo;

public class UsersRepository
{
    private readonly Cassandra.ISession _session;

    public UsersRepository()
    {
        var cluster = Cluster.Builder()
            .AddContactPoint("127.0.0.1")
            .WithPort(9042)
            .Build();
        _session = cluster.Connect("users");
    }

    public async Task<IEnumerable<User>> GetUsers()
    {
        const string cqlQuery = "SELECT * FROM users.users";
        var usersFromDb = await _session.ExecuteAsync(new SimpleStatement(cqlQuery));

        return usersFromDb.Select(userRow => new User
            {
                Id = int.Parse(userRow["id"].ToString()!),
                Email = userRow["email"].ToString()!,
                FirstName = userRow["firstname"].ToString()!,
                LastName = userRow["lastname"].ToString()!,
                Age = int.Parse(userRow["age"].ToString()!),
                Country = userRow["country"].ToString()
            })
            .ToList();
    }

    public async Task UpdateUserById(User user)
    {
        var cqlQuery =
            $"UPDATE users SET email='{user.Email}', firstname='{user.FirstName}', lastname='{user.LastName}', age={user.Age}, country='{user.Country}' WHERE id={user.Id} IF EXISTS";
        var statement = new SimpleStatement(cqlQuery);
        await _session.ExecuteAsync(statement);
    }

    public async Task DeleteUserById(long userId)
    {
        var statement = new SimpleStatement($"DELETE FROM users WHERE id = {userId}");
        await _session.ExecuteAsync(statement);
    }

    public async Task CreateUser(User user)
    {
        const string cqlQuery =
            "INSERT INTO users.users (id, email, firstname, lastname, age, country) VALUES(?,?,?,?,?,?)";
        var preparedStmt = await _session.PrepareAsync(cqlQuery);
        var statement = preparedStmt.Bind(user.Id, user.Email, user.FirstName, user.LastName, user.Age, user.Country);
        await _session.ExecuteAsync(statement);
    }
}