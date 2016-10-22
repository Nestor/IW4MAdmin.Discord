using System;
using Newtonsoft.Json;

namespace IW4MAdmin.Discord.Query
{
    [Serializable]
    public struct RestEvent
    {
        public RestEvent(eType Ty, eVersion V, string M, string T, string O, string Ta)
        {
            Type = Ty;
            Version = V;
            Message = M;
            Title = T;
            Origin = O;
            Target = Ta;

            ID = Math.Abs(DateTime.Now.GetHashCode());
        }

        public enum eType
        {
            NOTIFICATION,
            STATUS,
            ALERT,
        }

        public enum eVersion
        {
            IW4MAdmin
        }

        public eType Type;
        public eVersion Version;
        public string Message;
        public string Title;
        public string Origin;
        public string Target;
        public int ID;
    }

    [Serializable]
    public struct RestAPIEvent
    {
        public RestAPIEvent(int eventCount, RestEvent Event)
        {
            this.eventCount = eventCount;
            this.Event = Event;
        }

        public int eventCount;
        public RestEvent Event;
    }

    public class EventDeserialize
    {
        public static RestEvent deserializeEvent(string data)
        {
            if (data == null || data.Length == 0)
                throw new NullReferenceException("Bad object from event API");

            var e = JsonConvert.DeserializeObject<RestAPIEvent>(data);

            if (e.eventCount == 0)
                throw new NullReferenceException("No events in event object");

            return e.Event;
        }
    }
}
