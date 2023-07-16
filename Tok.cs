using Pidgin;
using static Pidgin.Parser;

namespace Celin.AIS.Data;

public class Base
{
    public static Parser<char, T> Tok<T>(Parser<char, T> p)
        => Try(p).Before(SkipWhitespaces);

    public static Parser<char, char> Tok(char value) => Tok(Char(value));
    public static Parser<char, string> Tok(string value) => Tok(String(value));
}
