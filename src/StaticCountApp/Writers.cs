using StaticCountApp;

static public class CountWriter
{
    public static async Task WriteCount()
    {
        for(int i = 0; i < 10; i++)
        {
            await Task.Delay(Random.Shared.Next(10,100));
            StaticCount.AddToCount(Random.Shared.Next(1,10));
        }
    }
}