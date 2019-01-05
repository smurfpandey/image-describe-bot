using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ReportAPI.Models;
using ReportAPI.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ReportAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ImagesController : Controller
    {
        private readonly ImageService _imageService;

        public ImagesController(ImageService imageService)
        {
            _imageService = imageService;
        }
        // GET: /images
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "image1", "value2" };
        }

        // GET /images/5
        [HttpGet("{id:length(24)}", Name = "GetImage")]
        public IActionResult Get(string id)
        {
            var image = _imageService.Get(id);

            if (image == null)
            {
                return NotFound();
            }

            return Ok(image);
        }

        // POST /images
        [HttpPost]
        public IActionResult Post(Image image)
        {
            _imageService.Create(image);
            return CreatedAtRoute("GetImage", new { id = image.Id.ToString() }, image);
        }

        // PUT /images/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE /images/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
