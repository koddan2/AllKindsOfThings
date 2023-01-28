namespace Ncs.Model.Configuration.Keycloak
{
    public class KeycloakConfiguration
    {
        public string? Realm { get; set; }
        public string? Auth_Server_Url { get; set; }
        public string? Ssl_Required { get; set; }
        public string? Resource { get; set; }
        public KeycloakCredentials? Credentials { get; set; }
        public int? ConfidentialPort { get; set; }
    }
}
