using LabApi.Events.Arguments.ServerEvents;
using LabApi.Events.Handlers;
using LabApi.Loader.Features.Plugins;
using LabApi.Loader;

namespace EndRoundFF;

public class PluginMain : Plugin<PluginConfig>
{
    public override string Author => "Fallout001";
    public override string Name => "EndRoundFF";
    public override string Description => "End Of round Friendly Fire";
    public override Version Version => new(0, 0, 1);
    public override Version RequiredApiVersion => LabApi.Features.LabApiProperties.CurrentVersion;

    public static PluginMain Instance;

    public string StatsTrackerPath = string.Empty;

    public override void Enable()
    {
        Instance = this;
        ServerEvents.RoundEnding += OnRoundEnding;
        ServerEvents.WaitingForPlayers += OnWaitingForPlayers;
    }

    public override void Disable()
    {
        Instance = null;
        ServerEvents.RoundEnding -= OnRoundEnding;
    }
    void OnRoundEnding(RoundEndingEventArgs ev)
    {
        Server.FriendlyFire = true;
    }

    void OnWaitingForPlayers()
    {
        Server.FriendlyFire = false; // just in case it dont reset between rounds but can remove if it does
    }
}


