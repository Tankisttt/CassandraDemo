using Cassandra;
using ISession = Cassandra.ISession;

namespace CassandraDemo.Repositories;

public class VariantsPlanRepository
{
    private readonly ISession _session;

    public VariantsPlanRepository(ISession session)
    {
        _session = session;
    }

    public async Task<IEnumerable<VariantPlan>> GetVariantPlans()
    {
        const string cqlQuery = "SELECT * FROM variant_plans";
        var variantsFromDb = await _session.ExecuteAsync(new SimpleStatement(cqlQuery));

        return variantsFromDb.Select(resultRow => new VariantPlan
            {
                TaskNumber = int.Parse(resultRow["task_number"].ToString()!),
                Requirements = resultRow["requirements"].ToString()!,
                MaxScore = int.Parse(resultRow["max_score"].ToString()!),
                Difficulty = resultRow["difficulty"].ToString()!,
                RequirementCodes = resultRow["requirement_codes"].ToString()!,
                RequirementContentCodes = resultRow["requirement_content_codes"].ToString()!,
                ExamYear = int.Parse(resultRow["exam_year"].ToString()!)
            })
            .ToList();
    }
    
    public async Task DeleteAllVariantPlans() => await _session.ExecuteAsync(new SimpleStatement("TRUNCATE variant_plans"));
    
    public async Task CreateVariantPlan(VariantPlan variantPlan)
    {
        var insertCqlStatement = await GetInsertCqlStatement(variantPlan);
        await _session.ExecuteAsync(insertCqlStatement);
    }
    
    public async Task BulkInsert(IEnumerable<VariantPlan> variantPlans)
    {
        var batchStmt = new BatchStatement();
        foreach (var variantPlan in variantPlans)
            batchStmt.Add(await GetInsertCqlStatement(variantPlan));

        await _session.ExecuteAsync(batchStmt);
    }
    
    private async Task<BoundStatement> GetInsertCqlStatement(VariantPlan variantPlan)
    {
        const string cqlQuery =
            "INSERT INTO variant_plans (task_number, requirements, max_score, difficulty, requirement_codes, requirement_content_codes, exam_year) VALUES(?,?,?,?,?,?,?)";
        var preparedStmt = await _session.PrepareAsync(cqlQuery);
        return preparedStmt.Bind(variantPlan.TaskNumber, variantPlan.Requirements, variantPlan.MaxScore, variantPlan.Difficulty,
            variantPlan.RequirementCodes, variantPlan.RequirementContentCodes, variantPlan.ExamYear);
    }
}