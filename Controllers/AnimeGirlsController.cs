using AniTyan.Models.Services.AniTyanService;
using AniTyan.Models.Services.KoikatsuCardService;
using Microsoft.AspNetCore.Mvc;
using Minio;

namespace AniTyan.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AnimeGirlsController : Controller
    {
        private readonly AnimeGirlMaker _animeGirlMaker;

        public AnimeGirlsController(AnimeGirlMaker animeGirlMaker)
        {
            _animeGirlMaker = animeGirlMaker;
        }

        [HttpPost]
        public async Task<IActionResult> CreateAnimeGirl(IFormFile cardFile,
            [FromServices] IKoikatsuCardService koikatsuCardService)
        {
            if (cardFile == null || cardFile.Length == 0)
                return BadRequest("Файл не загружен");

            if (cardFile.ContentType != "image/png")
                return BadRequest("Требуется PNG-файл (Koikatsu Card)");

            bool isValid = await koikatsuCardService.IsValidCardAsync(cardFile);
            if (!isValid)
                return BadRequest("Загруженный файл не является валидной картой Koikatsu");

            /*
             * Создание anime-girl, вызов AnimeGirlMaker
             */
            await _animeGirlMaker.MakeAnimeGirl(cardFile);

            return Ok(new { message = "Create anime girl"});
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(new { message = "Get list of all anime girls"});
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
