using LabApi.Events.Arguments.ServerEvents;
using LabApi.Events.Handlers;
using LabApi.Loader.Features.Plugins;
using LabApi.Loader;

namespace CalamityStatsTracker;

public class PluginMain : Plugin<PluginConfig>
{
    public override string Author => "Fallout001";
    public override string Name => "CalamityStatsTracker";
    public override string Description => "CalamityStatsTracker";
    public override Version Version => new(0, 0, 1);
    public override Version RequiredApiVersion => LabApi.Features.LabApiProperties.CurrentVersion;

    public static PluginMain Instance;

    public string StatsTrackerPath = string.Empty;

    public override void Enable()
    {
        Instance = this;
        StatsTrackerPath = this.GetConfigDirectory().CreateSubdirectory("StatsTracker").FullName;
        RoundStatsTracker.SaveRoundNumber();
        ServerEvents.RoundStarted += OnRoundStarted; 
        ServerEvents.RoundEnding += OnRoundEnding; 
        CL.Info("Calamity Stats Tracker Enabled");
    }

    public override void Disable()
    {
        Instance = null;
        ServerEvents.RoundStarted -= OnRoundStarted; 
        ServerEvents.RoundEnding -= OnRoundEnding;
        CL.Info("Calamity Stats Tracker Disabled");
    }

    void OnRoundStarted()
    {
        RoundStatsTracker.CurrentRoundStats.Clear(); // Clear stats for new round
        RoundStatsTracker.roundStartTime = DateTime.Now;

        RoundStatsTracker.AddStatEvent("CalamityStatsTracker", "Round", "RoundStarted","Round has started");

    }

    void OnRoundEnding(RoundEndingEventArgs ev)
    {
        RoundStatsTracker.AddStatEvent("CalamityStatsTracker", "Round", "RoundEnded", "Round has ended");

        RoundStatsTracker.SaveCurrentRoundStats(); // Save stats when round ends
    }
}


