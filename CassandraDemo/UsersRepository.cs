using Cassandra;
using ISession = Cassandra.ISession;

namespace CassandraDemo;

public class UsersRepository
{
    private readonly ISession _session;

    public UsersRepository(ISession session)
    {
        _session = session;
    }

    public async Task<IEnumerable<User>> GetUsers()
    {
        const string cqlQuery = "SELECT * FROM users";
        var usersFromDb = await _session.ExecuteAsync(new SimpleStatement(cqlQuery));

        return usersFromDb.Select(userRow => new User
            {
                Id = Guid.Parse(userRow["id"].ToString()!),
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

    public async Task BulkInsert(IEnumerable<User> users)
    {
        var batchStmt = new BatchStatement();

        foreach (var user in users)
            batchStmt.Add(await GetInsertCqlStatement(user));
        
        await _session.ExecuteAsync(batchStmt);
    }

    public async Task DeleteUserById(Guid userId)
    {
        var statement = new SimpleStatement($"DELETE FROM users WHERE id = {userId}");
        await _session.ExecuteAsync(statement);
    }

    public async Task CreateUser(User user)
    {
        var insertCqlStatement = await GetInsertCqlStatement(user);
        await _session.ExecuteAsync(insertCqlStatement);
    }

    public async Task DeleteAllUsers() => await _session.ExecuteAsync(new SimpleStatement("TRUNCATE users"));

    private async Task<BoundStatement> GetInsertCqlStatement(User user)
    {
        const string cqlQuery =
            "INSERT INTO users (id, email, firstname, lastname, age, country) VALUES(?,?,?,?,?,?)";
        var preparedStmt = await _session.PrepareAsync(cqlQuery);
        return preparedStmt.Bind(user.Id, user.Email, user.FirstName, user.LastName, user.Age, user.Country);
    }
}