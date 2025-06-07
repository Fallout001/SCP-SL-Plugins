using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CalamityStatsTracker
{
    internal class BaseStatEvent
    {
        public string PluginName { get; set; } // name of plugin that sent the event
        public string EventType { get; set; } // the type of event, e.g., "Vault", "Coin"
        public string EventName { get; set; } // CoinAction - "blah" , VaultOpen, etc.
        public DateTime Timestamp { get; set; } // Time at which it happened compared to real time
        public int RoundTime { get; set; } // Time at which it happened compared to round time (in seconds)
        public string ExtraData { get; set; } // such as a result of the event or additional information
    }
}
