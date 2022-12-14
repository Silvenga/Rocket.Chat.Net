using System;
using System.Collections.Generic;
using System.Text;

namespace Rocket.Chat.Net.Models.RestResults
{
    public class RestLoginResult 
    {
        
        public string UserId { get; set; }

        public string AuthToken { get; set; }

        public User Me { get; set; }

    }
}
