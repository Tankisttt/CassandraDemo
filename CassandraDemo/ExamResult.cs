using System.ComponentModel.DataAnnotations;

#pragma warning disable CS8618

namespace CassandraDemo;

public class ExamResult
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }
    
    [Range(0, 32)]
    public int PrimaryScore { get; set; }
    
    [Range(0, 100)]
    public int Score { get; set; }
    
    [Range(200000, 300000)]
    public int SchoolNumber { get; set; }

    [StringLength(5)]
    public string ClassNumber { get; set; }
}