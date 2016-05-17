namespace Rocket.Chat.Net.Interfaces
{
    using System;

    using Rocket.Chat.Net.Interfaces.Driver;

    public interface IRocketChatDriver : IDisposable,
                                         IRocketClient,
                                         IRocketUserManagement,
                                         IRocketMessaging,
                                         IRocketRoomManagement,
                                         IRocketAdministrativeManagement
    {
    }
}