using Pidgin;
using static Pidgin.Parser;
namespace Celin.AIS.Data
{
    public class List
    {
        public static Parser<char, string> Parser
        => Try(
                Alias.List
                .Between(Char('('), Char(')'))
            )
           .Labelled("Alias List");
    }
}
