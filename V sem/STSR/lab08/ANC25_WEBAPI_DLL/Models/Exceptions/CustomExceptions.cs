public class DeleteError : Exception
{
    public DeleteError(string message) : base(message) { }
}

public class NullError : Exception
{
    public NullError(string message) : base(message) { }
}

public class FileError : Exception
{
    public FileError(string message) : base(message) { }
}

public class MinusIdError : Exception
{
    public MinusIdError(string message) : base(message) { }
}