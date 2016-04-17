namespace Rocket.Chat.Net.Models.MethodResults
{
    using System;
    using System.Collections.Generic;

    using Newtonsoft.Json;

    using Rocket.Chat.Net.Helpers;

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
        public int TotalUsers { get; set; }
        public int ActiveUsers { get; set; }
        public int NonActiveUsers { get; set; }
        public int OnlineUsers { get; set; }
        public int AwayUsers { get; set; }
        public int OfflineUsers { get; set; }
        public int TotalRooms { get; set; }
        public int TotalChannels { get; set; }
        public int TotalPrivateGroups { get; set; }
        public int TotalDirect { get; set; }
        public int TotalMessages { get; set; }
        public int MaxRoomUsers { get; set; }
        public int AvgChannelUsers { get; set; }
        public int AvgPrivateGroupUsers { get; set; }

        [JsonConverter(typeof(MeteorDateConverter))]
        public DateTime? LastLogin { get; set; }

        [JsonConverter(typeof(MeteorDateConverter))]
        public DateTime? LastMessageSentAt { get; set; }

        [JsonConverter(typeof(MeteorDateConverter))]
        public DateTime? LastSeenSubscription { get; set; }

        public Os Os { get; set; }
        public Process Process { get; set; }
        public Migration Migration { get; set; }
        public int InstanceCount { get; set; }
    }

    public class Migration
    {
        [JsonProperty(PropertyName = "_id")]
        public string Id { get; set; }

        public int Version { get; set; }
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
        public int TotalMemory { get; set; }

        [JsonProperty(PropertyName = "Freemem")]
        public int FreeMemory { get; set; }

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
        public int User { get; set; }
        public int Nice { get; set; }
        public int Sys { get; set; }
        public long Idle { get; set; }
        public int Irq { get; set; }
    }

    public class Cpu
    {
        public string Model { get; set; }
        public int Speed { get; set; }
        public Times Times { get; set; }
    }
}