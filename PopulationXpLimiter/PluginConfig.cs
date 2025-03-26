using Exiled.API.Interfaces;

namespace PopulationXpLimiter
{
    public class PluginConfig : IConfig
    {
        public bool IsEnabled { get; set; } = true;
        public bool Debug { get; set; }
        public int PlayerRequirement { get; set; } //The number of players needed to Re-Enable the XP plugin

        public int BroadcastTime { get; set; }  //The amount of time in seconds that the Overhead broadcast will last

        public int BroadcastDelay { get; set; } // Amount of time in seconds to delay the Broadcast before it appears after the round has started

        public bool XPAlreadyReEnabled { get; set; } // boolean to signify whether the Xp system has already shown re-enabled status message 

        public string BroadcastMessageDisableXP { get; set; } = "<color=white> <u> <b> <color=yellow> Alert: <color=white> The <color=Green> XP-System <color=white> Has been Disabled Due to Low Population";
        public string BroadcastMessageReEnableXP { get; set; } = "<color=white> <u> <b> <color=yellow> WARNING: <color=white> The <color=Green> XP-System <color=white> Has been Re-Enabled";

    }


}

