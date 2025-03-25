using Exiled.Events;
using Exiled.Events.EventArgs.Server;
using Exiled.API.Extensions;
using MEC;
using System.Linq.Expressions;
using Exiled.API.Features;


namespace PopulationXpLimiter.EventHandlers
{
    public class Events
    {
        public static void OnRoundStart()
        {
            if (Exiled.API.Features.Server.PlayerCount < PluginMain.Instance.Config.PlayerRequirement)
            {
                int var = Exiled.API.Features.Server.PlayerCount;
                string var1 = var.ToString(); //counting and storing player count

                Log.Info("Recorded Player Count = " + var1); // logging the recorded player count 
                Log.Info("XP System disabled due to Low player Count"); // logging current status of the plugin

                PluginMain.Instance.Config.XPAlreadyReEnabled = false;

                Timing.CallDelayed(PluginMain.Instance.Config.BroadcastDelay, () =>
                {
                    Exiled.API.Features.Server.ExecuteCommand("/bc " + PluginMain.Instance.Config.BroadcastTime + " " + PluginMain.Instance.Config.BroadcastMessageDisableXP);
                    Exiled.API.Features.Server.ExecuteCommand("/xp pause true"); // sending server commands to broadcast and pause XP gain 
                });
            }
            else
            {
                if (!PluginMain.Instance.Config.XPAlreadyReEnabled)
                {
                    PluginMain.Instance.Config.XPAlreadyReEnabled = true;

                    int var = Exiled.API.Features.Server.PlayerCount;
                    string var1 = var.ToString(); //counting and storing player count

                    Log.Info("Recorded Player Count = " + var1); // logging the recorded player count
                    Log.Info("XP System Re-Enabled due to player Count");// logging current status of the plugin


                    Timing.CallDelayed(PluginMain.Instance.Config.BroadcastDelay, () =>
                    {
                        Exiled.API.Features.Server.ExecuteCommand("/bc " + PluginMain.Instance.Config.BroadcastTime + " " +  PluginMain.Instance.Config.BroadcastMessageReEnableXP);
                        Exiled.API.Features.Server.ExecuteCommand("/xp pause false"); // sending server commands to broadcast and Re-Enable XP gain
                    });
                }
            }
        }
    }
}