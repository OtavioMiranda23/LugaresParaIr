namespace LugaresParaIr.Exceptions;

public class InvalidEmailException : Exception
{
    public InvalidEmailException() : base("Email inválido") {}
    public InvalidEmailException(string message) : base(message) {}
    public InvalidEmailException(string message, Exception inner) : base(message, inner) {}
}