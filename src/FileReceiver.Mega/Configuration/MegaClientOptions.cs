namespace FileReceiver.Integrations.Mega.Configuration
{
    public class MegaClientOptions
    {
        public const string MegaClient = "MegaClient";

        public string ApplicationKey { get; set; }
        public bool SynchronizeApiRequests { get; set; }
        public int ApiRequestAttempts { get; set; }
        public int ApiRequestDelay { get; set; }
        public float ApiRequestDelayFactor { get; set; }
        public int BufferSize { get; set; }
        public int ChunksPackSize { get; set; }
        public long ReportProgressChunkSize { get; set; }

        public string Login { get; set; }
        public string Password { get; set; }
    }
}
