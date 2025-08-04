using CommandSystem;
using LabApi;
using LabApi.Events;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Arguments.ServerEvents;
using LabApi.Events.Handlers;
using LabApi.Loader.Features.Plugins;
using MEC;
using PlayerRoles;
using RemoteAdmin;
using System;
using System.Linq;
using static RoundSummary;


namespace EffectOnHUD
{
    [CommandHandler(typeof(ClientCommandHandler))]
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class HUDPluginMain : Plugin<PluginConfig>
    {
        public override string Author => "Fallout001";
        public override string Name => "Effect_On_HUD";
        public override string Description => "Effect_On_HUD";
        public override Version Version => new Version(0, 0, 1);
        public override Version RequiredApiVersion => LabApi.Features.LabApiProperties.CurrentVersion;

        public static HUDPluginMain Instance;

        public static readonly Dictionary<Player, bool> CanView = new();

        public override void Enable()
        {
            Instance = this;
            CL.Info("Effect Shown on HUD Plugin Enabled");
            PlayerEvents.ChangedRole += OnPlayerRoleChanged;
            ServerEvents.RoundEnding += OnRoundEnding;
            ServerSpecificSettings.Initialize();
        }

        public override void Disable()
        {
            Instance = null;
            CL.Info("Effect Shown on HUD Plugin Disabled");
            PlayerEvents.ChangedRole -= OnPlayerRoleChanged;
            ServerEvents.RoundEnding -= OnRoundEnding;
            ServerSpecificSettings.DeInitialize();
        }

        private void OnPlayerRoleChanged(PlayerChangedRoleEventArgs ev)
        {
            ShowEffects.PlayerHpModifiers.Remove(ev.Player);

            if (!CanView.ContainsKey(ev.Player))
                CanView.Add(ev.Player, false);
            else
                CanView[ev.Player] = false;

            Timing.CallDelayed(5f, () =>
            {
                CanView[ev.Player] = true;
            });
        }

        private void OnRoundEnding(RoundEndingEventArgs ev)
        {
            int uniqueCount = ShowEffects.UniqueUsersThisRound.Count;

                CalamityStatsTracker.RoundStatsTracker.AddStatEvent(
                    "EffectOnHud",
                    "UniqueUsers",
                    "ShowEffects",
                    $"UniqueUserCount={uniqueCount}"
                );

            ShowEffects.UniqueUsersThisRound.Clear();
        }
    }
}


