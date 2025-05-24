using CommandSystem;
using LabApi;
using LabApi.Events;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Handlers;
using LabApi.Loader.Features.Plugins;
using PlayerRoles;
using RemoteAdmin;
using System;
using System.Linq;


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
        public override void Enable()
        {
            Instance = this;
            CL.Info("Effect Shown on HUD Plugin Enabled");
            PlayerEvents.ChangedRole += OnPlayerRoleChanged;
            ServerSpecificSettings.Initialize();
        }

        public override void Disable()
        {
            Instance = null;
            CL.Info("Effect Shown on HUD Plugin Disabled");
            PlayerEvents.ChangedRole -= OnPlayerRoleChanged;
            ServerSpecificSettings.DeInitialize();
        }

        private void OnPlayerRoleChanged(PlayerChangedRoleEventArgs ev)
        {
                ShowEffects.PlayerHpModifiers.Remove(ev.Player);
        }
    }
}


