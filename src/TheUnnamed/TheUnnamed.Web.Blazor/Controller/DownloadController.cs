using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Http.Headers;
using TheUnnamed.Core.Storage;

namespace TheUnnamed.Web.Blazor.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class DownloadController : ControllerBase
    {
        private readonly IDocumentStorage _storage;

        public DownloadController(IDocumentStorage storage)
        {
            _storage = storage ?? throw new ArgumentNullException(nameof(storage));
        }

        [HttpGet("{uuid}/{filename}")]
        public async Task<ActionResult> Generate(Guid uuid, string filename)
        {
            var docInfo = await _storage.GetDocument(uuid);

            return File(docInfo.Stream, docInfo.ContentType, filename);
        }
    }
}
