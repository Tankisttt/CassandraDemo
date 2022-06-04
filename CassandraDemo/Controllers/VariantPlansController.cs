using AutoFixture;
using CassandraDemo.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace CassandraDemo.Controllers;

[ApiController]
[Route("[controller]")]
public class VariantPlansController
{
    private readonly VariantsPlanRepository _variantsPlanRepository;

    public VariantPlansController(VariantsPlanRepository variantsPlanRepository)
    {
        _variantsPlanRepository = variantsPlanRepository;
    }
    
    [HttpGet]
    public async Task<IEnumerable<VariantPlan>> GetUsers() =>
        await _variantsPlanRepository.GetVariantPlans();

    [HttpPost]
    public async Task AddVariantPlan([FromBody] VariantPlan variantPlan) =>
        await _variantsPlanRepository.CreateVariantPlan(variantPlan);

    [HttpPost("GenerateVariantPlans")]
    public async Task GenerateVariantPlans([FromBody] int count)
    {
        var users = new Fixture().CreateMany<VariantPlan>(count).ToList();
        await _variantsPlanRepository.BulkInsert(users);
    }
    
    [HttpDelete("DeleteAllVariantPlans")]
    public async Task DeleteAllVariantPlans() =>
        await _variantsPlanRepository.DeleteAllVariantPlans();
}