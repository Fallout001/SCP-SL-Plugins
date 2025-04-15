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
using Exiled.API.Features.Core.UserSettings;

namespace EffectOnHUD
{
    internal class ServerSpecificSettings : Plugin<PluginConfig>
    {
        public static SSKeybindSetting showEffectsKb;
        public static SSTwoButtonsSetting showIntensityButton;
        public static SSSliderSetting textSizeSlider;

        public static void Initialize()
        {
            showEffectsKb = new SSKeybindSetting(null, "Show my Effects");
            showIntensityButton = new SSTwoButtonsSetting(null, "Show Intensity", "True", "False");
            textSizeSlider = new SSSliderSetting(null, "Text Size", HUDPluginMain.Instance.Config.MinTextSize, HUDPluginMain.Instance.Config.MaxTextSize, HUDPluginMain.Instance.Config.DefaultTextSize);

            var Settings = new List<ServerSpecificSettingBase>
                    {
                        new SSGroupHeader("Effects On HUD", false),
                        showEffectsKb,
                        showIntensityButton,
                        textSizeSlider
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
            var player = Player.Get(hub);
            if (player == null)
                return;

            // Check if the keybind setting was pressed
            if (@base is SSKeybindSetting keybindSetting && keybindSetting.SettingId == showEffectsKb.SettingId)
            {
                // Retrieve player-specific settings
                bool showIntensity = false;
                int textSize = (int)HUDPluginMain.Instance.Config.DefaultTextSize;

                if (ServerSpecificSettings.showIntensityButton is SSTwoButtonsSetting intensitySetting)
                {
                    showIntensity = !intensitySetting.SyncIsB; // SyncIsA is true when SyncIsB is false
                }

                if (ServerSpecificSettings.textSizeSlider is SSSliderSetting sizeSetting)
                {
                    textSize = (int)sizeSetting.SyncIntValue;
                }

                // Call ShowEffectsOnHUD with player-specific settings
                ShowEffects.ShowEffectsOnHUD(player, showIntensity, textSize);
            }
        }
    }
}