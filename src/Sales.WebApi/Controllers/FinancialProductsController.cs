using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sales.WebApi.Persistence;

namespace Sales.WebApi.Controllers;

[ApiController]
[Route("api/sales/financial-products")]
public class FinancialProductsController(SalesDbContext db) : ControllerBase
{
    /// <summary>
    /// Get all financial products
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        Ok(await db.FinancialProducts.ToListAsync());
}
