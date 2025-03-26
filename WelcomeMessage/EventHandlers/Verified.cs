using Exiled.Events.EventArgs.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WelcomeMessage.EventHandlers
{
    public static class Verified
    {
        public static void onVerified(VerifiedEventArgs ev)
        {
            ev.Player.ShowHint(Main.Instance.Config.WelcomeMessageDisplay, 8);
        }
    }
}
