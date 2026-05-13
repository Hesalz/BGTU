using Oracle.ManagedDataAccess.Client;
using System.Globalization;

namespace OracleImportLib
{
    public class CsvToOracleImporter
    {
        private readonly string _connectionString;
        private readonly char _delimiter = ';';

        public CsvToOracleImporter(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void ImportOrders(string csvFilePath)
        {
            string[] lines = File.ReadAllLines(csvFilePath);

            using OracleConnection connection = new OracleConnection(_connectionString);
            connection.Open();

            using OracleTransaction transaction = connection.BeginTransaction();

            string sql = @"
                INSERT INTO Orders
                (OrderId, CustomerName, ProductName, Quantity, Price, OrderDate, Status)
                VALUES
                (:OrderId, :CustomerName, :ProductName, :Quantity, :Price, :OrderDate, :Status)";

            using OracleCommand command = new OracleCommand(sql, connection);
            command.Transaction = transaction;

            try
            {
                // Пропускаем заголовок (первая строка)
                for (int i = 1; i < lines.Length; i++)
                {
                    if (string.IsNullOrWhiteSpace(lines[i]))
                        continue;

                    string[] values = lines[i].Split(_delimiter);

                    if (values.Length < 7)
                        continue;

                    command.Parameters.Clear();

                    command.Parameters.Add(":OrderId", OracleDbType.Int32)
                        .Value = int.Parse(values[0]);

                    command.Parameters.Add(":CustomerName", OracleDbType.NVarchar2)
                        .Value = values[1];

                    command.Parameters.Add(":ProductName", OracleDbType.NVarchar2)
                        .Value = values[2];

                    command.Parameters.Add(":Quantity", OracleDbType.Int32)
                        .Value = int.Parse(values[3]);

                    command.Parameters.Add(":Price", OracleDbType.Decimal)
                        .Value = decimal.Parse(values[4], CultureInfo.InvariantCulture);

                    command.Parameters.Add(":OrderDate", OracleDbType.Date)
                        .Value = DateTime.ParseExact(values[5], "yyyy-MM-dd", CultureInfo.InvariantCulture);

                    command.Parameters.Add(":Status", OracleDbType.NVarchar2)
                        .Value = values[6];

                    command.ExecuteNonQuery();
                }

                transaction.Commit();
                Console.WriteLine($"Импортировано {lines.Length - 1} заказов.");
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
    }
}