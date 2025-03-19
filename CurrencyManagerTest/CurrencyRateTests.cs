using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Xunit;
using Newtonsoft.Json;
using System.Xml.Serialization;
using YamlDotNet.Serialization;
using CsvHelper;
using CurrencyManager;
using YamlDotNet.Core;

namespace CurrencyRateTests
{
    public class CurrencyRateTests
    {
        [Test]
        public void CurrencyRate_Constructor_CreatesObjectWithCorrectData()
        {
            
            int id = 1;
            string currency = "USD";
            decimal rate = 1.1m;
            DateTime updateDate = DateTime.Now;

            var currencyRate = new CurrencyRate()
            {
                Id = id,
                Currency = currency,
                Rate = rate,
                UpdateDate = updateDate
            };

            Xunit.Assert.Equal(id, currencyRate.Id);
            Xunit.Assert.Equal(currency, currencyRate.Currency);
            Xunit.Assert.Equal(rate, currencyRate.Rate);
            Xunit.Assert.Equal(updateDate, currencyRate.UpdateDate);
        }

        [Test]
        public void CurrencyRate_Properties_GetAndSetValuesCorrectly()
        {
           
            var currencyRate = new CurrencyRate();
 
            currencyRate.Id = 2;
            currencyRate.Currency = "EUR";
            currencyRate.Rate = 0.9m;
            currencyRate.UpdateDate = DateTime.Now.AddDays(-1);

            Xunit.Assert.Equal(2, currencyRate.Id);
            Xunit.Assert.Equal("EUR", currencyRate.Currency);
            Xunit.Assert.Equal(0.9m, currencyRate.Rate);
            Xunit.Assert.Equal(DateTime.Now.AddDays(-1).Date, currencyRate.UpdateDate.Date);
        }
    }

    public class CsvHandlerTests
    {
        [Test]
        public void ReadFromFile_ValidCsvFile_ReturnsListOfCurrencyRates()
        {  
            string fileName = "test.csv";
            string csvContent = "Id,Currency,Rate,UpdateDate\n1,USD,1.1,2023-05-01\n2,EUR,0.9,2023-05-02";
            File.WriteAllText(fileName, csvContent);

            var csvHandler = new CsvHandler();

            var result = csvHandler.ReadFromFile(fileName);

            Xunit.Assert.Equal(2, result.Count);
            Xunit.Assert.Equal(1, result[0].Id);
            Xunit.Assert.Equal("USD", result[0].Currency);
            Xunit.Assert.Equal(1.1m, result[0].Rate);
            Xunit.Assert.Equal(new DateTime(2023, 5, 1), result[0].UpdateDate);
        }

        [Test]
        public void WriteToFile_ListOfCurrencyRates_WritesDataToCsvFile()
        {
            
            string fileName = "test.csv";
            var currencyRates = new List<CurrencyRate>
            {
                new CurrencyRate { Id = 1, Currency = "USD", Rate = 1.1m, UpdateDate = new DateTime(2023, 5, 1) },
                new CurrencyRate { Id = 2, Currency = "EUR", Rate = 0.9m, UpdateDate = new DateTime(2023, 5, 2) }
            };

            var csvHandler = new CsvHandler();

            
            csvHandler.WriteToFile(currencyRates, fileName);

            
            Xunit.Assert.True(File.Exists(fileName));
            string expectedContent = "Id,Currency,Rate,UpdateDate\r\n1,USD,1.1,05/01/2023 00:00:00\r\n2,EUR,0.9,05/02/2023 00:00:00\r\n";
            string actualContent = File.ReadAllText(fileName);
            Xunit.Assert.Equal(expectedContent, actualContent);
        }
        [Test]
        public void ReadFromFile_NonExistingFile_ThrowsFileNotFoundException()
        {
            var fileName = "non_existing_file.csv";
            var csvHandler = new CsvHandler();

            Xunit.Assert.Throws<FileNotFoundException>(() => csvHandler.ReadFromFile(fileName));
        }

        [Test]
        public void ReadFromFile_InvalidJsonFormat_ThrowsJsonReaderException()
        {
            var fileName = "invalid_json.csv";
            File.WriteAllText(fileName, "This is not a valid CSV string.");
            var csvHandler = new CsvHandler();

            Xunit.Assert.ThrowsAny<Exception>(() => csvHandler.ReadFromFile(fileName));
        }
    }

    public class JsonHandlerTests
    {
        [Test]
        public void ReadFromFile_ValidJsonFile_ReturnsListOfCurrencyRates()
        {
            
            string fileName = "test.json";
            string jsonContent = "[{\"Id\":1,\"Currency\":\"USD\",\"Rate\":1.1,\"UpdateDate\":\"2023-05-01T00:00:00\"},{\"Id\":2,\"Currency\":\"EUR\",\"Rate\":0.9,\"UpdateDate\":\"2023-05-02T00:00:00\"}]";
            File.WriteAllText(fileName, jsonContent);

            var jsonHandler = new JsonHandler();

            
            var result = jsonHandler.ReadFromFile(fileName);

            
            Xunit.Assert.Equal(2, result.Count);
            Xunit.Assert.Equal(1, result[0].Id);
            Xunit.Assert.Equal("USD", result[0].Currency);
            Xunit.Assert.Equal(1.1m, result[0].Rate);
            Xunit.Assert.Equal(new DateTime(2023, 5, 1), result[0].UpdateDate);
        }

