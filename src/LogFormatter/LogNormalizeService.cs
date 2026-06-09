using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;


namespace LogFormatter;

// Сервис нормализации с внедрением списка форматов
public class LogNormalizerService
{
    private readonly List<ILogEntryNormalizer> _normalizers;

    public LogNormalizerService(List<ILogEntryNormalizer> normalizers)
    {
        if (normalizers is null)
            throw new ArgumentNullException(nameof(normalizers));

        _normalizers = normalizers;
    }

    /// <summary>
    /// Обрабатывает последовательность строк и разделяет их на нормализованные и проблемные.
    /// </summary>
    public (List<string> normalized, List<string> problems) Process(IEnumerable<string> lines)
    {
        var normalized = new List<string>();
        var problems = new List<string>();

        foreach (string line in lines)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                problems.Add(line);
                continue;
            }

            bool handled = false;
            foreach (var normalizer in _normalizers)
            {
                if (normalizer.TryNormalize(line, out string? normalizedLine))
                {
                    normalized.Add(normalizedLine);
                    handled = true;
                    break;
                }
            }

            if (!handled)
                problems.Add(line);
        }

        return (normalized, problems);
    }
}

// Статический класс для операций с файлами
public static class FileService
{
    public static IEnumerable<string> ReadLines(string filePath, Encoding? encoding = null)
    {
        encoding ??= Encoding.UTF8;
        return File.ReadLines(filePath, encoding);
    }

    public static void WriteLines(string filePath, List<string> lines, Encoding? encoding = null)
    {
        encoding ??= new UTF8Encoding(false); // без BOM
        File.WriteAllLines(filePath, lines, encoding);
    }
}

// Точка входа
public static class LogNormalizerStarter
{
    public static void Run(string? input = null, string? output = null, string? problemsFile = null)
    {
        // Обработка аргументов
        string inputFilePath = !string.IsNullOrEmpty(input) ? input : @"logs\input.log";
        string outputFilePath = !string.IsNullOrEmpty(output) ? output : @"logs\output.log";
        string problemsFilePath = !string.IsNullOrEmpty(problemsFile) ? problemsFile : @"logs\problems.txt";

        Console.WriteLine("*********************");
        Console.WriteLine("Введенные пути к файлам логов:");
        Console.WriteLine($"Исходный лог: {inputFilePath}");
        Console.WriteLine($"Лог для отформатированных строк: {outputFilePath}");
        Console.WriteLine($"Лог для невалидных строк: {problemsFilePath}");

        if (!File.Exists(inputFilePath))
        {
            Console.WriteLine($"Ошибка: файл '{inputFilePath}' не найден");
            return;
        }

        try
        {
            // Настройка доступных форматов
            var normalizers = new List<ILogEntryNormalizer>
            {
                new Format1Normalizer(),
                new Format2Normalizer()                
            };

            // Сервис сопоставления с форматами
            var service = new LogNormalizerService(normalizers);

            // Чтение из файла
            var lines = FileService.ReadLines(inputFilePath);

            // Сопоставление с форматами, валидация, обработка валидных строк
            var (normalized, problems) = service.Process(lines);

            // Запись результатов        
            FileService.WriteLines(outputFilePath, normalized);
            FileService.WriteLines(problemsFilePath, problems);

            Console.WriteLine("*********************");
            Console.WriteLine("Обработка завершена!");
            Console.WriteLine($"Успешно обработано строк: {normalized.Count}");
            Console.WriteLine($"Невалидных строк: {problems.Count}");
            Console.WriteLine($"Результат сохранен в {outputFilePath}");
            Console.WriteLine($"Строки неизвестного формата записаны в {problemsFilePath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка: {ex.Message}");
        }
    }
}