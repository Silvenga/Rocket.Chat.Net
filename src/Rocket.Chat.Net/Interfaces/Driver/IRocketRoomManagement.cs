﻿namespace Rocket.Chat.Net.Interfaces.Driver
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Rocket.Chat.Net.Collections;
    using Rocket.Chat.Net.Models;
    using Rocket.Chat.Net.Models.MethodResults;

    public interface IRocketRoomManagement
    {
        /// <summary>
        /// Subscribe to messages from given room
        /// </summary>
        /// <param name="roomId">The room to listen to. Null will listen to all authorized rooms. 
        /// This may or may not be correct. </param>
        /// <returns></returns>
        Task SubscribeToRoomAsync(string roomId = null);

        /// <summary>
        /// Get roomId by either roomId or room name
        /// </summary>
        /// <param name="roomIdOrName">Room name or roomId</param>
        /// <returns></returns>
        Task<MethodResult<string>> GetRoomIdAsync(string roomIdOrName);

        /// <summary>
        /// Joins a room, no effect if already joined
        /// </summary>
        /// <param name="roomId">The room to join</param>
        /// <returns></returns>
        Task<MethodResult> JoinRoomAsync(string roomId);

        /// <summary>
        /// List authorized channels
        /// </summary>
        /// <returns></returns>
        Task<MethodResult<ChannelListResult>> ChannelListAsync();

        /// <summary>
        /// Creates a new room.
        /// </summary>
        /// <param name="roomName">Name of the room to create</param>
        /// <param name="members">Optional. A list of users to add to the room on creation</param>
        /// <returns>The id of the room that was created when successful</returns>
        Task<MethodResult<CreateRoomResult>> CreateChannelAsync(string roomName, IList<string> members = null);

        /// <summary>
        /// Hide a room.
        /// </summary>
        /// <param name="roomId">Room to hide by id</param>
        /// <returns></returns>
        Task<MethodResult<CreateRoomResult>> HideRoomAsync(string roomId);

        /// <summary>
        /// Deletes a room. This requires permissions (admin for now?). 
        /// </summary>
        /// <param name="roomId">Room to delete by id</param>
        /// <returns>Number impacted?</returns>
        Task<MethodResult<bool>> EraseRoomAsync(string roomId);

        /// <summary>
        /// Creates a new private group.
        /// </summary>
        /// <param name="groupName">Name of the group to create</param>
        /// <param name="members">Optional. A list of users to add to the room on creation</param>
        /// <returns>The id of the room that was created when successful</returns>
        Task<MethodResult<CreateRoomResult>> CreateGroupAsync(string groupName, IList<string> members = null);

        /// <summary>
        /// Subscribe to room listings.
        /// </summary>
        /// <returns></returns>
        Task SubscribeToRoomListAsync();

        /// <summary>
        /// Get a list of currently known rooms from the room list subscription.
        /// SubscribeToRoomListAsync() should be called once before using this method.
        /// </summary>
        /// <returns></returns>
        [Obsolete("Use the property Rooms instead. This will be removed in version 2.")]
        IEnumerable<Room> GetRooms();

        /// <summary>
        /// Get a list of currently known rooms from the room list subscription.
        /// SubscribeToRoomListAsync() should be called once before using this method.
        /// </summary>
        /// <returns></returns>
        TypedStreamCollection<Room> GetRoomsCollection();

        [Obsolete("Use the property Rooms instead. This will be removed in version 2.")]
        TypedStreamCollection<RoomInfo> GetRoomInfoCollection();

        /// <summary>
        /// This is the method call used to get all the rooms a user belongs to.  
        /// </summary>
        /// <returns></returns>
        Task<MethodResult<IEnumerable<RoomInfo>>> GetAvailableRoomInfoCollection();

        /// <summary>
        /// Subscribe to information about a single room.
        /// </summary>
        /// <param name="roomName"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        Task SubscribeToRoomInformationAsync(string roomName, RoomType type);

        /// <summary>
        /// Get the list of known rooms.
        /// SubscribeToRoomListAsync() should be called once before using this property.
        /// </summary>
        RoomCollection Rooms { get; }
    }
}