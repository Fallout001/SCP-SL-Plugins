using LabApi.Events.Arguments.ServerEvents;
using LabApi.Events.Handlers;
using LabApi.Loader.Features.Plugins;
using LabApi.Loader;

namespace DisableSnake;

public class PluginMain : Plugin<PluginConfig>
{
    public override string Author => "Fallout001";
    public override string Name => "DisableSnake";
    public override string Description => "DisableSnake";
    public override Version Version => new(0, 0, 1);
    public override Version RequiredApiVersion => LabApi.Features.LabApiProperties.CurrentVersion;

    public static PluginMain Instance;

    public string StatsTrackerPath = string.Empty;

    public override void Enable()
    {
        Instance = this;
        PlayerEvents.InspectingKeycard += InspectingCard;
    }


    public override void Disable()
    {
        Instance = null;
        PlayerEvents.InspectingKeycard -= InspectingCard;
    }

    private void InspectingCard(LabApi.Events.Arguments.PlayerEvents.PlayerInspectingKeycardEventArgs ev)
    {
        if (ev.KeycardItem.Type == ItemType.KeycardChaosInsurgency)
            ev.IsAllowed = false;
        else
            ev.IsAllowed = true;
    }
}