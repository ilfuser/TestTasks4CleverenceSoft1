namespace LogFormatter;


// Интерфейс с методом для проверки формата
public interface ILogNormalizedCompiler
{
    /// <summary>
    /// Приводит проверенную строку к стандартному виду из распарсенных кусков
    /// </summary>
    /// <param name="date">Дата в нужном формате</param>    
    /// <param name="time">Время в нужном формате</param>    
    /// <param name="level">Уровень лога</param>    
    /// <param name="method">Вызывающий метод</param>    
    /// <param name="message">Сообщение</param>    
    /// <returns>Нормализованная строка</returns>
    public string CompileNormalized(string date, string time, string level, string method, string message);
}

public class NormalizedCompiler : ILogNormalizedCompiler
{
    public string CompileNormalized(string date, string time, string level, string method, string message)
    {
        return  $"{date}\t{time}\t{level}\tDEFAULT\t{message}";
    }
}