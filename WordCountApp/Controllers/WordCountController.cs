using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WordCountApp.Models;
using WordCountApp.Services;

namespace WordCountApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WordCountController : ControllerBase
    {
        private readonly IWordCountService _wordCountService;

        public WordCountController(IWordCountService wordCountService)
        {
            _wordCountService = wordCountService;
        }

        /// <summary>
        /// Uploads a text file and returns the word count results.
        /// </summary>
        /// <param name="file">The text file to process.</param>
        /// <returns>An IActionResult containing the word count results or an error message.</returns>
        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }
            try
            {
                string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(file.FileName);

                string text;
                using (var reader = new StreamReader(file.OpenReadStream()))
                {
                    text = await reader.ReadToEndAsync();
                }

                var result = _wordCountService.CountWords(text);
                await _wordCountService.UploadToS3Async(result, fileNameWithoutExtension);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        /// <summary>
        /// Retrieves word count results for a specific file stored in S3.
        /// </summary>
        /// <param name="fileKey">The S3 file key to retrieve results from.</param>
        /// <returns>An IActionResult containing the word count results or an error message.</returns>
        [HttpGet("retrieve")]
        public async Task<IActionResult> RetrieveFile([FromQuery] string fileKey)
        {
            try
            {
                var result = await _wordCountService.RetrieveFromS3Async(fileKey);
                if (result != null)
                    return Ok(result);
                else
                    return NotFound($"The file with key {fileKey} was not found.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

    }
}
