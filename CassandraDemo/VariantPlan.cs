using System.ComponentModel.DataAnnotations;

#pragma warning disable CS8618

namespace CassandraDemo;

public class VariantPlan
{
    [Range(1, 100)]
    public int TaskNumber { get; set; }

    public string Requirements { get; set; }

    [Range(0, 100)]
    public int MaxScore { get; set; }

    [MaxLength(32)]
    public string Difficulty { get; set; }
    
    [MaxLength(16)]
    public string RequirementCodes { get; set; }
    
    [MaxLength(256)]
    public string RequirementContentCodes { get; set; }
    
    [Range(1990, 2100)]
    public int ExamYear { get; set; }
}