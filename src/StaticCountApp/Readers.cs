using StaticCountApp;

static public class CountReader
{
    public static async Task ReadCount()
    {
        for(int i = 0; i < 20; i++)
        {
            await Task.Delay(Random.Shared.Next(10,60));
            StaticCount.GetCount();
        }
    }
}