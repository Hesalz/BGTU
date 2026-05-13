﻿using SqlExportLib;

namespace SqlToCsvApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string connectionString = @"Server=localhost;Database=lab11;TrustServerCertificate=True;Integrated Security=True;";
            string outputFile = @"D:\BGTU\VI sem\MCXOiAD\lab10\lab11\file\Orders.csv";

            var exporter = new SqlToCsvExporter(connectionString);
            exporter.ExportTableToCsv("Orders", outputFile);
            Console.WriteLine("Экспорт заказов из MS SQL Server в CSV завершен.");
        }
    }
}