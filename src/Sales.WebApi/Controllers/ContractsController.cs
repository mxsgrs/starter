using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sales.WebApi.Persistence;

namespace Sales.WebApi.Controllers;

public record CreateContractRequest(Guid UserId, Guid FinancialProductId);
public record UpdateContractRequest(Guid FinancialProductId);

[ApiController]
[Route("api/sales/contracts")]
public class ContractsController(SalesDbContext db) : ControllerBase
{
    /// <summary>
    /// Get all contracts
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        List<Contract> contracts = await db.Contracts
            .Include(c => c.FinancialProduct)
            .ToListAsync();
        return Ok(contracts);
    }

    /// <summary>
    /// Get a contract by id
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        Contract? contract = await db.Contracts
            .Include(c => c.FinancialProduct)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (contract is null) return NotFound();
        return Ok(contract);
    }

    /// <summary>
    /// Create a new contract
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateContractRequest request)
    {
        bool userExists = await db.Users.AnyAsync(u => u.Id == request.UserId);
        if (!userExists) return UnprocessableEntity($"User {request.UserId} not found in Sales database.");

        bool productExists = await db.FinancialProducts.AnyAsync(p => p.Id == request.FinancialProductId);
        if (!productExists) return UnprocessableEntity($"Financial product {request.FinancialProductId} not found.");

        Contract contract = Contract.Create(request.UserId, request.FinancialProductId);
        db.Contracts.Add(contract);
        await db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = contract.Id }, contract);
    }

    /// <summary>
    /// Update the financial product of a contract
    /// </summary>
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateContractRequest request)
    {
        Contract? contract = await db.Contracts.FindAsync(id);
        if (contract is null) return NotFound();

        bool productExists = await db.FinancialProducts.AnyAsync(p => p.Id == request.FinancialProductId);
        if (!productExists) return UnprocessableEntity($"Financial product {request.FinancialProductId} not found.");

        contract.UpdateFinancialProduct(request.FinancialProductId);
        await db.SaveChangesAsync();

        return NoContent();
    }

    /// <summary>
    /// Delete a contract
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        int deleted = await db.Contracts.Where(c => c.Id == id).ExecuteDeleteAsync();
        if (deleted == 0) return NotFound();
        return NoContent();
    }
}
