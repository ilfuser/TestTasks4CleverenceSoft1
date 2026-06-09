namespace StaticCountApp;

class Program
{
    static async Task Main(string[] args)
    {
        int numOfReaders = 10;
        int numOfWriters = 3;

        List<Task> tasks = new();

        for (int i = 0; i < numOfReaders; i++)
        {            
            tasks.Add(CountReader.ReadCount());
        }
        
        for (int i = 0; i < numOfWriters; i++)
        {            
            tasks.Add(CountWriter.WriteCount());
        }

        await Task.WhenAll(tasks);

        StaticCount.Dispose();
    }
}
