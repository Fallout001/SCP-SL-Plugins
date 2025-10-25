using CalamityStatsTracker;
using HintFrameworkHub;
using MEC;

namespace EffectOnHUD
{
    public class ShowEffects
    {
        private static readonly Dictionary<Player, int> PlayerBaseHp = new();

        public static readonly HashSet<string> UniqueUsersThisRound = new();

        public static readonly Dictionary<Player, Dictionary<string, List<int>>> PlayerHpModifiers = new();

        private static readonly Dictionary<Player, List<CustomHudEffect>> CustomEffects = new();

        private static readonly Dictionary<Player, List<CustomRegenEffect>> RegenEffects = new();

        private sealed class CustomHudEffect
        {
            public string Name { get; set; }
            public float Intensity { get; set; }
            public float DurationSeconds { get; set; }
            public DateTime StartedAtUtc { get; set; }
            public DateTime? ExpiresAtUtc { get; set; }
            public bool IsBinary { get; set; }
            public string ColorOverride { get; set; }
            public string Source { get; set; }

            public CustomHudEffect(
                string name,
                float intensity,
                float durationSeconds,
                DateTime startedAtUtc,
                DateTime? expiresAtUtc,
                bool isBinary,
                string colorOverride,
                string source)
            {
                Name = name;
                Intensity = intensity;
                DurationSeconds = durationSeconds;
                StartedAtUtc = startedAtUtc;
                ExpiresAtUtc = expiresAtUtc;
                IsBinary = isBinary;
                ColorOverride = colorOverride;
                Source = source;
            }
        }

        private sealed class CustomRegenEffect
        {
            public string Name { get; set; }
            public float Intensity { get; set; }
            public float DurationSeconds { get; set; }
            public DateTime StartedAtUtc { get; set; }
            public DateTime? ExpiresAtUtc { get; set; }

            public CustomRegenEffect(
                string name,
                float intensity,
                float durationSeconds,
                DateTime startedAtUtc,
                DateTime? expiresAtUtc,
                string? source)
            {
                Name = name;
                Intensity = intensity;
                DurationSeconds = durationSeconds;
                StartedAtUtc = startedAtUtc;
                ExpiresAtUtc = expiresAtUtc;
            }
        }

        private static DateTime? MaxUtc(DateTime? a, DateTime? b)
        {
            if (a is null) return b;
            if (b is null) return a;
            return a.Value >= b.Value ? a : b;
        }

        public static void AddCustomEffect(
     Player player,
     string name,
     float intensity = 1f,
     float durationSeconds = 0f,
     bool isBinary = false,
     string colorOverride = "#FFFFFF",
     string source = null)
        {
            if (player == null) return;

            if (!CustomEffects.TryGetValue(player, out var list))
            {
                list = new List<CustomHudEffect>();
                CustomEffects[player] = list;
            }

            var now = DateTime.UtcNow;
            DateTime? newExpires = durationSeconds > 0 ? now.AddSeconds(durationSeconds) : (DateTime?)null;

            var existingIndex = list.FindIndex(e =>
                string.Equals(e.Name, name, StringComparison.OrdinalIgnoreCase));

            if (existingIndex >= 0)
            {
                //extend expiry and take the max intensity
                var effect = list[existingIndex];
                effect.Intensity = Math.Max(effect.Intensity, intensity);
                effect.DurationSeconds = durationSeconds;
                effect.StartedAtUtc = now;
                effect.ExpiresAtUtc = MaxUtc(effect.ExpiresAtUtc, newExpires);
                effect.IsBinary = isBinary;
                if (!string.IsNullOrEmpty(colorOverride))
                    effect.ColorOverride = colorOverride;
            }
            else
            {
                // Create new
                list.Add(new CustomHudEffect(
                    name,
                    intensity,
                    durationSeconds,
                    now,
                    newExpires,
                    isBinary,
                    colorOverride,
                    source));
            }
        }

        public static bool RemoveCustomEffect(Player player, string name, string source = null)
        {
            if (player == null) return false;
            if (!CustomEffects.TryGetValue(player, out var list)) return false;

            int removed = list.RemoveAll(e =>
                string.Equals(e.Name, name, StringComparison.OrdinalIgnoreCase) &&
                (source == null || string.Equals(e.Source, source, StringComparison.Ordinal)));

            if (list.Count == 0) CustomEffects.Remove(player);

            return removed > 0;
        }

