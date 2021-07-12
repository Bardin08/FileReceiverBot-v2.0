namespace FileReceiver.Dal.Entities.Enums
{
    public enum TransactionStateDb
    {
        Active = 0,
        Failed = 1,
        Aborted = 2,
        Committed = 100,
    }
}
