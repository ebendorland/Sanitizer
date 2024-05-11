using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore.Metadata;
using Sanitizer.Core.Interfaces;
using Sanitizer.Library.Services;

namespace Sanitizer.Controllers;

/// <summary>
/// Api for sanitization of strings
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class SanitizerController : ControllerBase
{
    private readonly SanitizerService _service;
    private readonly ILogger<SanitizerController> _logger;

    /// <summary>
    /// Api for sanitization of strings
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="service"></param>
    public SanitizerController(ILogger<SanitizerController> logger, SanitizerService service)
    {
        _logger = logger;
        _service = service;
    }

    /// <summary>
    /// Get a sanitized string
    /// </summary>
    /// <param name="dirtyString"></param>
    /// <response code="200">Returns the sanitized string</response>
    /// <response code="400">An error has occured</response> 
    [HttpGet(Name = "GetSanitizedString")]
    public async Task<ActionResult<string>> Get([FromQuery, BindRequired] string dirtyString)
    {
        var cleanString = await _service.SanitizeString(dirtyString);
        return Ok(cleanString);
    }
}
