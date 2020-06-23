using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace UOBWebSample
{
    internal class WebClientManager
  {
    internal static HttpClient GetWebHttpClient(string callType, string accessToken)
    {
      var client = new HttpClient
      {
        Timeout = new TimeSpan(0, 0, 90),
        BaseAddress = new Uri(callType.Equals("Data") ? Startup.StaticConfig["DataBaseUrl"] : Startup.StaticConfig["TemplateBaseUrl"])
      };

      client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

      return client;
    }
  }
}
