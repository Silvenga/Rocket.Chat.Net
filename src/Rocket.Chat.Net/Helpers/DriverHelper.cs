namespace Rocket.Chat.Net.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;

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

        public static DateTime FromEpoch(long unixTime)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return epoch.AddMilliseconds(unixTime);
        }

        public static long ToEpoch(DateTime date)
        {
            var span = date.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return Convert.ToInt64(span.TotalMilliseconds);
        }

        public static User ParseUser(dynamic data)
        {
            if (data == null)
            {
                return null;
            }

            return new User
            {
                Id = data["_id"],
                Username = data["username"]
            };
        }

        public static IEnumerable<User> ParseMentions(dynamic data)
        {
            if (data == null)
            {
                yield break;
            }

            foreach (var user in data)
            {
                yield return ParseUser(user);
            }
        }

        private static IEnumerable<User> ParseStarred(dynamic data)
        {
            if (data == null)
            {
                yield break;
            }

            foreach (var user in data)
            {
                yield return new User
                {
                    Id = user["_id"]
                };
            }
        }

        public static RocketMessage ParseMessage(dynamic data)
        {
            var message = new RocketMessage
            {
                Id = data["_id"],
                Type = data.t, // Assuming this is what this means.
                RoomId = data.rid,
                Message = data.msg.ToString().Trim(),
                IsBot = data.bot != null && data.bot == true,
                CreatedOn = DriverHelper.ParseDateTime(data.ts),
                CreatedBy = DriverHelper.ParseUser(data.u),
                EditedOn = DriverHelper.ParseDateTime(data.editedAt),
                EditedBy = DriverHelper.ParseUser(data.editedBy),
                Mentions = Enumerable.ToList(DriverHelper.ParseMentions(data.mentions)),
                Starred = Enumerable.ToList(DriverHelper.ParseStarred(data.starred)),
            };

            return message;
        }

        public static DateTime? ParseDateTime(dynamic data)
        {
            if (data == null)
            {
                return null;
            }

            var rawValue = data["$date"].ToString(); // TODO Figure out why I can't just use the value as a long
            var epoch = long.Parse(rawValue);
            return DriverHelper.FromEpoch(epoch);
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