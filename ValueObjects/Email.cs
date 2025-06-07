using System.Text.RegularExpressions;
using LugaresParaIr.Builder;
using LugaresParaIr.Exceptions;

namespace LugaresParaIr.ValueObjects;

public class Email
{
    public Email(string address)
    {
        if (string.IsNullOrEmpty(address) || address.Length < 5)
        {
            throw new InvalidEmailException();
        }

        Address = address.ToLower().Trim();
        const string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
        if (Regex.IsMatch(Address, pattern))
        {
            throw new InvalidEmailException();
        }
    }
    public string Address { get; }
}