using Exiled.API.Interfaces;

namespace WelcomeMessage
{
    public class MainConfig : IConfig
    {
        public bool IsEnabled { get; set; }
        public bool Debug { get; set; }
        public string WelcomeMessageDisplay { get; set; } = "Welcome to the server!";

        public int WelcomeMessageDisplayDuration { get; set; } = 5;
    }
}
