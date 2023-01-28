using System.Text.Json.Serialization;

namespace Ncs.Model.Configuration
{
    public class KeycloakConfiguration
    {
        public string? Realm { get; set; }
        public string? Auth_Server_Url { get; set; }
        public string? Ssl_Required { get; set; }
        public string? Resource { get; set; }
        public Credentials? Credentials { get; set; }
        public int? ConfidentialPort { get; set; }
    }

    public class Credentials
    {
        public string? Secret { get; set; }
    }
}
