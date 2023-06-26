using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Newtonsoft.Json;
using Rocket.Chat.Net.Models.RestResults;
using Rocket.Chat.Net.JsonConverters;
using Newtonsoft.Json.Converters;

namespace Rocket.Chat.Net.Tests.Models
{

    [Trait("Category", "Models")]
    public class RestResultConverterFacts
    {
        [Fact]
        public void TestSerialization()
        {
            var successResponse = "{\r\n    \"status\": \"success\",\r\n    \"data\": {\r\n        \"userId\": \"kXZuDqZ4L4NFSJ6Mk\",\r\n        \"authToken\": \"zlds-SmP9Af0gzMOn7jjT_ec_Kiy3nuxXdpHmGLFFWW\",\r\n        \"me\": {\r\n            \"_id\": \"kXZuDqZ4L4NFSJ6Mk\",\r\n            \"services\": {\r\n                \"password\": {\r\n                    \"bcrypt\": \"$2b$10$7Ix/tzU4JQmcyszlBiahIemK91H7kJuQ2Jpkh7ltY4WCMPggr2b4W\"\r\n                }\r\n            },\r\n            \"username\": \"theotest\",\r\n            \"emails\": [\r\n                {\r\n                    \"address\": \"test@softbauware.de\",\r\n                    \"verified\": true\r\n                }\r\n            ],\r\n            \"status\": \"offline\",\r\n            \"active\": true,\r\n            \"_updatedAt\": \"2023-01-18T19:09:04.474Z\",\r\n            \"roles\": [],\r\n            \"name\": \"Theo Test\",\r\n            \"requirePasswordChange\": false,\r\n            \"settings\": {\r\n                \"preferences\": {\r\n                    \"enableAutoAway\": true,\r\n                    \"idleTimeLimit\": 300,\r\n                    \"desktopNotificationRequireInteraction\": false,\r\n                    \"desktopNotifications\": \"all\",\r\n                    \"pushNotifications\": \"all\",\r\n                    \"unreadAlert\": true,\r\n                    \"useEmojis\": true,\r\n                    \"convertAsciiEmoji\": true,\r\n                    \"autoImageLoad\": true,\r\n                    \"saveMobileBandwidth\": true,\r\n                    \"collapseMediaByDefault\": false,\r\n                    \"hideUsernames\": false,\r\n                    \"hideRoles\": false,\r\n                    \"hideFlexTab\": false,\r\n                    \"displayAvatars\": true,\r\n                    \"sidebarGroupByType\": true,\r\n                    \"sidebarViewMode\": \"medium\",\r\n                    \"sidebarDisplayAvatar\": true,\r\n                    \"sidebarShowUnread\": false,\r\n                    \"sidebarSortby\": \"activity\",\r\n                    \"showMessageInMainThread\": false,\r\n                    \"sidebarShowFavorites\": true,\r\n                    \"sendOnEnter\": \"normal\",\r\n                    \"messageViewMode\": 0,\r\n                    \"emailNotificationMode\": \"mentions\",\r\n                    \"newRoomNotification\": \"door\",\r\n                    \"newMessageNotification\": \"chime\",\r\n                    \"muteFocusedConversations\": true,\r\n                    \"notificationsSoundVolume\": 100,\r\n                    \"enableMessageParserEarlyAdoption\": false\r\n                }\r\n            },\r\n            \"statusConnection\": \"offline\",\r\n            \"email\": \"test@softbauware.de\",\r\n            \"avatarUrl\": \"http://localhost:3000/avatar/theotest\"\r\n        }\r\n    }\r\n}";
            var result = JsonConvert.DeserializeObject<RestResult<RestLoginResult>>(successResponse);
            // var result = JsonConvert.DeserializeObject<RestResult>(successResponse);

            Assert.True(result.Success);
        }
    }
}
