namespace Rocket.Chat.Net.Models.MethodResults
{
    using System;
    using System.Collections.Generic;

    using Newtonsoft.Json;

    using Rocket.Chat.Net.JsonConverters;

    public class StatisticsResult
    {
        [JsonProperty(PropertyName = "_id")]
        public string Id { get; set; }

        public string UniqueId { get; set; }

        [JsonConverter(typeof(MeteorDateConverter))]
        public DateTime CreatedAt { get; set; }

        public string Version { get; set; }
        public object Tag { get; set; }
        public object Branch { get; set; }
        public long TotalUsers { get; set; }
        public long ActiveUsers { get; set; }
        public long NonActiveUsers { get; set; }
        public long OnlineUsers { get; set; }
        public long AwayUsers { get; set; }
        public long OfflineUsers { get; set; }
        public long TotalRooms { get; set; }
        public long TotalChannels { get; set; }
        public long TotalPrivateGroups { get; set; }
        public long TotalDirect { get; set; }
        public long TotalMessages { get; set; }
        public long MaxRoomUsers { get; set; }
        public long AvgChannelUsers { get; set; }
        public long AvgPrivateGroupUsers { get; set; }

        [JsonConverter(typeof(MeteorDateConverter))]
        public DateTime? LastLogin { get; set; }

        [JsonConverter(typeof(MeteorDateConverter))]
        public DateTime? LastMessageSentAt { get; set; }

        [JsonConverter(typeof(MeteorDateConverter))]
        public DateTime? LastSeenSubscription { get; set; }

        public Os Os { get; set; }
        public Process Process { get; set; }
        public Migration Migration { get; set; }
        public long InstanceCount { get; set; }
    }

    public class Migration
    {
        [JsonProperty(PropertyName = "_id")]
        public string Id { get; set; }

        public long Version { get; set; }
        public bool Locked { get; set; }

        [JsonConverter(typeof(MeteorDateConverter))]
        public DateTime LockedAt { get; set; }

        public string BuildAt { get; set; }
    }

    public class Os
    {
        public string Type { get; set; }
        public string Platform { get; set; }
        public string Arch { get; set; }
        public string Release { get; set; }
        public double Uptime { get; set; }
        public List<double> Loadavg { get; set; }

        [JsonProperty(PropertyName = "TotalMemory")]
        public long TotalMemory { get; set; }

        [JsonProperty(PropertyName = "Freemem")]
        public long FreeMemory { get; set; }

        public List<Cpu> Cpus { get; set; }
    }

    public class Process
    {
        public string NodeVersion { get; set; }
        public int Pid { get; set; }
        public int Uptime { get; set; }
    }

    public class Times
    {
        public long User { get; set; }
        public long Nice { get; set; }
        public long Sys { get; set; }
        public long Idle { get; set; }
        public long Irq { get; set; }
    }

    public class Cpu
    {
        public string Model { get; set; }
        public long Speed { get; set; }
        public Times Times { get; set; }
    }
}