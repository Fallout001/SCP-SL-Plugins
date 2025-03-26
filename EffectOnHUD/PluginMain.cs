using CommandSystem;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Interfaces;
using Exiled.Events.EventArgs; 
using System;
using System.Linq;
using RemoteAdmin;


namespace EffectOnHUD
{
    [CommandHandler(typeof(ClientCommandHandler))]
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class HUDPluginMain : Plugin<PluginConfig>
    {
        public override string Author => "Fallout001";
        public override string Name => "Effect_On_HUD";
        public override string Prefix => Name;
        public override Version Version => new Version(0, 0, 1);
        public override Version RequiredExiledVersion => new Version(8, 12, 2);

        public static HUDPluginMain Instance;
        public override void OnEnabled()
        {
            Instance = this;
            PluginAPI.Core.Log.Debug("Effect Shown on HUD Plugin Enabled");
            ServerSpecificSettings.Initialize();
            base.OnEnabled();
        }
        public override void OnDisabled()
        {
            Instance = null;
            PluginAPI.Core.Log.Debug("Effect Shown on HUD Plugin Disabled");
            ServerSpecificSettings.DeInitialize();
            base.OnDisabled();
        }
    }

}


