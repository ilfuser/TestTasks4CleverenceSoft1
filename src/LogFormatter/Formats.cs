using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace LogFormatter;


public static class LogLevelMap
{
    // Словарь соответствия уровней логирования
    private static readonly Dictionary<string, string> _levelMap = new(StringComparer.OrdinalIgnoreCase)
    {
        { "INFO", "INFO" },
        { "WARN", "WARN" },
        { "ERROR", "ERROR" },
        { "DEBUG", "DEBUG" },
        { "INFORMATION", "INFO" },
        { "WARNING", "WARN" }
    };

    // Открытое поле только для чтения 
    public static IReadOnlyDictionary<string, string> LevelMap => _levelMap;
}



// Интерфейс для проверки формата
public interface ILogEntryNormalizer
{
    /// <summary>
    /// Пытается разобрать строку и привести к стандартному виду.
    /// </summary>
    /// <param name="line">Исходная строка лога.</param>
    /// <param name="normalizedLine">Нормализованная строка.
    /// <returns>true, если строка подошла под формат и успешно нормализована.</returns>
    bool TryNormalize(string line, out string normalizedLine);
}


// Формат 1: dd.MM.yyyy HH:mm:ss.fff LEVEL Message
// "10.03.2025 15:14:49.523 INFORMATION Версия программы: '3.4.0.48729'",
public class Format1Normalizer : ILogEntryNormalizer
{
    private static readonly Regex Regex = new(
        @"^(\d{2}\.\d{2}\.\d{4}) (\d{2}:\d{2}:\d{2}\.\d{3}) ([A-Za-z]+) (.*)$",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    public bool TryNormalize(string line, out string normalizedLine)
    {
        normalizedLine = "";

        var match = Regex.Match(line);

        if (!match.Success)
            return false;

        string date = match.Groups[1].Value;
        string time = match.Groups[2].Value;
        string level = match.Groups[3].Value;
        string message = match.Groups[4].Value.Trim();

        // Проверка корректности даты и времени
        if (!DateTime.TryParseExact($"{date} {time}", "dd.MM.yyyy HH:mm:ss.fff",
                CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
            return false;

        if (!LogLevelMap.LevelMap.TryGetValue(level, out string? shortLevel))
            return false;

        // Преобразование даты в DD-MM-YYYY
        string formattedDate = DateTime.ParseExact(date, "dd.MM.yyyy", CultureInfo.InvariantCulture)
            .ToString("dd-MM-yyyy");

        var compiler = new NormalizedCompiler();

        normalizedLine = compiler.CompileNormalized(
            date: formattedDate,
            time: time,
            level: shortLevel,            
            method: "DEFAULT",
            message: message
        );
        
        return true;
    }
}

// Формат 2: yyyy-MM-dd HH:mm:ss.ffff| LEVEL|Number|Method| Message
// "2025-03-10 15:14:51.5882| INFO|11|MobileComputer.GetDeviceId| Код устройства: '@MINDEO-M40-D-410244015546'"
public class Format2Normalizer : ILogEntryNormalizer
{
    public bool TryNormalize(string line, out string normalizedLine)
    {
        normalizedLine = "";

        if (!line.Contains('|'))
            return false;

        // Разбиваем максимум на 5 частей, чтобы сообщение могло содержать '|'
        string[] parts = line.Split(new[] { '|' }, 5);
        if (parts.Length != 5)
            return false;

        string dateTimePart = parts[0].Trim();
        string levelPart = parts[1].Trim();
        // parts[2] - в итоговый формат не пойдет
        string methodPart = parts[3].Trim();
        string messagePart = parts[4].Trim();

        string[] dtSplit = dateTimePart.Split(' ');
        if (dtSplit.Length != 2)
            return false;

        string date = dtSplit[0];
        string time = dtSplit[1];

        // Проверка даты и времени: yyyy-MM-dd HH:mm:ss.ffff (миллисекунды с 4 знаками)
        if (!DateTime.TryParseExact($"{date} {time}", "yyyy-MM-dd HH:mm:ss.ffff",
                CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
            return false;

        if (!LogLevelMap.LevelMap.TryGetValue(levelPart, out string? shortLevel))
            return false;

        string formattedDate = DateTime.ParseExact(date, "yyyy-MM-dd", CultureInfo.InvariantCulture)
            .ToString("dd-MM-yyyy");

        string caller = string.IsNullOrEmpty(methodPart) ? "DEFAULT" : methodPart;

        var compiler = new NormalizedCompiler();
                
        normalizedLine = compiler.CompileNormalized(
            date: formattedDate,
            time: time,
            level: shortLevel,            
            method: caller,
            message: messagePart
        );
        
        return true;        
    }
}