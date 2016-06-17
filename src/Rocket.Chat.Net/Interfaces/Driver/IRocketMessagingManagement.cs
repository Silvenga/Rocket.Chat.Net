namespace Rocket.Chat.Net.Interfaces.Driver
{
    using System;
    using System.Threading.Tasks;

    using Rocket.Chat.Net.Models;
    using Rocket.Chat.Net.Models.MethodResults;

    public interface IRocketMessagingManagement
    {
        event MessageReceived MessageReceived;

        /// <summary>
        /// Send a message to a room
        /// </summary>
        /// <param name="text">Text of the message</param>
        /// <param name="roomId">The room to send to</param>
        /// <returns></returns>
        Task<MethodResult<RocketMessage>> SendMessageAsync(string text, string roomId);

        /// <summary>
        /// Edit a message by messageId
        /// </summary>
        /// <param name="messageId">Message to update</param>
        /// <param name="roomId">Room that the message exists in</param>
        /// <param name="newMessage">Update the message to this</param>
        /// <returns></returns>
        Task<MethodResult> UpdateMessageAsync(string messageId, string roomId, string newMessage);

        /// <summary>
        /// Load the message history of a room ordered by timestamp newest first
        /// </summary>
        /// <param name="roomId">Room to list history of</param>
        /// <param name="end">No idea, something with timespan</param>
        /// <param name="limit">Max number of messages to load</param>
        /// <param name="ls">No idea, something with timespan, maybe less than?</param>
        /// <returns></returns>
        Task<MethodResult<LoadMessagesResult>> LoadMessagesAsync(string roomId, DateTime? end = null, int? limit = 20,
                                                                 string ls = null);

        /// <summary>
        /// Delete message
        /// </summary>
        /// <param name="messageId">The message to delete</param>
        /// <param name="roomId">The room where that message sits</param>
        /// <returns></returns>
        Task<MethodResult> DeleteMessageAsync(string messageId, string roomId);

        /// <summary>
        /// Create a private message room with target user
        /// </summary>
        /// <param name="username">The user to create a private message room for</param>
        /// <returns>The private RoomId</returns>
        Task<MethodResult<CreateRoomResult>> CreatePrivateMessageAsync(string username);

        /// <summary>
        /// Searches the messages for the given room.
        /// </summary>
        /// <param name="query">Query to search for (can use 'from:', 'mention:', etc.)</param>
        /// <param name="roomId">RoomId to search for</param>
        /// <param name="limit">Limit the number of messages (default 100)</param>
        /// <returns></returns>
        Task<MethodResult<LoadMessagesResult>> SearchMessagesAsync(string query, string roomId, int limit = 100);

        /// <summary>
        /// Set a reaction (e.g. ":grinning:") on a message. Set an existing reaction to remove it.
        /// </summary>
        /// <param name="reaction"></param>
        /// <param name="messageId"></param>
        /// <returns></returns>
        Task<MethodResult> SetReactionAsync(string reaction, string messageId);

        /// <summary>
        /// Pin message by message id. 
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        Task<MethodResult<RocketMessage>> PinMessageAsync(RocketMessage message);

        /// <summary>
        /// Unpin message by message id. 
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        Task<MethodResult> UnpinMessageAsync(RocketMessage message);

        /// <summary>
        /// Send a message with an attachment. 
        /// NOTE: Don't know the longevity of this as the UI cannot create this manually
        /// TODO
        /// </summary>
        /// <param name="attachment">Attachment to include</param>
        /// <param name="roomId">Room to send to</param>
        /// <returns></returns>
        Task<MethodResult<RocketMessage>> SendCustomMessageAsync(Attachment attachment, string roomId);
    }
}