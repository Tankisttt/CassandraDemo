using Microsoft.AspNetCore.Mvc;

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

    [HttpPut]
    public async Task UpdateUser([FromBody] User user) =>
        await _usersRepository.UpdateUserById(user);

    [HttpDelete]
    public async Task DeleteUser([FromRoute] long userId) =>
        await _usersRepository.DeleteUserById(userId);
}