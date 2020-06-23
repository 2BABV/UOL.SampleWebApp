using System;

namespace UOBWebSample.Models
{
    internal class AuthenticationData
    {
        public string AccessToken { get; set; }

        public long ExpiresIn { get; set; }

        public string RefreshToken { get; set; }

        public DateTime Retrieved { get; set; }

        public string TokenType { get; set; }
    }
}
