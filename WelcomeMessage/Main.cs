using Exiled.API.Features;
using Exiled.Loader;
using player = Exiled.Events.Handlers.Player;

namespace WelcomeMessage
{
    public class Main : Plugin<MainConfig>
    {
        public static Main Instance;
        public override void OnEnabled()
        {
            Instance = this;
            player.Verified += EventHandlers.Verified.onVerified;
            base.OnEnabled();
        }
        public override void OnDisabled()
        {
            Instance = null;
            player.Verified -= EventHandlers.Verified.onVerified;
            base.OnDisabled();
        }
    }
}
