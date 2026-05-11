using Microsoft.Data.SqlClient;
using System.Globalization;
using System.Text;

namespace SqlExportLib
{
    public class SqlToCsvExporter
    {
        private readonly string _connectionString;
        private readonly char _delimiter = ';';

        public SqlToCsvExporter(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void ExportTableToCsv(string tableName, string outputFilePath)
        {
            string? directory = Path.GetDirectoryName(outputFilePath);

            if (!string.IsNullOrWhiteSpace(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            using SqlConnection connection = new SqlConnection(_connectionString);
            connection.Open();

            string query = $"SELECT * FROM [{tableName}]";

            using SqlCommand command = new SqlCommand(query, connection);
            using SqlDataReader reader = command.ExecuteReader();
            using StreamWriter writer = new StreamWriter(outputFilePath, false, new UTF8Encoding(true));

            for (int i = 0; i < reader.FieldCount; i++)
            {
                if (i > 0)
                    writer.Write(_delimiter);

                writer.Write(reader.GetName(i));
            }

            writer.WriteLine();

            while (reader.Read())
            {
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    if (i > 0)
                        writer.Write(_delimiter);

                    object value = reader.GetValue(i);
                    writer.Write(ConvertValue(value));
                }

                writer.WriteLine();
            }
        }

        private string ConvertValue(object value)
        {
            if (value == DBNull.Value)
                return "";

            if (value is DateTime date)
                return date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);

            if (value is IFormattable formattable)
                return formattable.ToString(null, CultureInfo.InvariantCulture) ?? "";

            return value.ToString() ?? "";
        }
    }
}