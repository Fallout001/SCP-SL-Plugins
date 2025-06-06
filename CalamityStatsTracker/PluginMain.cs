using CalamityStatsTracker;
using CommandSystem;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Arguments.ServerEvents;
using LabApi.Events.Handlers;
using LabApi.Loader.Features.Plugins;
using LabApi;
using MEC;
using System.Xml.Serialization;
using EffectOnHUD;

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
            //clear previous stats from last round or etc
            CL.Error("Round Started ARG");
            RoundStatsTracker.AddStatEvent("CalamityStatsTracker", "Round", "RoundStarted","Round has started");
            RoundStatsTracker.AddStatEvent("CalamityStatsTracker", "Round", "RoundStarted","Round has started");
        }

        void OnRoundEnding(RoundEndingEventArgs ev) // Added handler for round ending
        {
            CL.Error("Round Ended  ARG");
            var roundEndEvent = new BaseStatEvent
            {
                PluginName = "CalamityStatsTracker",
                EventType = "Round",
                EventName = "RoundEnded",
                Timestamp = DateTime.Now,
                RoundTime = DateTime.Now.Second - RoundStatsTracker.roundStartTime, // Assuming RoundDuration is in seconds
                ExtraData = $"Winning Team: {ev.LeadingTeam}"
            };
            CL.Error("Round Ending and attempt add");
            RoundStatsTracker.CurrentRoundStats.Add(roundEndEvent);

            RoundStatsTracker.SaveCurrentRoundStats(); // Save stats when round ends
        }
    }
}


