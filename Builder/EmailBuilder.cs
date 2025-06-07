using LugaresParaIr.Dtos;

namespace LugaresParaIr.Builder;

public class EmailBuilder
{
    private readonly NotificationMessageModel _email;

    public EmailBuilder()
    {
        _email = new NotificationMessageModel();
    }

    //TODO: Criar Value Object Email
    public EmailBuilder SetFrom(string email)
    {
        _email.From = email;
        return this;
    }
    
    public EmailBuilder SetTo(string email)
    {
        _email.To = email;
        return this;
    }
    
    public EmailBuilder SetSubject(string title)
    {
        _email.Subject = title;
        return this;
    }
    public EmailBuilder SetBody(string content)
    {
        _email.Body = content;
        return this;
    }

    public EmailBuilder SetLink(string link)
    {
        _email.Body = _email.Body.Replace("{link}", link);
        return this;
    }
    public NotificationMessageModel Build()
    {
        return _email;
    }
}