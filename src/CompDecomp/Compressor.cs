using System.Globalization;
using System.Text;

namespace CompDecomp;

// Статический класс для компрессии строк
static public class Compressor
{
    static public string Compress(string input)
    {
        // Тип для рациональной работы со строками - быстрой и экономичной по памяти 
        StringBuilder output = new StringBuilder(input.Length);

        for (int i = 0; i < input.Length; i++)
        {            
            int count = 1;            

            for (int j = i + 1; j < input.Length; j++)
            {
                // Сравниваем текущий символ с последующими
                if (input[i] != input[j])
                {
                    break;
                }                
                
                // Если символы совпадают, увеличиваем счетчик
                count++;      
            }

            // Добавляем символ
            output.Append(input[i]);
            
            if (count > 1)
            {
                // Добавляем число повторений, если есть
                output.Append(count);
                i += count - 1;
            }                        
        }

        return output.ToString();        
    }

    // Класс для декомпрессии сжатой строки
    static public string Decompress(string input)
    {
        StringBuilder output = new StringBuilder(input.Length * 10);

        for (int i = 0; i < input.Length; i++)
        {            
            if (char.IsDigit(input[i]))
            {
                int j = i;

                // Создаем аккумулятор символов цифр
                StringBuilder numString = new StringBuilder();
                numString.Append(input[i]);

                for(j = i + 1; j < input.Length; j++)
                {
                    if (char.IsDigit(input[j]))
                    {
                        numString.Append(input[j]);                        
                    }                        
                    else
                        break;
                }                                

                int num = int.Parse(numString.ToString());

                // Console.Write($"{input[i - 1]}\t");
                // Console.Write($"{numString.ToString()}\t");
                // Console.Write($"{num}\n");

                // Добавляем в выходную строку символ N раз, 
                output.Append(input[i - 1], num - 1);  
                
                i = j - 1;                
            }
            else
            {
                output.Append(input[i]);
            }
            
            //System.Console.WriteLine(output.ToString());
        }
        return output.ToString();        
    }
}