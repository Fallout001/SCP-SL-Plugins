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

        public static void SaveCurrentRoundStats()
        {
            foreach (var statEvent in CurrentRoundStats)
            {
                string statsDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "StatsTracker");
                Directory.CreateDirectory(statsDir);

                string fileName = $"RoundStats_{DateTime.Now:yyyyMMdd_HHmmss}.txt";
                string filePath = Path.Combine(statsDir, fileName);

                var line = CurrentRoundStats.Select(statEvent =>
                $"{statEvent.Timestamp:u} | {statEvent.PluginName} | {statEvent.EventType} | {statEvent.EventName} | {statEvent.RoundTime}s | {statEvent.ExtraData}");

                CL.Error($"Saving stats to {filePath}");

                File.WriteAllLines(filePath, line);
            }
            // Clear the stats after saving just in case 
            CurrentRoundStats.Clear();
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
    
     
