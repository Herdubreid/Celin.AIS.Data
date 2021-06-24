using Pidgin;
using static Pidgin.Parser;
namespace Celin.AIS.Data
{
    public class List
    {
        public static Parser<char, string> Parser
        => Try(
                Alias.List
                .Between(Char('('), SkipWhitespaces.Then(Char(')')))
            )
           .Labelled("Alias List");
    }
}
