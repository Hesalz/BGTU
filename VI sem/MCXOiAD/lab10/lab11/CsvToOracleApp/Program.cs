using OracleImportLib;

namespace CsvToOracleApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string oracleConnectionString = "Data Source=//localhost:1521/orcl;User Id=system;Password=your_password;";
            string csvFile = @"D:\BGTU\VI sem\MCXOiAD\lab10\lab11\file\Orders.csv";

            var importer = new CsvToOracleImporter(oracleConnectionString);
            importer.ImportOrders(csvFile);
            Console.WriteLine("Импорт заказов из CSV в Oracle завершен.");
        }
    }
}