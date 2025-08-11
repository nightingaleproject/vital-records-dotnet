using Newtonsoft.Json.Linq;
using static System.Formats.Asn1.AsnWriter;

namespace VR
{
    //
    // Summary:
    //     The Credentials class includes the values required to authenticate to the API
    //     Gateway
    public class Credentials
    {
        //
        // Summary:
        //     The oauth authentication url
        public string Url { get; }

        //
        // Summary:
        //     The client ID provided by SAMS
        public string ClientId { get; }

        //
        // Summary:
        //     The client secret provided by SAMS
        public string ClientSecret { get; }

        //
        // Summary:
        //     The username provided by SAMS
        public string Username { get; }

        //
        // Summary:
        //     The password provided by SAMS
        public string Pass { get; }

        //
        // Summary:
        //     The scope of the request
        public string? Scope { get; }

        //
        // Summary:
        //     Constructor
        public Credentials(string url, string clientID, string clientSecret, string username, string pass, string? scope = null)
        {
            Url = url;
            ClientId = clientID;
            ClientSecret = clientSecret;
            Username = username;
            Pass = pass;
            Scope = scope;
        }
    }
}