        [Test]
        public void WriteToFile_ListOfCurrencyRates_WritesDataToJSONFile()
        {
             
            string fileName = "test.json";
            var currencyRates = new List<CurrencyRate>
            {
                new CurrencyRate { Id = 1, Currency = "USD", Rate = 1.1m, UpdateDate = new DateTime(2023, 5, 1) },
                new CurrencyRate { Id = 2, Currency = "EUR", Rate = 0.9m, UpdateDate = new DateTime(2023, 5, 2) }
            };

            var jsonHandler = new JsonHandler();

           
            jsonHandler.WriteToFile(currencyRates, fileName);

            
            Xunit.Assert.True(File.Exists(fileName));
            string expectedContent = $"[\r\n  {{\r\n    \"Id\": 1,\r\n    \"Currency\": \"USD\",\r\n    \"Rate\": 1.1,\r\n    \"UpdateDate\": \"2023-05-01T00:00:00\"\r\n  }},\r\n  {{\r\n    \"Id\": 2,\r\n    \"Currency\": \"EUR\",\r\n    \"Rate\": 0.9,\r\n    \"UpdateDate\": \"2023-05-02T00:00:00\"\r\n  }}\r\n]";
            string actualContent = File.ReadAllText(fileName);

            Xunit.Assert.Equal(expectedContent, actualContent);
        }
        [Test]
        public void ReadFromFile_NonExistingFile_ThrowsFileNotFoundException()
        {
            var fileName = "non_existing_file.json";
            var jsonHandler = new JsonHandler();

            Xunit.Assert.Throws<FileNotFoundException>(() => jsonHandler.ReadFromFile(fileName));
        }

        [Test]
        public void ReadFromFile_InvalidJsonFormat_ThrowsJsonReaderException()
        {
            var fileName = "invalid_json.json";
            File.WriteAllText(fileName, "This is not a valid JSON string.");
            var jsonHandler = new JsonHandler();

            Xunit.Assert.Throws<JsonReaderException>(() => jsonHandler.ReadFromFile(fileName));
        }
    }

    public class XmlHandlerTests
    {
        [Test]
        public void ReadFromFile_ValidXmlFile_ReturnsListOfCurrencyRates()
        {
            
            string fileName = "test.xml";
            string xmlContent = @"<?xml version=""1.0"" encoding=""utf-8""?><ArrayOfCurrencyRate xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema""><CurrencyRate><Id>1</Id><Currency>USD</Currency><Rate>1.1</Rate><UpdateDate>2023-05-01T00:00:00</UpdateDate></CurrencyRate><CurrencyRate><Id>2</Id><Currency>EUR</Currency><Rate>0.9</Rate><UpdateDate>2023-05-02T00:00:00</UpdateDate></CurrencyRate></ArrayOfCurrencyRate>";
            File.WriteAllText(fileName, xmlContent);

            var xmlHandler = new XmlHandler();

            
            var result = xmlHandler.ReadFromFile(fileName);

            
            Xunit.Assert.Equal(2, result.Count);
            Xunit.Assert.Equal(1, result[0].Id);
            Xunit.Assert.Equal("USD", result[0].Currency);
            Xunit.Assert.Equal(1.1m, result[0].Rate);
            Xunit.Assert.Equal(new DateTime(2023, 5, 1), result[0].UpdateDate);
        }

        [Test]
        public void WriteToFile_ListOfCurrencyRates_WritesDataToXmlFile()
        {
            
            string fileName = "test.xml";
            var currencyRates = new List<CurrencyRate>
            {
                new CurrencyRate { Id = 1, Currency = "USD", Rate = 1.1m, UpdateDate = new DateTime(2023, 5, 1) },
                new CurrencyRate { Id = 2, Currency = "EUR", Rate = 0.9m, UpdateDate = new DateTime(2023, 5, 2) }
            };

            var xmlHandler = new XmlHandler();

            
            xmlHandler.WriteToFile(currencyRates, fileName);

            
            Xunit.Assert.True(File.Exists(fileName));
            string expectedContent = @"<?xml version=""1.0"" encoding=""utf-8""?>
<ArrayOfCurrencyRate xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
  <CurrencyRate>
    <Id>1</Id>
    <Currency>USD</Currency>
    <Rate>1.1</Rate>
    <UpdateDate>2023-05-01T00:00:00</UpdateDate>
  </CurrencyRate>
  <CurrencyRate>
    <Id>2</Id>
    <Currency>EUR</Currency>
    <Rate>0.9</Rate>
    <UpdateDate>2023-05-02T00:00:00</UpdateDate>
  </CurrencyRate>
</ArrayOfCurrencyRate>";

            string actualContent = File.ReadAllText(fileName);
            Xunit.Assert.Equal(expectedContent, actualContent);
        }

