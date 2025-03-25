using Exiled.API.Interfaces;

namespace WelcomeMessage
{
    public class MainConfig : IConfig
    {
        public bool IsEnabled { get; set; }
        public bool Debug { get; set; }
    }
}
