using System;
using System.Collections.Generic;
using VRDR;
using Hl7.Fhir.Model;
using VR;

namespace canary.Models
{
    public abstract class Message
    {
        protected CommonMessage message { get; set; }

        private static String[] validateForPresence = new String[] {
            "MessageTimestamp",
            "MessageId",
            "MessageSource"
        };

        public int MessageId { get; set; }

        public Message() { }

        public Message(string message)
        {
            this.message = BaseMessage.Parse(message, false);
        }

        public Message(CommonMessage message)
        {
            this.message = message;
        }

        public CommonMessage GetMessage()
        {
            return message;
        }

        public string MessageType
        {
            get
            {
                return message.MessageType;
            }
        }

        public abstract Dictionary<string, Message> GetResponsesFor(String type);

        // Returns whether or not we should only validate presence for these fields and not their values
        public static Boolean validatePresenceOnly(string field)
        {
            return Array.Exists(validateForPresence, element => element == field);
        }

        public string Xml
        {
            get
            {
                if (message == null)
                {
                    return null;
                }
                return message.ToXML();
            }
        }

        public string Json
        {
            get
            {
                if (message == null)
                {
                    return null;
                }
                return message.ToJSON();
            }
        }
    }
}