        public static void ClearCustomEffects(Player player)
        {
            if (player == null)
            {
                return;
            }
            CustomEffects.Remove(player);
        }

        private static List<CustomHudEffect> GetActiveCustomEffects(Player player)
        {
            if (!CustomEffects.TryGetValue(player, out var list) || list.Count == 0)
                return new List<CustomHudEffect>();

            var now = DateTime.UtcNow;

            list.RemoveAll(e => e.ExpiresAtUtc.HasValue && e.ExpiresAtUtc.Value <= now);

            if (list.Count == 0)
            {
                CustomEffects.Remove(player);
                return new List<CustomHudEffect>();
            }

            return list;
        }

        public static void AddRegenEffect(
    Player player,
    string name,
    float intensity = 1f,
    float durationSeconds = 0f, // 0 -> infinite
    string? source = null)
        {
            if (player == null) return;

            if (!RegenEffects.TryGetValue(player, out var list))
            {
                list = new List<CustomRegenEffect>(4);
                RegenEffects[player] = list;
            }

            var now = DateTime.UtcNow;
            DateTime? newExpires = durationSeconds > 0 ? now.AddSeconds(durationSeconds) : (DateTime?)null;

            // Refresh by name+source
            int idx = list.FindIndex(e =>
                string.Equals(e.Name, name, StringComparison.OrdinalIgnoreCase));

            if (idx >= 0)
            {
                var e = list[idx];
                // refresh semantics: keep higher intensity, extend to later expiry, reset start time
                e.Intensity = Math.Max(e.Intensity, intensity);
                e.DurationSeconds = durationSeconds;
                e.StartedAtUtc = now;
                e.ExpiresAtUtc = MaxUtc(e.ExpiresAtUtc, newExpires);
            }
            else
            {
                list.Add(new CustomRegenEffect(
                    name,
                    intensity,
                    durationSeconds,
                    now,
                    newExpires,
                    source));
            }
        }

        // Remove by name (and optional source)
        public static bool RemoveRegenEffect(Player player, string name, string? source = null)
        {
            if (player == null) return false;
            if (!RegenEffects.TryGetValue(player, out var list)) return false;

            int removed = list.RemoveAll(e =>
                string.Equals(e.Name, name, StringComparison.OrdinalIgnoreCase));

            if (list.Count == 0) RegenEffects.Remove(player);
            return removed > 0;
        }

        public static void ClearRegenEffects(Player player)
        {
            if (player == null)
            {
                return;
            }
            RegenEffects.Remove(player);
        }

        private static List<CustomRegenEffect> GetActiveRegenEffects(Player player)
        {
            if (!RegenEffects.TryGetValue(player, out var list) || list.Count == 0)
                return new List<CustomRegenEffect>();

            var now = DateTime.UtcNow;
            list.RemoveAll(e => e.ExpiresAtUtc.HasValue && e.ExpiresAtUtc.Value <= now);

            if (list.Count == 0)
            {
                RegenEffects.Remove(player);
                return new List<CustomRegenEffect>();
            }

            return list;
        }

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

            HintSystem.ShowHint(Recipient, "", 1, HintSystem.HintType.InformationLeft, true);
            ActiveHudCoroutines.Remove(Recipient);
            RoundStatsTracker.AddStatEvent("EffectOnHud", "KeybindPressed", "ShowEffects", $"player role = {Recipient.Role} , Target Role = {ReadinPlayer.Role}"); // log the event
        }

        public static void StartEffectHud(Player Recipient, bool showIntensity, int textSize, Player ReadinPlayer)
        {
            if (!string.IsNullOrEmpty(Recipient.UserId))
            {
                UniqueUsersThisRound.Add(Recipient.UserId);
            }
            else
            {
                CL.Error("Recipient UserId is null or empty. Cannot track unique users.");
            }

            if (ActiveHudCoroutines.TryGetValue(Recipient, out var handle))
                Timing.KillCoroutines(handle); //if already coroutine running for it kill it 

            CoroutineHandle newHandle = Timing.RunCoroutine(UpdateHudCoroutine(Recipient, showIntensity, textSize, ReadinPlayer));
            ActiveHudCoroutines[Recipient] = newHandle; // make a new coroutine for it 
            //do this so that no overlap in coroutines 
        }

