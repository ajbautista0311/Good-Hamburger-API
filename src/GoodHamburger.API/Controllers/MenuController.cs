namespace GoodHamburger.API.Controllers;

using Microsoft.AspNetCore.Mvc;
using GoodHamburger.Domain.Repositories;

[ApiController]
[Route("api/[controller]")]
[Tags("Menu")]
public class MenuController(IMenuRepository menuRepository) : ControllerBase
{
    private readonly IMenuRepository _menuRepository = menuRepository;

    /// <summary>Get all menu</summary>
    [HttpGet]
    public async Task<IActionResult> GetAllItems()
    {
        var items = await _menuRepository.GetAllItemsAsync();
        
        if (items is null || items.Count is 0)
        {
            return Ok(new { success = false, message = "No items found." });
        }
        
        return Ok(new { success = true, data = items });
    }

    /// <summary>Get only sandwiches</summary>
    [HttpGet("sandwiches")]
    public async Task<IActionResult> GetSandwiches()
    {
        var sandwiches = await _menuRepository.GetSandwichesAsync();

        if (sandwiches is null || sandwiches.Count is 0)
        {
            return Ok(new { success = false, message = "No items found." });
        }

        return Ok(new { success = true, data = sandwiches });
    }

    /// <summary>Get only extras</summary>
    [HttpGet("extras")]
    public async Task<IActionResult> GetExtras()
    {
        var extras = await _menuRepository.GetExtrasAsync();

        if (extras is null || extras.Count is 0)
        {
            return Ok(new { success = false, message = "No items found." });
        }

        return Ok(new { success = true, data = extras });
    }
}