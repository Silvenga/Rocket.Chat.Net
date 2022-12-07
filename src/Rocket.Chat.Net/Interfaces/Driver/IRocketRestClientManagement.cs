using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Rocket.Chat.Net.Interfaces.Driver
{
    public interface IRocketRestClientManagement
    {
        Task LoginRestApi(object loginArgs);
    }
}
