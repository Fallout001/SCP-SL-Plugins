using InventorySystem.Items.Firearms.ShotEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LabApi;
using System.IO;

namespace EffectOnHUD
{
    internal class RoundStatsTracker
    {
        public static List<BaseStatEvent> CurrentRoundStats = new();

        public static int roundStartTime = DateTime.Now.Second;

        public static int RoundNumber = LoadRoundNumber();

        private static string statsDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "StatsTracker");
        private static string roundNumberFile = Path.Combine(statsDir, "CurrentRoundNumber.txt");

        private static int LoadRoundNumber()
        {
            Directory.CreateDirectory(statsDir);
            if (File.Exists(roundNumberFile))
            {
                var text = File.ReadAllText(roundNumberFile);
                if (int.TryParse(text, out int num))
                    return num;
            }
            return 1;
        }

        private static void SaveRoundNumber()
        {
            File.WriteAllText(roundNumberFile, RoundNumber.ToString());
        }

        public static void SaveCurrentRoundStats()
        {
            Directory.CreateDirectory(statsDir);

            string filePath = Path.Combine(statsDir, "AllRoundsStats.csv");
            bool fileExists = File.Exists(filePath);

            var csvLines = new List<string>();

            if (!fileExists)
            {
                csvLines.Add("RoundNumber,Timestamp,PluginName,EventType,EventName,RoundTime,ExtraData");
            }

            foreach (var statEvent in CurrentRoundStats)
            {
                string extraData = statEvent.ExtraData?.Replace("\"", "\"\"") ?? "";
                if (extraData.Contains(",") || extraData.Contains("\""))
                    extraData = $"\"{extraData}\"";

                csvLines.Add($"{RoundNumber},{statEvent.Timestamp:u},{statEvent.PluginName},{statEvent.EventType},{statEvent.EventName},{statEvent.RoundTime},{extraData}");
            }

            CL.Error($"Appending stats to {filePath}");

            File.AppendAllLines(filePath, csvLines);

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
                RoundTime = DateTime.Now.Second - roundStartTime,
                ExtraData = ExtraData
            };
            
            CL.Error($"Adding stat event: {StatEvent.EventName} at {StatEvent.Timestamp:u}");
            CurrentRoundStats.Add(StatEvent);

        }
    }
}
    
     
