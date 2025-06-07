namespace LugaresParaIr.Exceptions;

public class TemplateNotFoundException : Exception
{
    public TemplateNotFoundException() : base ("O template não foi encontrado.") {}
    public  TemplateNotFoundException(string message) : base(message) {}
    public  TemplateNotFoundException(string message, Exception inner) : base(message, inner) {}
}