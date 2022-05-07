namespace CassandraDemo;

public class User
{
    public long Id { get; set; }
    public string Email { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public int Age { get; set; }

    public string? Country { get; set; }
}