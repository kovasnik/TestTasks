using System;
using System.IO;
using System.Threading.Tasks;
using TestTasks.InternationalTradeTask;
using TestTasks.VowelCounting;
using TestTasks.WeatherFromAPI;

namespace TestTasks
{
    class Program
    {
        static async Task Main()
        {
            //Below are examples of usage. However, it is not guaranteed that your implementation will be tested on those examples.            
            var stringProcessor = new StringProcessor();
            string str = File.ReadAllText("./CharCounting/StringExample.txt");
            var charCount = stringProcessor.GetCharCount(str, new char[] { 'l', 'r', 'm' });

            foreach (var (symbol, count) in charCount)
            {
                Console.WriteLine($"'{symbol}': {count}");
            }

            var commodityRepository = new CommodityRepository();
            commodityRepository.GetImportTariff("Natural honey");
            commodityRepository.GetExportTariff("Iron/steel scrap not sorted or graded");
            //Console.WriteLine($"{commodityRepository.GetImportTariff("Natural honey")}");
            //Console.WriteLine($"{commodityRepository.GetExportTariff("Iron/steel scrap not sorted or graded")}");

            var weatherManager = new WeatherManager();
            var comparisonResult = await weatherManager.CompareWeather("kyiv,ua", "lviv,ua", 4);
            Console.WriteLine($"{comparisonResult.WarmerDaysCount}{comparisonResult.RainierDaysCount}");
        }
    }
}
