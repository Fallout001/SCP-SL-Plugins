using InventorySystem.Items.Firearms.ShotEvents;
using LabApi;
using LabApi.Loader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalamityStatsTracker
{
    public class RoundStatsTracker
    {
        public static List<BaseStatEvent> CurrentRoundStats = new();

        public static DateTime roundStartTime = DateTime.Now;

        public static int RoundNumber = LoadRoundNumber();

        private static string statsDir = Path.Combine(CalamityStatsTracker.Instance.GetConfigDirectory().FullName, "StatsTracker");
        private static string roundNumberFile = Path.Combine(statsDir, "CurrentRoundNumber.txt");

        private static int LoadRoundNumber()
        {
            try
            {
                Directory.CreateDirectory(statsDir);
                if (File.Exists(roundNumberFile))
                {
                    var text = File.ReadAllText(roundNumberFile);
                    if (int.TryParse(text, out int num))
                        return num;
                }
            }
            catch (Exception ex)
            {
                CL.Error($"Error loading round number: {ex}");
            }
            return 1;
        }

        private static void SaveRoundNumber()
        {
            try
            {
                File.WriteAllText(roundNumberFile, RoundNumber.ToString());
            }
            catch (Exception ex)
            {
                CL.Error($"Error saving round number: {ex}");
            }
        }

        public static void SaveCurrentRoundStats()
        {
            Directory.CreateDirectory(statsDir);

            string filePath = Path.Combine(statsDir, "AllRoundsStats.csv");
            bool fileExists = File.Exists(filePath);

            var csvLines = new List<string>();

            if (!fileExists)
            {
                CL.Debug($"File not exist, adding new lines");
                csvLines.Add("RoundNumber,Timestamp,PluginName,EventType,EventName,RoundTime,ExtraData");
            }

            foreach (var statEvent in CurrentRoundStats)
            {
                string extraData = statEvent.ExtraData?.Replace("\"", "\"\"") ?? "";
                if (extraData.Contains(",") || extraData.Contains("\""))
                    extraData = $"\"{extraData}\"";

                CL.Debug($"StatEventWithin list. extra data = {extraData}");

                csvLines.Add($"{RoundNumber},{statEvent.Timestamp:u},{statEvent.PluginName},{statEvent.EventType},{statEvent.EventName},{statEvent.RoundTime},{extraData}");
            }

            CL.Debug($"Appending stats to {filePath}");

            File.AppendAllLines(filePath, csvLines);

            CL.Debug($"Appending Done");

            CurrentRoundStats.Clear();
            RoundNumber++;
            SaveRoundNumber();
        }

        public static void AddStatEvent(string PluginName, string EventType, string EventName, string ExtraData)
        {
            var StatEvent = new BaseStatEvent
            {
                PluginName = PluginName,
                EventType = EventType,
                EventName = EventName,
                Timestamp = DateTime.Now,
                RoundTime = (int)(DateTime.Now - roundStartTime).TotalSeconds,
                ExtraData = ExtraData
            };

            CL.Debug($"Adding stat event: {StatEvent.EventName} at {StatEvent.Timestamp:u}");
            CurrentRoundStats.Add(StatEvent);

        }
    }
}
    
     
