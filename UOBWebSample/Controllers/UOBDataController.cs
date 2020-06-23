using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using UOL.Models;

namespace UOBWebSample.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UOBDataController : ControllerBase
    {
        [HttpGet("{manufacturerGLN}/{productCode}")]
        public async Task<IActionResult> GetAsync(string manufacturerGLN, string productCode)
        {
            var authenticationData = new AuthenticationManager().GetAccessToken();

            if (authenticationData == null || string.IsNullOrEmpty(authenticationData.AccessToken))
            {
                return Unauthorized();
            }

            using var client = WebClientManager.GetWebHttpClient("Data", authenticationData.AccessToken);
            try
            {
                var url = HttpUtility.UrlEncode($"productdata/{manufacturerGLN}/{productCode}");

                using var response = await client.GetAsync(url).ConfigureAwait(false);
                var contentString = string.Empty;
                if (response.IsSuccessStatusCode)
                {
                    contentString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                    var result = JsonConvert.DeserializeObject<CADProduct>(contentString);

                    return new JsonResult(result);
                }
                else if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    return default;
                }
                else
                {
                    contentString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                }

                return new ContentResult() { Content = contentString, ContentType = "text/plain", StatusCode = 200 };
            }
            catch
            {
                throw;
            }
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync([FromBody] IEnumerable<UOL.Models.ManufacturerGLNandProductCode> manufacturerGLNandProductCodes)
        {
            var authenticationData = new AuthenticationManager().GetAccessToken();

            if (authenticationData == null || string.IsNullOrEmpty(authenticationData.AccessToken))
            {
                return Unauthorized();
            }

            using var client = WebClientManager.GetWebHttpClient("Data", authenticationData.AccessToken);
            try
            {
                var url = HttpUtility.UrlEncode($"productdata");
                var content = new StringContent(JsonConvert.SerializeObject(manufacturerGLNandProductCodes), Encoding.UTF8, "application/json");

                using var response = await client.PostAsync(url, content).ConfigureAwait(false);
                var contentString = string.Empty;
                if (response.IsSuccessStatusCode)
                {
                    contentString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                    var result = JsonConvert.DeserializeObject<IEnumerable<CADProduct>>(contentString);

                    return new JsonResult(result);
                }
                else if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    return default;
                }
                else
                {
                    contentString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                }

                return new ContentResult() { Content = contentString, ContentType = "text/plain", StatusCode = 200 };
            }
            catch
            {
                throw;
            }
        }
    }
}