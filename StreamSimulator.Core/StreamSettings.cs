using System.Collections.Generic;

namespace StreamSimulator.Core
{
    public class StreamSettings
    {
        public string MessagePattern { get; set; } = @"\d{1,3}\.\d{2}";
        public int ListeningPort { get; set; } = 1234;
        public List<string> Symbols { get; set; } = new List<string>
        {
            "DemoSymbol01","DemoSymbol02","DemoSymbol03","DemoSymbol04","DemoSymbol05",
            "DemoSymbol06","DemoSymbol07","DemoSymbol08","DemoSymbol09","DemoSymbol10",
        };

        public bool SendHeartbeat { get; set; } = false;
        public string HeartbeatPattern { get; set; } = @"HB";
        public int HeartbeatIntervalMs { get; set; } = 5000;
        public bool SendReplyCommands { get; set; } = false;
        public Dictionary<string, string> ReplyCommands { get; set; } = new Dictionary<string, string>
        {
            { "ping", "pong" }
        };
        public bool AllowCustomMessageInjection { get; set; } = false;
        public int SimultaniousMessageCount { get; set; } = 1;
        public double MessagesPerSecond { get; set; } = 1;
        public bool AllowSubscribe { get; set; } = true;
        public string SubscribeCommand { get; set; } = "subscribe,sub";
        public bool AllowUnsubscribe { get; set; } = true;
        public string UnsubscribeCommand { get; set; } = "unsubscribe,unsub";
    }
}
