using Exiled.API.Features;
using Exiled.Loader;
using PluginAPI.Core;
using player = Exiled.Events.Handlers.Player;
using Exiled.CreditTags.Commands;
using Exiled.CreditTags.Features;
using Exiled.API.Extensions;
using Exiled.Loader.Features;
using Exiled.API.Interfaces;
using CommandSystem.Commands.RemoteAdmin;
using Exiled.Loader.Models;
using System;


namespace PopulationXpLimiter
{

    public class PluginMain : Plugin<PluginConfig>
    {
        public override string Author => "Fallout001";
        public override string Name => "Population_XP_Limiter";
        public override string Prefix => Name;
        public override Version Version => new Version(1, 0, 2);
        public override Version RequiredExiledVersion => new Version(8, 12, 2);

        public static PluginMain Instance;
        public override void OnEnabled()
        {
            Instance = this;
            Exiled.Events.Handlers.Server.RoundStarted += EventHandlers.Events.OnRoundStart;
            PluginAPI.Core.Log.Debug("XP Limiter Plugin Enabled");
            base.OnEnabled();
        }
        public override void OnDisabled()
        {
            Instance = null;
            Exiled.Events.Handlers.Server.RoundStarted -= EventHandlers.Events.OnRoundStart;
            PluginAPI.Core.Log.Debug("XP Limiter Plugin Disabled");
            base.OnDisabled();
        }
    }
   
}
