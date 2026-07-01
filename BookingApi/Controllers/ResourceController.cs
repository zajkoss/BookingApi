using BookingApi.Commands;
using BookingApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetAllAsync()
    {
        return Ok(await _resourceService.GetAllAsync());
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> GetByIdAsync(Guid id)
    {
        var resource = await _resourceService.GetByIdAsync(id);
        if (resource is null) return NotFound();
        return Ok(resource);
    }


    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> PostAsync([FromBody] CreateResourceCommand newResource)
    {
        var created = await _resourceService.CreateAsync(newResource);
        return Created($"/api/v1/resources/{created.Id}", created);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> DeactivateAsync([FromRoute] Guid id, [FromBody] UpdateResourceCommand newResource)
    {
        var updated = await _resourceService.UpdateAsync(id, newResource);
        if (updated is null) return NotFound();
        return Ok(updated);
    }

    [Authorize(Roles = "Admin")]
    [HttpPatch("{id}/deactive")]
    public async Task<IActionResult> DeactivateAsync([FromRoute] Guid id)
    {
        var updated = await _resourceService.DeactiveAsync(id);
        if (updated is null) return NotFound();
        return Ok(updated);
    }
}