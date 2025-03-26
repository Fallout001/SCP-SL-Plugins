using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.Events.EventArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EffectOnHUD
{
    internal class ShowEffects : ServerSpecificSettings
    {
        public static void ShowEffectsOnHUD(Player player)
        {
            string response = "<align=\"" + HUDPluginMain.Instance.Config.EffectDisplayAlignment + "\"><size=" + HUDPluginMain.Instance.Config.EffectDisplayFontSize + ">" + HUDPluginMain.Instance.Config.DisplayHeader + " \n";

            string[] GoodEffects = { "AntiScp207", "Scp1853", "Invigorated", "BodyshotReduction", "DamageReduction", "MovementBoost", "RainbowTaste", "Vitality" };
            string[] BadEffects = { "CardiacArrest", "Traumatized", "Scanned", "PocketCorroding", "Strangled", "SeveredHands", "Stained", "Hypothermia", "SinkHole", "Poisoned", "Asphyxiated", "Bleeding", "Blinded", "Burned", "Concussed", "Corroding", "Deafened", "Decontaminating", "Disabled", "Ensnared", "Exhausted", "Flashed", "Hemorrhage" };
            string[] MiscEffects = { "Scp1344", "Scp207", "Invisible", "SpawnProtected", "SilentWalk", "Ghostly" };

            foreach (var effect in player.ActiveEffects)
            {
                string effectName = EffectTypeExtension.GetEffectType(effect).ToString();
                string colorCode = "<color=" + HUDPluginMain.Instance.Config.EffectDisplayColorBad + ">"; // Default color for Bad effects"<color=#C50000>"

                if (GoodEffects.Contains(effectName))
                {
                    colorCode = "<color=" + HUDPluginMain.Instance.Config.EffectDisplayColorGood + ">"; ; // Green for Good effects
                }
                else if (MiscEffects.Contains(effectName))
                {
                    colorCode = "<color=" + HUDPluginMain.Instance.Config.EffectDisplayColorMixed + ">"; ; // Purple for Misc effects
                }

                if (effectName != "AmnesiaItems" && effectName != "AmnesiaVision")
                {
                    if (effect.Duration != 0)
                    {
                        response += colorCode + effectName + $"</color> ({(int)effect.TimeLeft} s, Intensity:{effect.Intensity})" + "\n";
                    }
                    else
                    {
                        response += colorCode + effectName + $"</color> (Infinite, Intensity:{effect.Intensity})" + "\n";
                    }
                }
            }
            response += "</size></align>";
            player.ShowHint(response, HUDPluginMain.Instance.Config.EffectDisplayDuration);
        }
    }
}
