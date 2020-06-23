using System;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using UOBWebSample.Models;

namespace UOBWebSample
{
    internal class AuthenticationManager
    {
        public AuthenticationData GetAccessToken()
        {
            var contentstring = $"grant_type=password&username={Startup.StaticConfig["UOB_Username"]}&password={Startup.StaticConfig["UOB_Password"]}&client_id={Startup.StaticConfig["ClientId"]}&client_secret={Startup.StaticConfig["ClientSecret"]}&scope=unifeed openid offline_access apix";
            var content = new ASCIIEncoding().GetBytes(contentstring);
            var request = (HttpWebRequest)WebRequest.Create($"{Startup.StaticConfig["AuthorizationServerUrl"]}/OAuth/Token");
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = content.Length;
            try
            {
                HttpWebResponse response;
                using (var stream = request.GetRequestStream())
                {
                    stream.Write(content, 0, content.Length);
                    response = (HttpWebResponse)request.GetResponse();
                }

                using (var stream = response.GetResponseStream())
                using (var reader = JsonReaderWriterFactory.CreateJsonReader(stream, new XmlDictionaryReaderQuotas()))
                {
                    var root = XElement.Load(reader);
                    var accessToken = root.XPathSelectElement("//access_token");
                    var expiresIn = root.XPathSelectElement("//expires_in");
                    var refreshToken = root.XPathSelectElement("//refresh_token");
                    var tokenType = root.XPathSelectElement("//token_type");

                    return new AuthenticationData
                    {
                        AccessToken = accessToken?.Value,
                        ExpiresIn = expiresIn == null ? 0 : long.Parse(expiresIn.Value),
                        RefreshToken = refreshToken?.Value,
                        Retrieved = DateTime.UtcNow,
                        TokenType = tokenType?.Value
                    };
                }
            }
            catch
            {
                return null;
            }
        }
    }
}
