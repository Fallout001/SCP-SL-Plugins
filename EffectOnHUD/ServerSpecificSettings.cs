using Exiled.API.Features;
using Exiled.API.Interfaces;
using Exiled.Events.EventArgs;
using Exiled.API.Features;
using UserSettings.ServerSpecific;
using System.Collections.Generic;
using static System.Runtime.CompilerServices.RuntimeHelpers;
using Exiled.API.Extensions;
using Exiled.Events.Commands.Reload;
using System.ComponentModel;

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
                                new SSGroupHeader("Effects On HUD", false),
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
                    ShowEffects.ShowEffectsOnHUD(player);
                }
            }
        }
    }
}