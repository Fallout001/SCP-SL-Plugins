namespace CalamityStatsTracker;

public static class RoundStatsTracker
{
    public static List<BaseStatEvent> CurrentRoundStats = [];

    public static DateTime roundStartTime = DateTime.Now;

    internal static int roundNumber = -1;

    public static int RoundNumber
    {
        get 
        { 
            if (roundNumber == -1)
                roundNumber = LoadRoundNumber();
            return roundNumber;
        }
        set 
        {
            roundNumber = value;
        }
    }

    private static readonly string CurrentRoundNumberPath = Path.Combine(PluginMain.Instance.StatsTrackerPath, "CurrentRoundNumber.txt");

    internal static int LoadRoundNumber()
    {
        try
        {
            if (File.Exists(CurrentRoundNumberPath))
            {
                var text = File.ReadAllText(CurrentRoundNumberPath);
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

    internal static void SaveRoundNumber()
    {
        try
        {
            File.WriteAllText(CurrentRoundNumberPath, RoundNumber.ToString());
        }
        catch (Exception ex)
        {
            CL.Error($"Error saving round number: {ex}");
        }
    }

    public static void SaveCurrentRoundStats()
    {
        string filePath = Path.Combine(PluginMain.Instance.StatsTrackerPath, "AllRoundsStats.csv");
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

 
