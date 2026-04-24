using Microsoft.AspNetCore.Mvc;

namespace AniTyan.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AnimeGirlsController : Controller
    {
        [HttpPost]
        public async Task<IActionResult> CreateAnimeGirl(IFormFile cardFile)
        {
            if (cardFile == null || cardFile.Length == 0)
                return BadRequest("Файл не загружен");

            if (cardFile.ContentType != "image/png")
                return BadRequest("Требуется PNG-файл (Koikatsu Card)");

            using var memoryStream = new MemoryStream();
            await cardFile.CopyToAsync(memoryStream);
            var fileBytes = memoryStream.ToArray();

            return File(fileBytes, "image/png");
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(new { message = "Get list of all anime girls" });
        }

        [HttpGet("{id}")]
        public IActionResult GetById(string id)
        {
            return Ok(new { message = "Get anime girl info by id"});
        }

        [HttpGet("{id}/card-image")]
        public IActionResult GetCardImage(string id)
        {
            return Ok(new { message = "Get PNG of anime girl"});
        }

        [HttpPost("{id}/3d-model")]
        public IActionResult AttachThreeDModel(string id)
        {
            return Ok(new { message = "Attach 3D-model" });
        }

        [HttpGet("{id}/3d-model")]
        public IActionResult GetThreeDModel(string id)
        {
            return Ok(new { message = "Get 3D of anime girl" });
        }

        [HttpGet("{id}/chat")]
        public IActionResult Chat(string id)
        {
            return Ok(new { message = "Chat anime girl" });
        }
    }
}
