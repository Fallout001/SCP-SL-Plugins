using CommandSystem;
using LabApi.Events;
using LabApi;
using System;
using System.Linq;
using RemoteAdmin;
using LabApi.Loader.Features.Plugins;


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
            ServerSpecificSettings.Initialize();
        }
        public override void Disable()
        {
            Instance = null;
            CL.Info("Effect Shown on HUD Plugin Disabled");
            ServerSpecificSettings.DeInitialize();
        }
    }

}


