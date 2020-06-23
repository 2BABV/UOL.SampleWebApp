using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using UOL.Models;

namespace UOBWebSample.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UOBTemplateController : ControllerBase
    {
        [HttpGet("{manufacturerGLN}/{productCode}/{application}/{applicationVersion}")]
        public async Task<IActionResult> GetAsync(string manufacturerGLN, string productCode, string application, string applicationVersion)
        {
            var uobDataController = new UOBDataController();

            var response = await uobDataController.GetAsync(manufacturerGLN, productCode);

            var product = ((JsonResult)response).Value;

            return await GetTemplates(application, applicationVersion, new List<CADProduct> { (CADProduct)product });
        }

        [HttpPost("{application}/{applicationVersion}")]
        public async Task<IActionResult> PostAsync([FromRoute] string application,[FromRoute] string applicationVersion, [FromBody] IEnumerable<UOL.Models.ManufacturerGLNandProductCode> manufacturerGLNandProductCodes)
        {
            var uobDataController = new UOBDataController();

            var response = await uobDataController.PostAsync(manufacturerGLNandProductCodes);

            var products = ((JsonResult)response).Value;

            return await GetTemplates(application, applicationVersion, (List<CADProduct>)products);
        }

        private async Task<IActionResult> GetTemplates(string application, string applicationVersion, List<CADProduct> cadProducts)
        {
            var authenticationData = new AuthenticationManager().GetAccessToken();

            if (authenticationData == null || string.IsNullOrEmpty(authenticationData.AccessToken))
            {
                return Unauthorized();
            }

            using var client = WebClientManager.GetWebHttpClient("Template", authenticationData.AccessToken);
            try
            {
                var url = $"export/{application}/{applicationVersion}";
                var content = new StringContent(JsonConvert.SerializeObject(cadProducts), Encoding.UTF8, "application/json");

                using var response = await client.PostAsync(url, content).ConfigureAwait(false);
                var contentString = string.Empty;
                if (response.IsSuccessStatusCode)
                {
                    using var contentStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
                    // When the HttpClient is disposed, the contentStream is no longer readable, seekable, etc. To
                    // fix this, we clone the contentStream into a copy that is returned instead of the contentStream.
                    var contentStreamClone = new MemoryStream();

                    await contentStream.CopyToAsync(contentStreamClone).ConfigureAwait(false);

                    contentStreamClone.Seek(0, SeekOrigin.Begin);
                    return new FileStreamResult(contentStreamClone, "application/octet-stream");
                }
                else if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    return default;
                }

                return null;
            }
            catch
            {
                throw;
            }
        }
    }
}