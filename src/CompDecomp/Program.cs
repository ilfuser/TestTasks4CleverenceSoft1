namespace CompDecomp;

class Program
{
    static void Main(string[] args)
    {
        string defaultToCompress = "aaabbcccdde";
        string defaultToDecompress = "a3b2c3d2e";
        // f2f1c2b1a
        // g100

        // Компрессор
        System.Console.WriteLine("Введите строку для компрессии (только из буквы или символы, без цифр): ");
        string? inputToCompress = Console.ReadLine();
        
        if (string.IsNullOrEmpty(inputToCompress))
        {
            System.Console.WriteLine($"Введена пустая строка, будет использовано значение по умолчанию: {defaultToCompress}");            
            inputToCompress = defaultToCompress;
        }
            
        
        // Проверка компрессора
        string compressed = Compressor.Compress(inputToCompress);
        System.Console.WriteLine("Сжатая строка: " + compressed);
        
        // Декомпрессор
        System.Console.WriteLine("Введите строку для декомпрессии: ");
        string? inputToDecompress = Console.ReadLine();
        
        if (string.IsNullOrEmpty(inputToDecompress))
        {
            System.Console.WriteLine($"Введена пустая строка, будет использовано значение по умолчанию: {defaultToDecompress}");
            inputToDecompress = defaultToDecompress;
        }        
        
        // Проверка декомпрессора
        string decompressed = Compressor.Decompress(inputToDecompress);
        System.Console.WriteLine("Строка после декомпрессии: " + decompressed);
    }
}
