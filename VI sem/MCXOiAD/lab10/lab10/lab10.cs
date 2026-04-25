using System;
using System.IO;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;

[Serializable]
[SqlUserDefinedType(Format.UserDefined, IsByteOrdered = true, MaxByteSize = 8000)]
public struct CurrencyExchange : INullable, IBinarySerialize
{
    private string _currencyCode;
    private decimal _exchangeRate;
    private DateTime _exchangeDate;
    private bool _isNull;

    public string CurrencyCode
    {
        get { return _currencyCode; }
        set { _currencyCode = value; }
    }

    public decimal ExchangeRate
    {
        get { return _exchangeRate; }
        set { _exchangeRate = value; }
    }

    public DateTime ExchangeDate
    {
        get { return _exchangeDate; }
        set { _exchangeDate = value; }
    }

    public bool IsNull
    {
        get { return _isNull; }
    }

    public static CurrencyExchange Null
    {
        get
        {
            CurrencyExchange ce = new CurrencyExchange();
            ce._isNull = true;
            return ce;
        }
    }

    public static CurrencyExchange Parse(SqlString s)
    {
        if (s.IsNull || s.Value == "NULL")
            return Null;

        CurrencyExchange ce = new CurrencyExchange();
        string[] parts = s.Value.Split('|');

        if (parts.Length != 3)
            throw new ArgumentException("Неверный формат. Ожидается: КодВалюты|Курс|Дата");

        ce._currencyCode = parts[0];
        ce._exchangeRate = decimal.Parse(parts[1]);
        ce._exchangeDate = DateTime.Parse(parts[2]);
        ce._isNull = false;

        return ce;
    }

    public override string ToString()
    {
        if (_isNull)
            return "NULL";
        return string.Format("{0}|{1}|{2}", _currencyCode, _exchangeRate, _exchangeDate.ToString("yyyy-MM-dd"));
    }

    public SqlString GetCurrencyInfo()
    {
        if (_isNull)
            return SqlString.Null;
        return new SqlString(string.Format("Валюта: {0}, Курс: {1:F2}, Дата: {2:yyyy-MM-dd}",
            _currencyCode, _exchangeRate, _exchangeDate));
    }

    public void Write(BinaryWriter writer)
    {
        writer.Write(_isNull);
        if (!_isNull)
        {
            writer.Write(_currencyCode ?? "");
            writer.Write(_exchangeRate);
            writer.Write(_exchangeDate.ToBinary());
        }
    }

    public void Read(BinaryReader reader)
    {
        _isNull = reader.ReadBoolean();
        if (!_isNull)
        {
            _currencyCode = reader.ReadString();
            _exchangeRate = reader.ReadDecimal();
            _exchangeDate = DateTime.FromBinary(reader.ReadInt64());
        }
    }
}

public class FileOperations
{
    [SqlProcedure]
    public static void MoveFile(SqlString sourcePath, SqlString destinationPath, SqlBoolean overwrite)
    {
        if (sourcePath.IsNull || destinationPath.IsNull)
            throw new ArgumentException("Пути к файлам не могут быть NULL");

        string source = sourcePath.Value;
        string dest = destinationPath.Value;

        if (!File.Exists(source))
            throw new FileNotFoundException($"Исходный файл не найден: {source}");

        string destDirectory = Path.GetDirectoryName(dest);
        if (!string.IsNullOrEmpty(destDirectory) && !Directory.Exists(destDirectory))
            Directory.CreateDirectory(destDirectory);

        if (overwrite.IsNull || overwrite.Value)
        {
            if (File.Exists(dest))
                File.Delete(dest);
            File.Move(source, dest);
        }
        else
        {
            File.Move(source, dest);
        }
    }
}