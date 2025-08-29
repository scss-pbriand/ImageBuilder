namespace ImageBuilder.Server.Models;

public class DomainException : ApplicationException
{
    public DomainException(string message) : base(message)
    {
    }
}