        public static void ShowEffectsOnHUD(Player Recipient, bool showIntensity, int textSize, Player ReadinPlayer)
        {

            string response = string.Empty;

            if (Recipient == ReadinPlayer)
            {
                response = "<size=" + textSize + ">" + "Your Effects:" + "\n";
            }
            else
            {
                response = "<size=" + textSize + ">" + $"{ReadinPlayer.Nickname}'s Effects:" + " \n";
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
                    if (total != 0)
                    {
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
                    else
                    {
                        response += $" <color=" + HUDPluginMain.Instance.Config.EffectDisplayColorMixed + ">" + "+" + " </color>"; //if the value is positive, show it as a plus
                        response += $"{kvp.Key}\n";
                    }
                }
            }

            var regens = GetActiveRegenEffects(ReadinPlayer);
            if(regens.Count > 0)
            {
                response += "Regeneration:\n"; 

                foreach (var regen in regens)
                {
                    if (showIntensity)
                    {
                        if (regen.ExpiresAtUtc != null)
                        {
                            var secs = (int)Math.Max(0, (regen.ExpiresAtUtc.Value - DateTime.UtcNow).TotalSeconds);
                            response += $" <color=" + HUDPluginMain.Instance.Config.EffectDisplayColorGood + ">" + regen.Name + $"</color> ({secs} s, {GetFormattedIntensity(regen.Name, regen.Intensity)})" + "\n";
                        }
                        else
                        {
                            response += $" <color=" + HUDPluginMain.Instance.Config.EffectDisplayColorGood + ">" + regen.Name + $"</color> (∞, {GetFormattedIntensity(regen.Name, regen.Intensity)})" + "\n";
                        }
                    }
                    else
                    {
                        if (regen.ExpiresAtUtc != null)
                        {
                            var secs = (int)Math.Max(0, (regen.ExpiresAtUtc.Value - DateTime.UtcNow).TotalSeconds);
                            response += $" <color=" + HUDPluginMain.Instance.Config.EffectDisplayColorGood + ">" + regen.Name + $"</color> ({secs} s)" + "\n";
                        }
                        else
                        {
                            response += $" <color=" + HUDPluginMain.Instance.Config.EffectDisplayColorGood + ">" + regen.Name + $"</color> (∞)" + "\n";
                        }
                    }
                }
            }

            response += "Effects:\n";

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

            var customs = GetActiveCustomEffects(ReadinPlayer);
            if (customs == null)
            {
                //not goog
            }

            if (customs.Count > 0)
            {
                foreach (var e in customs)
                {
                    // Color priority: explicit override -> buckets (Good/Misc/else Bad)
                    string colorCode =
                        !string.IsNullOrWhiteSpace(e.ColorOverride)
                            ? $"<color={e.ColorOverride}>"
                            : (GoodEffects.Contains(e.Name)
                                ? "<color=" + HUDPluginMain.Instance.Config.EffectDisplayColorGood + ">"
                                : (MiscEffects.Contains(e.Name)
                                    ? "<color=" + HUDPluginMain.Instance.Config.EffectDisplayColorMixed + ">"
                                    : "<color=" + HUDPluginMain.Instance.Config.EffectDisplayColorBad + ">"));

                    // Determine time-left text
                    string timeText;
                    if (e.ExpiresAtUtc.HasValue)
                    {
                        var secs = (int)Math.Max(0, (e.ExpiresAtUtc.Value - DateTime.UtcNow).TotalSeconds);
                        if (secs == 0) continue; // will be pruned next tick
                        timeText = $"{secs} s";
                    }
                    else
                    {
                        timeText = "∞";
                    }

                    if (e.IsBinary || BinaryEffects.Contains(e.Name))
                    {
                        response += colorCode + e.Name + $"</color> ({timeText})" + "\n";
                    }
                    else
                    {
                        if (showIntensity)
                            response += colorCode + e.Name + $"</color> ({timeText}, {GetFormattedIntensity(e.Name, e.Intensity)})" + "\n";
                        else
                            response += colorCode + e.Name + $"</color> ({timeText})" + "\n";
                    }
                }
            }

            response += "</size>";
            HintSystem.ShowHint(Recipient, response, 1.0f, HintSystem.HintType.InformationLeft, true);
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