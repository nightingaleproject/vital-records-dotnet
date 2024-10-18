using VRDR.Interfaces;
using VR;
using VRDR;
using Hl7.Fhir.Utility;
using canary.Models;

namespace VRDR.Factory
{
    public static class VRDRMessaging
    {

        public static ICanaryDeathMessage GetCanaryDeathMessage(ICommonMessage message, string version)
        {
            if (String.Equals(version, "1.0"))
            {
                return new CanaryDeathMessage(message);
            }
            else
            {
                return null;
            }
        }

        public static ICommonMessage ParseBasemessage(string input, bool permissive, string version) 
        {
            if (String.Equals(version, "1.0"))
            {
                return BaseMessage.Parse(input, permissive);
            }
            else
            {
                return null;
            }
        }

    }
}