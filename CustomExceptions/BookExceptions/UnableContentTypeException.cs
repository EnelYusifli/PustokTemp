namespace PustokTemp.CustomExceptions.BookExceptions;

public class UnableContentTypeException : Exception
{
    public string Property { get; set; }
    public UnableContentTypeException()
    {
    }

    public UnableContentTypeException(string? message) : base(message)
    {
    }
    public UnableContentTypeException(string? message, string prop) : base(message)
    {
        Property = prop;
    }
}