        [Test]
        public void ReadFromFile_NonExistingFile_ThrowsFileNotFoundException()
        {
            var fileName = "non_existing_file.xml";
            var xmlHandler = new XmlHandler();

            Xunit.Assert.Throws<FileNotFoundException>(() => xmlHandler.ReadFromFile(fileName));
        }

        [Test]
        public void ReadFromFile_InvalidXmlFormat_ThrowsXmlException()
        {
            var fileName = "invalid_xml.xml";
            File.WriteAllText(fileName, "This is not valid XML data");
            var xmlHandler = new XmlHandler();

            Xunit.Assert.Throws<InvalidOperationException>(() => xmlHandler.ReadFromFile(fileName));
        }
    }

    public class YamlHandlerTests
    {
        [Test]
        public void ReadFromFile_ValidYamlFile_ReturnsListOfCurrencyRates()
        {
             
            string fileName = "test.yaml";
            string yamlContent = @"- Id: 1
  Currency: USD
  Rate: 1.1
  UpdateDate: 2023-05-01
- Id: 2
  Currency: EUR
  Rate: 0.9
  UpdateDate: 2023-05-02";
            File.WriteAllText(fileName, yamlContent);

            var yamlHandler = new YamlHandler();

            
            var result = yamlHandler.ReadFromFile(fileName);

            
            Xunit.Assert.Equal(2, result.Count);
            Xunit.Assert.Equal(1, result[0].Id);
            Xunit.Assert.Equal("USD", result[0].Currency);
            Xunit.Assert.Equal(1.1m, result[0].Rate);
            Xunit.Assert.Equal(new DateTime(2023, 5, 1), result[0].UpdateDate);
        }

        [Test]
        public void WriteToFile_ListOfCurrencyRates_WritesDataToYamlFile()
        { 
            string fileName = "test.yaml";
            var currencyRates = new List<CurrencyRate>
            {
                new CurrencyRate { Id = 1, Currency = "USD", Rate = 1.1m, UpdateDate = new DateTime(2023, 5, 1) },
                new CurrencyRate { Id = 2, Currency = "EUR", Rate = 0.9m, UpdateDate = new DateTime(2023, 5, 2) }
            };

            var yamlHandler = new YamlHandler();

            yamlHandler.WriteToFile(currencyRates, fileName);

            Xunit.Assert.True(File.Exists(fileName));
            string expectedContent = @"- Id: 1
  Currency: USD
  Rate: 1.1
  UpdateDate: 2023-05-01T00:00:00.0000000
- Id: 2
  Currency: EUR
  Rate: 0.9
  UpdateDate: 2023-05-02T00:00:00.0000000
";
            string actualContent = File.ReadAllText(fileName);
            Xunit.Assert.Equal(expectedContent, actualContent);
        }

        [Test]
        public void ReadFromFile_NonExistingFile_ThrowsFileNotFoundException()
        {
            var fileName = "non_existing_file.yaml";
            var yamlHandler = new YamlHandler();

            Xunit.Assert.Throws<FileNotFoundException>(() => yamlHandler.ReadFromFile(fileName));
        }

        [Test]
        public void ReadFromFile_InvalidYamlFormat_ThrowsYamlException()
        {
            var fileName = "invalid_yaml.yaml";
            File.WriteAllText(fileName, "This is not valid YAML: - data"); 
            var yamlHandler = new YamlHandler();

            Xunit.Assert.Throws<YamlException>(() => yamlHandler.ReadFromFile(fileName));
        }
    }

    public class UserInterfaceTests
    {
        [Test]
        public void SortData_SortByCurrency_ReturnsSortedListByCurrency()
        {
           
            var currencyRates = new List<CurrencyRate>
            {
                new CurrencyRate { Id = 1, Currency = "USD", Rate = 1.1m, UpdateDate = new DateTime(2023, 5, 1) },
                new CurrencyRate { Id = 2, Currency = "EUR", Rate = 0.9m, UpdateDate = new DateTime(2023, 5, 2) }
            };

            var result = UserInterface.SortData(currencyRates, "Currency");

            Xunit.Assert.Equal("EUR", result[0].Currency);
            Xunit.Assert.Equal("USD", result[1].Currency);
        }

        [Test]
        public void SearchData_SearchByCurrency_ReturnsFilteredListByCurrency()
        {
            
            var currencyRates = new List<CurrencyRate>
            {
                new CurrencyRate { Id = 1, Currency = "USD", Rate = 1.1m, UpdateDate = new DateTime(2023, 5, 1) },
                new CurrencyRate { Id = 2, Currency = "EUR", Rate = 0.9m, UpdateDate = new DateTime(2023, 5, 2) }
            };
 
            var result = UserInterface.SearchData(currencyRates, "USD");

            Xunit.Assert.Single(result);
            Xunit.Assert.Equal("USD", result[0].Currency);
        }

        
    }
}
