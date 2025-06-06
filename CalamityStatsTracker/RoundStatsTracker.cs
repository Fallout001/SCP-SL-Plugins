using InventorySystem.Items.Firearms.ShotEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EffectOnHUD
{
    internal class RoundStatsTracker
    {
        public static List<BaseStatEvent> CurrentRoundStats = new();
        public static void SaveCurrentRoundStats(int roundNumber)
        {
           foreach (var statEvent in CurrentRoundStats)
            {
                
            }
            // Clear the stats after saving jusst in case 
            CurrentRoundStats.Clear();
        }
    }
}
