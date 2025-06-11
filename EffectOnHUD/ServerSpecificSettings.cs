using PlayerRoles;
using PlayerRoles.Spectating;
using UserSettings.ServerSpecific;
using CalamityStatsTracker;


namespace EffectOnHUD
{
    internal class ServerSpecificSettings
    {
        public static SSKeybindSetting showEffectsKb;
        public static SSTwoButtonsSetting showIntensityButton;
        public static SSSliderSetting textSizeSlider;
        public static Dictionary<ReferenceHub, (bool /* showIntensity */, float /* textSize */)> SettingsForPlayer = [];

        public static void Initialize()
        {
            showEffectsKb = new SSKeybindSetting(null, "Show my Effects");
            showEffectsKb.PreventInteractionOnGUI = false;
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

            if (!SettingsForPlayer.TryGetValue(hub, out var val))
            {
                SettingsForPlayer.Add(hub, (false, HUDPluginMain.Instance.Config.MinTextSize));
            }
            var settings = SettingsForPlayer[hub];
            
            var player = Player.Get(hub);
            if (player == null)
                return;

            if (@base is SSTwoButtonsSetting twoButton && twoButton.SettingId == showIntensityButton.SettingId)
            {
                settings.Item1 = twoButton.SyncIsA;
            }

            if (@base is SSSliderSetting slider && slider.SettingId == textSizeSlider.SettingId)
            {
                settings.Item2 = slider.SyncFloatValue;
            }

            // Check if the keybind setting was pressed
            if (@base is SSKeybindSetting keybindSetting && keybindSetting.SettingId == showEffectsKb.SettingId)
            {;
                if (HUDPluginMain.CanView.TryGetValue(player, out var value) && value == true)
                {
                    if (player.Role == RoleTypeId.Spectator || player.Role == RoleTypeId.None) // if the player who pressed it is a spectator
                    {
                        Player spectated = null;
                        foreach (var person in Player.List) //loop through and find who they are spectating 
                        {
                            if (person.ReferenceHub.IsSpectatedBy(player.ReferenceHub)) // idk if this works lmao
                            {
                                spectated = person; //set that spectated person 
                                break;
                            }
                        }

                        if (spectated != null) // if player is spectating a person 
                        {
                            ShowEffects.StartEffectHud(player, settings.Item1, (int)settings.Item2, spectated); // run it with that person being spectated effects 
                        }
                        else
                        {
                            //if it reaches here either they arent spectating a proper player or somthing broke :fire:
                        }
                    }
                    else
                    {
                        // Show the player's own effects because they are not spectator
                        ShowEffects.StartEffectHud(player, settings.Item1, (int)settings.Item2, player);
                    }
                }
            }

            SettingsForPlayer[hub] = settings;
        }
    }
}