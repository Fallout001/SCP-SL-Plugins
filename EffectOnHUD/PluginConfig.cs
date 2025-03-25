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
        public string DisplayHeader { get; set; } = "Your effects: \n";
        public string EffectDisplayColor { get; set; }
        public string EffectDisplayPosition { get; set; }
    }
}
