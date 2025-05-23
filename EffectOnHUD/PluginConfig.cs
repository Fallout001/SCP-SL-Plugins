using LabApi.Events;
using LabApi;

namespace EffectOnHUD
{
    public class PluginConfig
    {
        public bool Debug { get; set; } = false;

        public int EffectDisplayDuration { get; set; } = 5;
        public string EffectDisplayColorGood { get; set; } = "#228B22";
        public string EffectDisplayColorBad { get; set; } = "#C50000";
        public string EffectDisplayColorMixed { get; set; } = "#8137CE";
        public string EffectDisplayAlignment { get; set; } = "left";
        public float DefaultTextSize { get; set; } = 20;
        public float MinTextSize { get; set; } = 10;
        public float MaxTextSize { get; set; } = 50;
    }
}
