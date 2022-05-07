using Microsoft.AspNetCore.Mvc;
using AutoFixture;

namespace CassandraDemo.Controllers;

[ApiController]
[Route("[controller]")]
public class CassandraDemoController : ControllerBase
{
    private readonly UsersRepository _usersRepository;

    public CassandraDemoController(UsersRepository usersRepository)
    {
        _usersRepository = usersRepository;
    }

    [HttpGet]
    public async Task<IEnumerable<User>> GetUsers() =>
        await _usersRepository.GetUsers();

    [HttpPost]
    public async Task CreateUser([FromBody] User user) =>
        await _usersRepository.CreateUser(user);

    [HttpPost("GenerateUsers")]
    public async Task GenerateUsers([FromBody] int count)
    {
        var users = new Fixture().CreateMany<User>(count).ToList();
        await _usersRepository.BulkInsert(users);
    }

    [HttpPut]
    public async Task UpdateUser([FromBody] User user) =>
        await _usersRepository.UpdateUserById(user);

    [HttpDelete("{userId:guid}")]
    public async Task DeleteUser([FromRoute] Guid userId) =>
        await _usersRepository.DeleteUserById(userId);
    
    [HttpDelete("DeleteAllUsers")]
    public async Task DeleteAllUsers() =>
        await _usersRepository.DeleteAllUsers();
}