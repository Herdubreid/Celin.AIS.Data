using Pidgin;
using static Pidgin.Parser;
namespace Celin.AIS.Data
{
    public class List
    {
        static Parser<char, string> Item
            => Literal.DoubleQuoted.Map(s => '"' + s + '"')
            .Or(Alias.Parser);
        public static Parser<char, string> Parser
        => Try(
                Item
                .Separated(Char(','))
                .Select(i => string.Join('|', i))
                .Between(Char('('), SkipWhitespaces.Then(Char(')')))
            )
           .Labelled("Alias List");
    }
}
