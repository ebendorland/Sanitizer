using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.OpenApi.Models;
using Sanitizer.Controllers;
using Sanitizer.Core.Exceptions;
using Sanitizer.Core.Interfaces;
using Sanitizer.Core.Models;
using System.ComponentModel;
using System.Drawing.Printing;

namespace SanitizerAPI.Controllers
{
    /// <summary>
    /// CRUD Api for management of Sensitive Words
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class SensitiveWordsController : ControllerBase
    {
        private readonly ISensitiveWordsRepo _repo;
        private readonly ILogger<SanitizerController> _logger;

        /// <summary>
        /// CRUD Api for management of Sensitive Words
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="repo"></param>
        public SensitiveWordsController(ILogger<SanitizerController> logger, ISensitiveWordsRepo repo)
        {
            _logger = logger;
            _repo = repo;
        }

        /// <summary>
        /// Gets a paged list of Sensitive Words
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="sortOrder">1(asc) or -1(desc)</param>
        /// <param name="search"></param>
        /// <response code="200">Returns a list of words and pagination info</response>
        /// <response code="400">An error has occured</response>
        [HttpGet("GetWordsList")]
        public async Task<ActionResult<PaginationResponse<string>>> GetList([FromQuery, BindRequired]int page, [FromQuery, BindRequired] int pageSize, [FromQuery, BindRequired] int sortOrder, string search = "")
        {
            var words = await _repo.GetSensitiveWords(page, pageSize, sortOrder, search: search);
            return Ok(words);
        }
        /// <summary>
        /// Get a single Sensitive Word. Used to check if word is present in the database
        /// </summary>
        /// <param name="sensitiveWord"></param>
        /// <response code="200">Returns a single word</response>
        /// <response code="400">An error has occured</response>
        /// <response code="404">No matching word found</response>
        [HttpGet(Name = "GetWord")]
        public async Task<ActionResult<string>> Get([FromQuery, BindRequired]string sensitiveWord)
        {
            try
            {
                return Ok(await _repo.GetSensitiveWord(sensitiveWord));
            }
            catch (ApiException ex)
            {
                return ex.StatusCode switch
                {
                    404 => NotFound(ex.Message),
                    _ => (ex.Message)
                };
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// Used to create a single Sensitive Word
        /// </summary>
        /// <param name="word"></param>
        /// <response code="201">Word successfully created</response>
        /// <response code="400">Could not create word with parameter</response>
        /// <response code="409">Word already exists</response>
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status409Conflict)]
        [HttpPost(Name = "CreateWord")]
        public async Task<ActionResult> Create([FromBody, BindRequired] string word)
        {
            try
            {
                await _repo.CreateSensitiveWord(word);
                return CreatedAtAction(nameof(Get), word);
            }
            catch (ApiException ex)
            {
                return ex.StatusCode switch
                {
                    409 => Conflict(ex.Message),
                    _ => BadRequest(ex.Message)
                };
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// Update a single Sensitive Word
        /// </summary>
        /// <param name="word"></param>
        /// <response code="204">Word successfully updated</response>
        /// <response code="400">An error has occured</response>
        /// <response code="409">Word already exists</response>

        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(void), StatusCodes.Status409Conflict)]
        [HttpPut(Name = "UpdateWord")]
        public async Task<ActionResult> Update(SensitiveWord word)
        {
            if (string.IsNullOrEmpty(word.OldWord))
                return BadRequest("OldWord is required.");
            if (string.IsNullOrEmpty(word.NewWord))
                return BadRequest("NewWord is required.");
            try
            {
                await _repo.UpdateSensitiveWord(word);
                return NoContent();
            }
            catch (ApiException ex)
            {
                return ex.StatusCode switch
                {
                    404 => NotFound(ex.Message),
                    409 => Conflict(ex.Message),
                    _ => BadRequest(ex.Message)
                };
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// Delete a single Sensitive Word
        /// </summary>
        /// <param name="sensitiveWord"></param>
        /// <response code="204">Word successfully deleted</response>
        /// <response code="400">An error has occured</response>
        /// [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [HttpDelete(Name = "DeleteWord")]
        public async Task<ActionResult> Delete([FromQuery, BindRequired] string sensitiveWord)
        {
            try
            {
                await _repo.DeleteSensitiveWord(sensitiveWord);
                return NoContent();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
    }
}
