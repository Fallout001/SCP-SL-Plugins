using LabApi;
using LabApi.Events;
using MEC;
using PlayerRoles;

namespace EffectOnHUD
{
    public class ShowEffects
    {
        private static readonly Dictionary<Player, int> PlayerBaseHp = new();

        public static readonly Dictionary<Player, Dictionary<string, List<int>>> PlayerHpModifiers = new();

        public static void RemoveAllHpModifiers(Player player)
        {
            PlayerHpModifiers.Remove(player);
        }

        public static void SetBaseHp(Player player, int baseHp)
        {
            PlayerBaseHp[player] = baseHp;
        }

        public static void AddHpModifier(Player player, string source, int amount)
        {
            if (!PlayerHpModifiers.TryGetValue(player, out var mods))
            {
                mods = new Dictionary<string, List<int>>();
                PlayerHpModifiers[player] = mods;
            }
            if (!mods.TryGetValue(source, out var amounts))
            {
                amounts = new List<int>();
                mods[source] = amounts;
            }
            amounts.Add(amount);
        }

        public static void RemoveHpModifier(Player player, string source, int? amount = null)
        {
            if (PlayerHpModifiers.TryGetValue(player, out var mods))
            {
                if (mods.TryGetValue(source, out var amounts))
                {
                    if (amount.HasValue)
                    {
                        amounts.Remove(amount.Value);
                        if (amounts.Count == 0)
                            mods.Remove(source);
                    }
                    else
                    {
                        mods.Remove(source);
                    }
                }
                if (mods.Count == 0)
                {
                    PlayerHpModifiers.Remove(player);
                }
            }
        }
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

        private static IEnumerator<float> UpdateHudCoroutine(Player Recipient, bool showIntensity, int textSize, Player ReadinPlayer)
        {
            int updates = 5;
            while (updates-- > 0) //loop 5 times for 5 seconds
            {
                ShowEffectsOnHUD(Recipient, showIntensity, textSize, ReadinPlayer);
                yield return Timing.WaitForSeconds(1f);
            }

            Recipient?.SendHint("", 1); // send blank when done
            ActiveHudCoroutines.Remove(Recipient);
        }

        public static void StartEffectHud(Player Recipient, bool showIntensity, int textSize, Player ReadinPlayer)
        {
            if (ActiveHudCoroutines.TryGetValue(Recipient, out var handle))
                Timing.KillCoroutines(handle); //if already coroutine running for it kill it 

            CoroutineHandle newHandle = Timing.RunCoroutine(UpdateHudCoroutine(Recipient, showIntensity, textSize, ReadinPlayer)); 
            ActiveHudCoroutines[Recipient] = newHandle; // make a new coroutine for it 
            //do this so that no overlap in coroutines 
        }

        public static void ShowEffectsOnHUD(Player Recipient, bool showIntensity, int textSize, Player ReadinPlayer)
        {
            string response;

            if (Recipient == ReadinPlayer)
            {
              response = "<align=\"" + HUDPluginMain.Instance.Config.EffectDisplayAlignment + "\"><size=" + textSize + ">" + "Your Effects:" + " \n";
            }
            else
            {
              response = "<align=\"" + HUDPluginMain.Instance.Config.EffectDisplayAlignment + "\"><size=" + textSize + ">" + $"{ReadinPlayer.Nickname}'s Effects:" + " \n";
            }


            string[] GoodEffects = { "AntiScp207", "Scp1853", "Invigorated", "BodyshotReduction", "DamageReduction", "MovementBoost", "RainbowTaste", "Vitality" };
            string[] BadEffects = { "CardiacArrest", "Traumatized", "Scanned", "PocketCorroding", "Strangled", "SeveredHands", "Stained", "Hypothermia", "SinkHole", "Poisoned", "Asphyxiated", "Bleeding", "Blinded", "Burned", "Concussed", "Corroding", "Deafened", "Decontaminating", "Disabled", "Ensnared", "Exhausted", "Flashed", "Hemorrhage" };
            string[] MiscEffects = { "Scp1344", "Scp207", "Invisible", "SpawnProtected", "SilentWalk", "Ghostly" };

            int ActualMaxHp = (int)ReadinPlayer.MaxHealth;
            response += "MaxHP: " + ActualMaxHp + "\n";
            if (PlayerHpModifiers.TryGetValue(ReadinPlayer, out var modifiers) && modifiers.Count > 0)
            {
                foreach (var kvp in modifiers)
                {
                    int total = kvp.Value.Sum();
                    if (total > 0)
                    {
                        response += $" <color=" + HUDPluginMain.Instance.Config.EffectDisplayColorGood + ">" + "+" + " </color>"; //if the value is positive, show it as a plus
                        response += $" {total} {kvp.Key}\n";
                    }
                    else
                    {
                        response += $" <color=" + HUDPluginMain.Instance.Config.EffectDisplayColorBad + ">" + "-" + " </color>"; //if the value is positive, show it as a plus
                        response += $" {total * -1} {kvp.Key}\n";
                    }
                }
            }

            foreach (var effect in ReadinPlayer.ActiveEffects)
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
            Recipient.SendHint(response, HUDPluginMain.Instance.Config.EffectDisplayDuration);
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
