using AutoFixture;
using CassandraDemo.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace CassandraDemo.Controllers;

[ApiController]
[Route("[controller]")]
public class ResultsController : ControllerBase
{
    private readonly ResultsRepository _resultsRepository;
    private readonly Fixture _fixture = new();

    public ResultsController(ResultsRepository resultsRepository)
    {
        _resultsRepository = resultsRepository;
    }

    [HttpGet]
    public async Task<IEnumerable<ExamResult>> GetExamResults() =>
        await _resultsRepository.GetResults();

    [HttpPost]
    public async Task AddResult([FromBody] ExamResult result) =>
        await _resultsRepository.CreateExamResult(result);

    [HttpPost("GenerateResults")]
    public async Task GenerateResults([FromBody] int count)
    {
        var users = _fixture
            .Build<ExamResult>()
            .Without(r => r.Score)
            .CreateMany(count)
            .ToList();
        users.ForEach(x => x.Score = x.PrimaryScore * 3);
        await _resultsRepository.BulkInsert(users);
    }

    [HttpPut]
    public async Task UpdateResult([FromBody] ExamResult result) =>
        await _resultsRepository.UpdateExamResultById(result);

    [HttpDelete("{examId:guid}")]
    public async Task DeleteExamById([FromRoute] Guid examId, [FromQuery] int schoolNumber,
        [FromQuery] string classNumber) =>
        await _resultsRepository.DeleteExamResultById(examId, schoolNumber, classNumber);

    [HttpDelete("DeleteAllExams")]
    public async Task DeleteAllExams() =>
        await _resultsRepository.DeleteAllExamResults();
}