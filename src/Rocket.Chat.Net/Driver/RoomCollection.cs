namespace Rocket.Chat.Net.Driver
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    using Rocket.Chat.Net.Collections;
    using Rocket.Chat.Net.Models;

    public class RoomCollection : IEnumerable<RoomWithInfo>
    {
        private readonly TypedStreamCollection<Room> _roomCollection;
        private readonly TypedStreamCollection<RoomInfo> _roomInfoCollection;

        public RoomCollection(TypedStreamCollection<Room> roomCollection, TypedStreamCollection<RoomInfo> roomInfoCollection)
        {
            _roomCollection = roomCollection;
            _roomInfoCollection = roomInfoCollection;
        }

        public IEnumerator<RoomWithInfo> GetEnumerator()
        {
            var roomItems = _roomCollection?.Items() ?? Enumerable.Empty<KeyValuePair<string, Room>>();
            var infoItems = _roomInfoCollection?.Items() ?? Enumerable.Empty<KeyValuePair<string, RoomInfo>>();

            return (from roomPair in roomItems
                    join infoPair in infoItems on roomPair.Value.RoomId equals infoPair.Key
                    let room = roomPair.Value
                    let info = infoPair.Value
                    select new RoomWithInfo
                    {
                        Type = room.Type,
                        Id = room.RoomId,
                        Name = room.Name,
                        Timestamp = room.Timestamp,
                        IsAlert = room.IsAlert,
                        IsOpen = room.IsOpen,
                        LastMessage = info.LastMessage,
                        LastSeen = room.LastSeen,
                        MessageCount = info.MessageCount,
                        Owner = info.Owner,
                        UnreadCount = room.UnreadCount,
                        Usersnames = info.Usersnames
                    })
                .GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}