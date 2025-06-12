using CalamityStatsTracker;
using CommandSystem;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Arguments.ServerEvents;
using LabApi.Events.Handlers;
using LabApi.Loader.Features.Plugins;
using LabApi;
using MEC;
using System.Xml.Serialization;

namespace CalamityNPCs
{
    [CommandHandler(typeof(ClientCommandHandler))]
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class CalamityNPCs : LabApi.Loader.Features.Plugins.Plugin<PluginConfig>
    {
        public override string Author => "Fallout001";
        public override string Name => "CalamityNPCs";
        public override string Description => "CalamityNPCs";
        public override Version Version => new Version(0, 0, 1);
        public override Version RequiredApiVersion => LabApi.Features.LabApiProperties.CurrentVersion;

        public static CalamityNPCs Instance;

        public override void Enable()
        {
            Instance = this;
            ServerEvents.RoundStarted += OnRoundStarted; 
            ServerEvents.RoundEnding += OnRoundEnding; 
            CL.Info("Calamity NPC's Enabled");
        }

        public override void Disable()
        {
            Instance = null;
            ServerEvents.RoundStarted -= OnRoundStarted; 
            ServerEvents.RoundEnding -= OnRoundEnding;
            CL.Info("Calamity NPC's Disabled");
        }

        void OnRoundStarted()
        {
        }

        void OnRoundEnding(RoundEndingEventArgs ev)
        {
        }
    }
}


