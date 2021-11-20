using WebApiUebungen.Settings;

namespace WebApiUebungen.Settings
{
    public class AppSettings
    {
        public AuthenticationSettings Authentication { get; set; }

        public DatabaseSettings Database { get; set; }
    }
}
