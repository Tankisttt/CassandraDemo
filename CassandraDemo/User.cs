using System.ComponentModel.DataAnnotations;

namespace CassandraDemo;

public class User
{
    public Guid Id { get; set; }
    public string Email { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    [Range(0, 125)]
    public int Age { get; set; }

    public string? Country { get; set; }
}