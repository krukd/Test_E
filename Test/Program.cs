using System;

using System.Collections.Generic;

using System.IO;

using System.Linq;


namespace Test
{
    class Program

    {

        static void Main(string[] args)

        {

            // Проверяем количество аргументов командной строки

            if (args.Length < 2)

            {

                Console.WriteLine("Usage: [--file-log <path>] [--file-output <path>] [--address-start <IP>] [--address-mask <subnet mask>]");

                return;

            }



            // Инициализируем параметры из командной строки

            string logFilePath = null;

            string outputFilePath = null;

            string addressStart = null;

            string addressMask = null;



            // Парсим аргументы командной строки

            for (int i = 0; i < args.Length; i++)

            {

                switch (args[i])

                {

                    case "--file-log":

                        logFilePath = args[++i];

                        break;

                    case "--file-output":

                        outputFilePath = args[++i];

                        break;

                    case "--address-start":

                        addressStart = args[++i];

                        break;

                    case "--address-mask":

                        addressMask = args[++i];

                        break;

                    default:

                        Console.WriteLine($"Unknown argument: {args[i]}");

                        return;

                }

            }



            // Проверяем наличие необходимых параметров

            if (logFilePath == null || outputFilePath == null)

            {

                Console.WriteLine("Missing required parameters");

                return;

            }



            try

            {

                // Читаем файл логов

                string[] lines = File.ReadAllLines(logFilePath);



                // Фильтруем строки по указанным адресам

                IEnumerable<string> filteredLines = lines;

                if (addressStart != null)

                {

                    // Применяем маску подсети, если она указана

                    if (addressMask != null)

                    {

                        // Находим адреса, соответствующие маске

                        string[] addressParts = addressStart.Split('.');

                        string[] maskParts = addressMask.Split('.');

                        var maskedAddress = addressParts.Zip(maskParts, (a, m) => (Address: a, Mask: m))

                                                         .Select(pair => int.Parse(pair.Address) & int.Parse(pair.Mask))

                                                         .Select(part => part.ToString());



                        string maskedAddressStart = string.Join(".", maskedAddress);

                        filteredLines = filteredLines.Where(line => line.StartsWith(maskedAddressStart));

                    }

                    else

                    {

                        filteredLines = filteredLines.Where(line => line.StartsWith(addressStart));

                    }

                }



                // Считаем количество обращений с каждого адреса

                var addressCounts = new Dictionary<string, int>();

                foreach (string line in filteredLines)

                {

                    string ipAddress = line.Split(' ')[0];

                    if (addressCounts.ContainsKey(ipAddress))

                    {

                        addressCounts[ipAddress]++;

                    }

                    else

                    {

                        addressCounts[ipAddress] = 1;

                    }

                }



                // Записываем результаты в выходной файл

                using (StreamWriter writer = new StreamWriter(outputFilePath))

                {

                    foreach (var kvp in addressCounts)

                    {

                        writer.WriteLine($"{kvp.Key}: {kvp.Value} обращений");

                    }

                }



                Console.WriteLine("Готово!");

            }

            catch (Exception ex)

            {

                Console.WriteLine($"Ошибка: {ex.Message}");

            }

        }

    }

}
