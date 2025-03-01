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
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (sender is PlayerCommandSender pcs)
            {
                var player = Player.List.Where(x => x.UserId == pcs.SenderId).FirstOrDefault();
                if (player == null)
                {
                    response = "Must be coming from a valid player";
                    return false;
                }
                response = "Your effects: \n";
                foreach (var effect in player.ActiveEffects)
                {
                    string effectName = EffectTypeExtension.GetEffectType(effect).ToString();
                    response += effectName + $" (d: {effect.Duration} i:{effect.Intensity})" + "\n";
                }
                return true;

            }
            response = "Must be coming from Player!";
            return false;
        }
    }

}


