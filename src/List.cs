using Pidgin;
using static Pidgin.Parser;
namespace Celin.AIS.Data
{
    public class List
    {
        public static Parser<char, string> Parser
        => Try(String("list"))
           .Before(Whitespace)
           .Then(Alias.List)
           .Labelled("Alias List");
    }
}
