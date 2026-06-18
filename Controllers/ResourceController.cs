using BookingApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookingApi.Controllers;
[ApiController]
[Route("api/v1/resources")]
public class ResourceController : ControllerBase
{
    private readonly ILogger<ResourceController> _logger;
    private readonly IResourceService _resourceService;

    public ResourceController(ILogger<ResourceController> logger, IResourceService resourceService)
    {
        _logger = logger;
        _resourceService = resourceService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAsync()
    {
        return Ok(await _resourceService.GetAllAsync());
    }
    
    
    
}