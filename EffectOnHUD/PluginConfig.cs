using Exiled.API.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EffectOnHUD
{
    public class PluginConfig : IConfig
    {
        public bool IsEnabled { get; set; } = true;
        public bool Debug { get; set; } = false;


        public int EffectDisplayDuration { get; set; } = 5;
        public string DisplayHeader { get; set; } = "Your effects:";
        public string EffectDisplayColorGood { get; set; } = "#228B22";
        public string EffectDisplayColorBad { get; set; } = "#C50000";
        public string EffectDisplayColorMixed { get; set; } = "#8137CE";
        public string EffectDisplayAlignment { get; set; } = "left";
        public int EffectDisplayFontSize { get; set; } = 20;
    }
}
