using Microsoft.AspNetCore.Mvc;
using FileStorageApi.DTOs;

namespace FileStorageApi.Controllers
{
    [ApiController]
    [Route("api/files")]
    public class FilesController : ControllerBase
    {
        [HttpGet]
        public IActionResult FileList()
        {
            return Ok("Files List End-point");
        }

        [HttpGet("status")]
        public IActionResult FileStorageStatus()
        {
            return Ok("File Storage Service is running");
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(FileDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<FileDto> GetById(int id)
        {
            var sampleFile = new FileDto
            {
                Id = 55,
                FileName = "SampleFile.pdf"
            };

            if (id <= 0)
            {
                return Problem(
                    title: "invalid file id",
                    detail: "the file id must be greater than zero",
                    statusCode: StatusCodes.Status400BadRequest
                    );
            }

            if (id != sampleFile.Id)
            {
                return Problem(
                    title: "file not found",
                    detail: "the file was not found in the database",
                    statusCode: StatusCodes.Status404NotFound
                    );
            }

            return Ok(sampleFile);
        }

        [HttpGet("search")]
        public IActionResult Search([FromQuery] string? extension, [FromQuery] string? searchTerm)
        {
            var result = new
            {
                extension = extension,
                searchTerm = searchTerm
            };

            if (string.IsNullOrWhiteSpace(extension) && string.IsNullOrWhiteSpace(searchTerm))
            {
                return BadRequest("there was nothing to search based on!");
            }

            return Ok(result);

        }
        [HttpDelete("delete-by-id/{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult DeleteById(int id)
        {
            var sampleFile = new FileDto
            {
                Id = 55,
                FileName = "SampleFile.pdf"
            };

            if (id <= 0)
            {
                return Problem(
                    title: "invalid id",
                    detail: "the id must be greater than zero",
                    statusCode: StatusCodes.Status400BadRequest);
            }

            if (id != sampleFile.Id)
            {
                return Problem(
                    title: "file not found",
                    detail: "the file was not found in the database!",
                    statusCode: StatusCodes.Status404NotFound);
            }

            return NoContent();
        }

        [HttpPost("upload")]
        [ProducesResponseType(typeof(FileDto), StatusCodes.Status201Created)]
        public IActionResult Upload() 
        {
            var newFile = new FileDto
            {
                Id = 56,
                FileName = "new-file"
            };

            
            return CreatedAtAction(
                    nameof(GetById),
                    new {id = newFile.Id},
                    newFile);
            
        }

        [HttpGet("download/{fileName}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Download([FromRoute] string? fileName)
        {
            if (fileName == "sample")
            {
                byte[] content = System.Text.Encoding.UTF8.GetBytes("Hello World");
                var file = File(
                            content,
                            "text/plain",
                            "sample.txt");
                return file;
            }

            return Problem(
                title: "file not found",
                detail: "there was no such file in the database",
                statusCode: StatusCodes.Status404NotFound);
            
        }

        [HttpGet("download/local/{fileName}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult DownloadLocal([FromRoute] string? fileName)
        {

            if (fileName == "sample")
            {
                var path = Path.Combine(
                Directory.GetCurrentDirectory(),
                "storage",
                "sample.pdf");

                if (!System.IO.File.Exists(path))
                {
                    return Problem(
                        title: "file doesnt exist",
                        detail: "the file doesnt exist anymore in the storage",
                        statusCode: StatusCodes.Status404NotFound);
                }

                var stream = System.IO.File.OpenRead(path);
                var file = File(
                    stream,
                    "application/pdf",
                    "sample.pdf");

                return file;
            }

            return BadRequest();

        }
    }
}
