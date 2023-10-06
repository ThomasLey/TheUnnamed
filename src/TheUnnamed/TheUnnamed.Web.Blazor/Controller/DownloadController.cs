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

        [HttpGet("{uuid}")]
        public async Task<HttpResponseMessage> Generate(Guid uuid)
        {
            var docInfo = await _storage.GetDocument(uuid);

            var result = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ByteArrayContent(docInfo.Stream)
            };
            result.Content.Headers.ContentDisposition =
                new ContentDispositionHeaderValue("attachment")
                {
                    FileName = docInfo.Filename
                };
            result.Content.Headers.ContentType =
                new MediaTypeHeaderValue(docInfo.ContentType);

            return result;
        }
    }
}
