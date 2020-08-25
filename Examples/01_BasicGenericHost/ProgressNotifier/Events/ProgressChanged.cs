namespace ProgressNotifier.Events
{
    public class ProgressChanged
    {
        public int ProcessId { get; set; }

        public int Value { get; set; }
    }
}
