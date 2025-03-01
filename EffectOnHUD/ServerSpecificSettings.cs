using Exiled.API.Features;
using Exiled.API.Interfaces;
using Exiled.Events.EventArgs;
using Exiled.API.Features;
using UserSettings.ServerSpecific;
using System.Collections.Generic;
using static System.Runtime.CompilerServices.RuntimeHelpers;
using Exiled.API.Extensions;
using Exiled.Events.Commands.Reload;

namespace EffectOnHUD
{
    internal class ServerSpecificSettings : Plugin<PluginConfig>
    {
        private static SSKeybindSetting showEffectsKb;



        public static void Initialize()
        {
            showEffectsKb = new SSKeybindSetting(null, "Show my Effects");
            var Settings = new List<ServerSpecificSettingBase>
                            {
                                new SSGroupHeader("EffectOnHud", false),
                                showEffectsKb
                            };
            ServerSpecificSettingsSync.DefinedSettings = Settings.ToArray();
            ServerSpecificSettingsSync.ServerOnSettingValueReceived += ServerOnSettingValueReceived;
            ServerSpecificSettingsSync.SendToAll();
        }

        public static void DeInitialize()
        {
            ServerSpecificSettingsSync.DefinedSettings = new ServerSpecificSettingBase[0];
            ServerSpecificSettingsSync.ServerOnSettingValueReceived -= ServerOnSettingValueReceived;
            ServerSpecificSettingsSync.SendToAll();
        }

        private static void ServerOnSettingValueReceived(ReferenceHub hub, ServerSpecificSettingBase @base)
        {
            var keybindSetting = @base as SSKeybindSetting;
            if (keybindSetting == null || !keybindSetting.SyncIsPressed)
            {
                return;
            }
            if (keybindSetting.SettingId == showEffectsKb.SettingId)
            {

                var player = Player.Get(hub);
                if (player != null)
                {
                    string response = "<align=\"left\"><size=20>Your Effects: \n";


                    foreach (var effect in player.ActiveEffects)
                    {
                       
                        string effectName = EffectTypeExtension.GetEffectType(effect).ToString();
                        if(effect.Duration != 0)
                        {
                            response += effectName + $" ({(int)effect.TimeLeft} s, Intensity:{effect.Intensity})" + "\n";
                        }
                        else
                        {
                            response += effectName + $" (Infinite, Intensity:{effect.Intensity})" + "\n";
                        }
                       
                    }
                    response += "</size></align>";
                    player.ShowHint(response, 5);
                }
            }
        }
    }
}