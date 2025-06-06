using CalamityStatsTracker;
using CommandSystem;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Arguments.ServerEvents;
using LabApi.Events.Handlers;
using LabApi.Loader.Features.Plugins;
using MEC;
using System.Xml.Serialization;

namespace CalamityStatsTracker
{
    [CommandHandler(typeof(ClientCommandHandler))]
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class CalamityStatsTracker : Plugin<PluginConfig>
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
            //sub round ending
            CL.Info("Calamity Stats Tracker Enabled");
        }

        public override void Disable()
        {
            Instance = null;
            ServerEvents.RoundStarted -= OnRoundStarted; // unsub round start
            //unsub round end
            CL.Info("Calamity Stats Tracker Disabled");
        }

        void OnRoundStarted(RoundStartingEventArgs ev) // make on round start arg
        {
            //clear previous stats from last round or etc
        }

        //make on round end arg
        //call the saving of stats function
    }
}


