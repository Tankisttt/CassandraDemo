using Cassandra;
using ISession = Cassandra.ISession;

namespace CassandraDemo.Repositories;

public class ResultsRepository
{
    private readonly ISession _session;

    public ResultsRepository(ISession session)
    {
        _session = session;
    }

    public async Task<IEnumerable<ExamResult>> GetResults()
    {
        const string cqlQuery = "SELECT * FROM results";
        var usersFromDb = await _session.ExecuteAsync(new SimpleStatement(cqlQuery));

        return usersFromDb.Select(resultRow => new ExamResult
            {
                Id = Guid.Parse(resultRow["id"].ToString()!),
                UserId = Guid.Parse(resultRow["user_id"].ToString()!),
                PrimaryScore = int.Parse(resultRow["primary_score"].ToString()!),
                Score = int.Parse(resultRow["exam_score"].ToString()!),
                SchoolNumber = int.Parse(resultRow["school_number"].ToString()!),
                ClassNumber = resultRow["class_number"].ToString()!
            })
            .ToList();
    }

    public async Task UpdateExamResultById(ExamResult result)
    {
        var cqlQuery =
            $"UPDATE results SET user_id={result.UserId}, primary_score={result.PrimaryScore}, exam_score={result.Score} WHERE school_number={result.SchoolNumber} AND class_number='{result.ClassNumber}' AND id={result.Id} IF EXISTS";
        var statement = new SimpleStatement(cqlQuery);
        await _session.ExecuteAsync(statement);
    }

    public async Task BulkInsert(IEnumerable<ExamResult> results)
    {
        var batchStmt = new BatchStatement();
        foreach (var result in results)
            batchStmt.Add(await GetInsertCqlStatement(result));

        await _session.ExecuteAsync(batchStmt);
    }

    public async Task DeleteExamResultById(Guid resultId, int schoolNumber, string classNumber)
    {
        var statement =
            new SimpleStatement(
                $"DELETE FROM results WHERE school_number={schoolNumber} AND class_number='{classNumber}' AND id = {resultId}");
        await _session.ExecuteAsync(statement);
    }

    public async Task CreateExamResult(ExamResult result)
    {
        var insertCqlStatement = await GetInsertCqlStatement(result);
        await _session.ExecuteAsync(insertCqlStatement);
    }

    public async Task DeleteAllExamResults() => await _session.ExecuteAsync(new SimpleStatement("TRUNCATE results"));

    private async Task<BoundStatement> GetInsertCqlStatement(ExamResult result)
    {
        const string cqlQuery =
            "INSERT INTO results (id, user_id, primary_score, exam_score, school_number, class_number) VALUES(?,?,?,?,?,?)";
        var preparedStmt = await _session.PrepareAsync(cqlQuery);
        return preparedStmt.Bind(result.Id, result.UserId, result.PrimaryScore, result.Score, result.SchoolNumber,
            result.ClassNumber);
    }
}