using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using System.Xml.Serialization;
using YamlDotNet.Serialization;
using CsvHelper;
using System.Xml;
using System.Formats.Asn1;

namespace CurrencyManager
{
    public class CurrencyRate
    {
        public int Id { get; set; }
        public string Currency { get; set; }
        public decimal Rate { get; set; }
        public DateTime UpdateDate { get; set; }
    }

    public interface IFileHandler<T>
    {
        T ReadFromFile(string fileName);
        void WriteToFile(T data, string fileName);
    }

    public class CsvHandler : IFileHandler<List<CurrencyRate>>
    {
        public List<CurrencyRate> ReadFromFile(string fileName)
        {
            try
            {
                using (var reader = new StreamReader(fileName))
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    return csv.GetRecords<CurrencyRate>().ToList();
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.LogError(ex);
                throw;
            }
        }

        public void WriteToFile(List<CurrencyRate> data, string fileName)
        {
            try
            {
                using (var writer = new StreamWriter(fileName))
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    csv.WriteRecords(data);
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.LogError(ex);
                throw;
            }
        }
    }

    public class JsonHandler : IFileHandler<List<CurrencyRate>>
    {
        public List<CurrencyRate> ReadFromFile(string fileName)
        {
            try
            {
                var json = File.ReadAllText(fileName);
                return JsonConvert.DeserializeObject<List<CurrencyRate>>(json);
            }
            catch (Exception ex)
            {
                ErrorHandler.LogError(ex);
                throw;
            }
        }

        public void WriteToFile(List<CurrencyRate> data, string fileName)
        {
            try
            {
                var json = JsonConvert.SerializeObject(data, Newtonsoft.Json.Formatting.Indented);
                File.WriteAllText(fileName, json);
            }
            catch (Exception ex)
            {
                ErrorHandler.LogError(ex);
                throw;
            }
        }
    }

