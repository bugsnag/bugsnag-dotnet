namespace Bugsnag.Event
{
    public abstract class Event
    {
        public string GroupingHash { get; set; }
        public Severity Severity { get; set; }
        public MetaData MetaData { get; private set; }

        protected Event()
        {
            MetaData = new MetaData();
        }
    }
}
