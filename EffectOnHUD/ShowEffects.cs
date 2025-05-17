using LabApi.Events;
using LabApi;
using MEC;

namespace EffectOnHUD
{
    internal class ShowEffects
    {
        private static readonly HashSet<string> BinaryEffects = new HashSet<string>
        {
        "Invisible",
        "SpawnProtected",
        "Ghostly",
        "Scanned",
        "Flashed",
        "Invigorated",
        "RainbowTaste",
        "Vitality",
        "CardiacArrest",
        "PocketCorroding",
        "Strangled",
        "SeveredHands",
        "Stained",
        "SinkHole",
        "Decontaminating",
        };

        private static readonly Dictionary<Player, CoroutineHandle> ActiveHudCoroutines = new();

        private static IEnumerator<float> UpdateHudCoroutine(Player player, bool showIntensity, int textSize)
        {
            int updates = 5;
            while (updates-- > 0) //loop 5 times for 5 seconds
            {
                ShowEffectsOnHUD(player, showIntensity, textSize);
                yield return Timing.WaitForSeconds(1f);
            }

            player?.SendHint("", 1); // send blank when done
            ActiveHudCoroutines.Remove(player);
        }

        public static void StartEffectHud(Player player, bool showIntensity, int textSize)
        {
            if (ActiveHudCoroutines.TryGetValue(player, out var handle))
                Timing.KillCoroutines(handle); //if already coroutine running for it kill it 

            CoroutineHandle newHandle = Timing.RunCoroutine(UpdateHudCoroutine(player, showIntensity, textSize)); 
            ActiveHudCoroutines[player] = newHandle; // make a new coroutine for it 
            //do this so that no overlap in coroutines 
        }

        public static void ShowEffectsOnHUD(Player player, bool showIntensity, int textSize)
        {
            if (player == null || !player.IsAlive)
                return;

            string response = "<align=\"" + HUDPluginMain.Instance.Config.EffectDisplayAlignment + "\"><size=" + textSize + ">" + HUDPluginMain.Instance.Config.DisplayHeader + " \n";

            string[] GoodEffects = { "AntiScp207", "Scp1853", "Invigorated", "BodyshotReduction", "DamageReduction", "MovementBoost", "RainbowTaste", "Vitality" };
            string[] BadEffects = { "CardiacArrest", "Traumatized", "Scanned", "PocketCorroding", "Strangled", "SeveredHands", "Stained", "Hypothermia", "SinkHole", "Poisoned", "Asphyxiated", "Bleeding", "Blinded", "Burned", "Concussed", "Corroding", "Deafened", "Decontaminating", "Disabled", "Ensnared", "Exhausted", "Flashed", "Hemorrhage" };
            string[] MiscEffects = { "Scp1344", "Scp207", "Invisible", "SpawnProtected", "SilentWalk", "Ghostly" };

            foreach (var effect in player.ActiveEffects)
            {
                string effectName = effect.name;
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
                    if (BinaryEffects.Contains(effectName))
                    {
                        // Do not show intensity for binary effects
                        if (effect.Duration != 0)
                        {
                            response += colorCode + effectName + $"</color> ({(int)effect.TimeLeft} s)" + "\n";
                        }
                        else
                        {
                            response += colorCode + effectName + $"</color> (∞)" + "\n";
                        }
                    }
                    else
                    {
                        // Show intensity based on the user's setting
                        if (showIntensity)
                        {
                            if (effect.Duration != 0)
                            {
                                response += colorCode + effectName + $"</color> ({(int)effect.TimeLeft} s, {GetFormattedIntensity(effectName, effect.Intensity)})" + "\n";
                               
                            }
                            else
                            {
                                response += colorCode + effectName + $"</color> (∞, {GetFormattedIntensity(effectName, effect.Intensity)})" + "\n";
                            }
                        }
                        else
                        {
                            if (effect.Duration != 0)
                            {
                                response += colorCode + effectName + $"</color> ({(int)effect.TimeLeft} s)" + "\n";
                            }
                            else
                            {
                                response += colorCode + effectName + $"</color> (∞)" + "\n";
                            }
                        }
                    }
                }
            }
            response += "</size></align>";
            player.SendHint(response, HUDPluginMain.Instance.Config.EffectDisplayDuration);
        }

        private static string GetFormattedIntensity(string effectName, float intensity)
        {
            return effectName switch
            {
                // "Scp207" => ((int)intensity).ToString(), // Static numbers
                //  "BodyshotReduction" => $"{intensity * 100:F1}%", // Percentage
                //   _ => intensity.ToString("F1") // Default formatting
                "Scp207" => ((int)intensity).ToString(),
                "BodyshotReduction" => GetBodyshotReduction(intensity),
                "DamageReduction" => GetDamageReduction(intensity),
                "MovementBoost" => $"{intensity:F1}%", // Each intensity level is 1% boost
                "AntiScp207" => ((int)intensity).ToString(),
                "Scp1853" => ((int)intensity).ToString(),
                "Traumatized" => ((int)intensity).ToString(),
                "Hypothermia" => ((int)intensity).ToString(),
                "Poisoned" => ((int)intensity).ToString(),
                "Asphyxiated" => ((int)intensity).ToString(),
                "Bleeding" => ((int)intensity).ToString(),
                "Blinded" => ((int)intensity).ToString(),
                "Burned" => ((int)intensity).ToString(),
                "Concussed" => ((int)intensity).ToString(),
                "Corroding" => ((int)intensity).ToString(),
                "Deafened" => ((int)intensity).ToString(),
                "Disabled" => ((int)intensity).ToString(),
                "Ensnared" => ((int)intensity).ToString(),
                "Exhausted" => ((int)intensity).ToString(),
                "Hemorrhage" => ((int)intensity).ToString(),
                "Scp1344" => ((int)intensity).ToString(),
                "SilentWalk" => ((int)intensity).ToString(),
                _ => intensity.ToString("F1") // Default formatting
            };
        }

        private static string GetBodyshotReduction(float intensity)
        {
            float reduction = intensity switch
            {
                1 => 5f,
                2 => 10f,
                3 => 12.5f,
                4 => 15f,
                _ => 15f // Cap at 15%
            };
            return $"{reduction:F1}%";
        }

        private static string GetDamageReduction(float intensity)
        {
            float reduction = intensity * 0.5f; // Each intensity adds 0.5%
            return $"{reduction:F1}%";
        }
    }

}