    public class XmlHandler : IFileHandler<List<CurrencyRate>>
    {
        public List<CurrencyRate> ReadFromFile(string fileName)
        {
            try
            {
                var serializer = new XmlSerializer(typeof(List<CurrencyRate>));
                using (var reader = new StreamReader(fileName))
                {
                    return (List<CurrencyRate>)serializer.Deserialize(reader);
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.LogError(ex);
                throw;
            }
        }

        public void WriteToFile(List<CurrencyRate> data, string fileName)
        {
            try
            {
                var serializer = new XmlSerializer(typeof(List<CurrencyRate>));
                using (var writer = new StreamWriter(fileName))
                {
                    serializer.Serialize(writer, data);
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.LogError(ex);
                throw;
            }
        }
    }

    public class YamlHandler : IFileHandler<List<CurrencyRate>>
    {
        public List<CurrencyRate> ReadFromFile(string fileName)
        {
            try
            {
                var yaml = new Deserializer();
                var text = File.ReadAllText(fileName);
                return yaml.Deserialize<List<CurrencyRate>>(text);
            }
            catch (Exception ex)
            {
                ErrorHandler.LogError(ex);
                throw;
            }
        }

        public void WriteToFile(List<CurrencyRate> data, string fileName)
        {
            try
            {
                var yaml = new Serializer();
                var text = yaml.Serialize(data);
                File.WriteAllText(fileName, text);
            }
            catch (Exception ex)
            {
                ErrorHandler.LogError(ex);
                throw;
            }
        }
    }

    public static class ErrorHandler
    {
        public static void LogError(Exception ex)
        {
            string logMessage = $"[{DateTime.Now}] Ошибка: {ex.Message}\n{ex.StackTrace}\n";
            File.AppendAllText("error_log.txt", logMessage);
        }
    }

    public static class UserInterface
    {
        public static void DisplayMenu()
        {
            Console.WriteLine("1. Считать данные из файла");
            Console.WriteLine("2. Записать данные в файл");
            Console.WriteLine("3. Вывести данные на экран");
            Console.WriteLine("4. Сортировать данные");
            Console.WriteLine("5. Поиск данных");
            Console.WriteLine("6. Добавить данные");
            Console.WriteLine("7. Удалить данные");
            Console.WriteLine("8. Изменить данные");
            Console.WriteLine("9. Выход");
        }

        public static void DisplayData(List<CurrencyRate> rates)
        {
            foreach (var rate in rates)
            {
                Console.WriteLine($"ID: {rate.Id}, Валюта: {rate.Currency}, Курс: {rate.Rate}, Дата: {rate.UpdateDate:yyyy-MM-dd}");
            }
        }

        public static List<CurrencyRate> SortData(List<CurrencyRate> rates, string sortBy)
        {
            switch (sortBy)
            {
                case "Currency":
                    return rates.OrderBy(r => r.Currency).ToList();
                case "Rate":
                    return rates.OrderBy(r => r.Rate).ToList();
                case "UpdateDate":
                    return rates.OrderBy(r => r.UpdateDate).ToList();
                default:
                    return rates;
            }
        }

        public static List<CurrencyRate> SearchData(List<CurrencyRate> rates, string searchTerm)
        {
            return rates.Where(r =>
                r.Currency.Contains(searchTerm) ||
                r.Rate.ToString().Contains(searchTerm) ||
                r.UpdateDate.ToString("yyyy-MM-dd").Contains(searchTerm)
            ).ToList();
        }
    }

    class CurrencyManager
    {
        static List<CurrencyRate> currencyRates = new List<CurrencyRate>();

        static void Main(string[] args)
        {
            bool isRunning = true;

            while (isRunning)
            {
                UserInterface.DisplayMenu();
                Console.Write("Выберите опцию: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        ReadDataFromFile();
                        break;
                    case "2":
                        WriteDataToFile();
                        break;
                    case "3":
                        UserInterface.DisplayData(currencyRates);
                        break;
                    case "4":
                        SortData();
                        break;
                    case "5":
                        SearchData();
                        break;
                    case "6":
                        AddData();
                        break;
                    case "7":
                        RemoveData();
                        break;
                    case "8":
                        UpdateData();
                        break;
                    case "9":
                        isRunning = false;
                        break;
                    default:
                        Console.WriteLine("Неверный выбор. Попробуйте снова.");
                        break;
                }
            }
        }

        static void ReadDataFromFile()
        {
            Console.Write("Введите имя файла: ");
            string fileName = Console.ReadLine();
            Console.Write("Выберите формат (CSV, JSON, XML, YAML): ");
            string format = Console.ReadLine().ToLower();

            IFileHandler<List<CurrencyRate>> handler = format switch
            {
                "csv" => new CsvHandler(),
                "json" => new JsonHandler(),
                "xml" => new XmlHandler(),
                "yaml" => new YamlHandler(),
                _ => throw new ArgumentException("Неверный формат файла")
            };

            try
            {
                currencyRates = handler.ReadFromFile(fileName);
                Console.WriteLine("Данные успешно загружены.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }

        static void WriteDataToFile()
        {
            Console.Write("Введите имя файла: ");
            string fileName = Console.ReadLine();
            Console.Write("Выберите формат (CSV, JSON, XML, YAML): ");
            string format = Console.ReadLine().ToLower();

            IFileHandler<List<CurrencyRate>> handler = format switch
            {
                "csv" => new CsvHandler(),
                "json" => new JsonHandler(),
                "xml" => new XmlHandler(),
                "yaml" => new YamlHandler(),
                _ => throw new ArgumentException("Неверный формат файла")
            };

            try
            {
                handler.WriteToFile(currencyRates, fileName);
                Console.WriteLine("Данные успешно сохранены.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }

        static void SortData()
        {
            Console.Write("Введите поле для сортировки (Currency, Rate, UpdateDate): ");
            string sortBy = Console.ReadLine();
            currencyRates = UserInterface.SortData(currencyRates, sortBy);
            Console.WriteLine("Данные отсортированы.");
        }

        static void SearchData()
        {
            Console.Write("Введите строку для поиска: ");
            string searchTerm = Console.ReadLine();
            var results = UserInterface.SearchData(currencyRates, searchTerm);
            UserInterface.DisplayData(results);
        }

        static void AddData()
        {
            var rate = new CurrencyRate();
            Console.Write("Введите ID: ");
            rate.Id = int.Parse(Console.ReadLine());
            Console.Write("Введите название валюты: ");
            rate.Currency = Console.ReadLine();
            Console.Write("Введите курс: ");
            rate.Rate = decimal.Parse(Console.ReadLine());
            rate.UpdateDate = DateTime.Now;

            currencyRates.Add(rate);
            Console.WriteLine("Данные добавлены.");
        }

        static void RemoveData()
        {
            Console.Write("Введите ID для удаления: ");
            int id = int.Parse(Console.ReadLine());
            var rate = currencyRates.FirstOrDefault(r => r.Id == id);

            if (rate != null)
            {
                currencyRates.Remove(rate);
                Console.WriteLine("Данные удалены.");
            }
            else
            {
                Console.WriteLine("Запись не найдена.");
            }
        }

        static void UpdateData()
        {
            Console.Write("Введите ID для изменения: ");
            int id = int.Parse(Console.ReadLine());
            var rate = currencyRates.FirstOrDefault(r => r.Id == id);

            if (rate != null)
            {
                Console.Write("Введите новое название валюты: ");
                rate.Currency = Console.ReadLine();
                Console.Write("Введите новый курс: ");
                rate.Rate = decimal.Parse(Console.ReadLine());
                rate.UpdateDate = DateTime.Now;
                Console.WriteLine("Данные обновлены.");
            }
            else
            {
                Console.WriteLine("Запись не найдена.");
            }
        }
    }
}
