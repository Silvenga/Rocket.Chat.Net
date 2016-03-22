namespace Rocket.Chat.Net.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;

    using Newtonsoft.Json.Linq;

    using Rocket.Chat.Net.Models;
    using Rocket.Chat.Net.Models.Results;

    public static class DriverHelper
    {
        public const string Sha256 = "sha-256";

        public static bool HasProperty(dynamic data, string name)
        {
            return data[name] != null;
        }

        public static string Sha256Hash(string value)
        {
            var builder = new StringBuilder();
            var encoding = Encoding.UTF8;

            using (var hash = SHA256.Create())
            {
                var result = hash.ComputeHash(encoding.GetBytes(value));
                foreach (var b in result)
                {
                    builder.Append(b.ToString("x2"));
                }
            }

            return builder.ToString();
        }
        
        public static RocketMessage ParseMessage(dynamic data)
        {
            var message = ((JObject) data).ToObject<RocketMessage>();
            return message;
        }
        
        public static bool HasError(dynamic data)
        {
            return data?.error != null;
        }

        public static ErrorData ParseError(dynamic data)
        {
            if (data == null || data.error == null)
            {
                return null;
            }

            var error = data.error;
            var result = new ErrorData
            {
                Error = error.error,
                Reason = error.reason,
                Message = error.message,
                ErrorType = error.errorType
            };

            return result;
        }
    }
}