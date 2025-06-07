using CalamityStatsTracker;
using CommandSystem;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Arguments.ServerEvents;
using LabApi.Events.Handlers;
using LabApi.Loader.Features.Plugins;
using LabApi;
using MEC;
using System.Xml.Serialization;

namespace CalamityStatsTracker
{
    [CommandHandler(typeof(ClientCommandHandler))]
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class CalamityStatsTracker : LabApi.Loader.Features.Plugins.Plugin<PluginConfig>
    {
        public override string Author => "Fallout001";
        public override string Name => "CalamityStatsTracker";
        public override string Description => "CalamityStatsTracker";
        public override Version Version => new Version(0, 0, 1);
        public override Version RequiredApiVersion => LabApi.Features.LabApiProperties.CurrentVersion;

        public static CalamityStatsTracker Instance;

        public override void Enable()
        {
            Instance = this;
            ServerEvents.RoundStarted += OnRoundStarted; 
            ServerEvents.RoundEnding += OnRoundEnding; 
            CL.Info("Calamity Stats Tracker Enabled");
        }

        public override void Disable()
        {
            Instance = null;
            ServerEvents.RoundStarted -= OnRoundStarted; 
            ServerEvents.RoundEnding -= OnRoundEnding;
            CL.Info("Calamity Stats Tracker Disabled");
        }

        void OnRoundStarted()
        {
            RoundStatsTracker.CurrentRoundStats.Clear(); // Clear stats for new round
            RoundStatsTracker.roundStartTime = DateTime.Now;

            RoundStatsTracker.AddStatEvent("CalamityStatsTracker", "Round", "RoundStarted","Round has started");

        }

        void OnRoundEnding(RoundEndingEventArgs ev)
        {
            RoundStatsTracker.AddStatEvent("CalamityStatsTracker", "Round", "RoundStarted", "Round has started");

            RoundStatsTracker.SaveCurrentRoundStats(); // Save stats when round ends
        }
    }
}


