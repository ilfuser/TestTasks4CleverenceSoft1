namespace StaticCountApp;

static class StaticCount
{
    private readonly static ReaderWriterLockSlim _locker = new();

    private static int count;

    public static int GetCount()
    {
        _locker.EnterReadLock();
        try
        {
            Console.WriteLine($"Поток {Thread.CurrentThread.ManagedThreadId} прочитал count: {count}");
            return count;
        }
        finally
        {
            _locker.ExitReadLock();
        }
    }

    public static void AddToCount(int value)
    {
        _locker.EnterUpgradeableReadLock();
        try
        {            
            int newValue = count + value;
            
            _locker.EnterWriteLock();
            try
            {                
                count = newValue;
                Console.WriteLine($"Поток {Thread.CurrentThread.ManagedThreadId} обновил count: {count - value} -> {newValue}");
            }
            finally
            {                
                _locker.ExitWriteLock();
            }
        }
        finally
        {
            _locker.ExitUpgradeableReadLock();
        }
    }

    public static void Dispose()
    {
        _locker.Dispose();
    }
}