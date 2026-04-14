using System.Linq;

namespace Lab6Lib
{
    public class Publisher
    {
        public delegate void EventRaisedHandler(string eventname);
        public event EventRaisedHandler eventRaised;
        public string eventName;

        public Publisher(string eventName)
        {
            this.eventName = eventName;
        }

        public void subscribe(ISubscriber subscriber)
        {
            eventRaised += subscriber.update;
        }

        public bool unsubscribe(ISubscriber subscriber)
        {
            eventRaised -= subscriber.update;
            return true;
        }

        public int notify()
        {
            EventRaisedHandler handler = eventRaised;

            if (handler != null)
            {
                handler(eventName);
                return eventRaised.GetInvocationList().Count();
            }

            return 0;
        }
    }
}
