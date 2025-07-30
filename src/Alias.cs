using System.Collections.Generic;
using Pidgin;
using static Pidgin.Parser;
namespace Celin.AIS.Data
{
    public class Alias
    {
        static readonly Parser<char, string> LString =
            Try(SkipWhitespaces
                .Then(LetterOrDigit
                      .ManyString()));
        static readonly Parser<char, string> AliasSuffix =
            Char('.')
                .Then(LString)
                .Select(name => name);
        public static Parser<char, string> Parser
        => Map((p, s) => s is null
            ? p.ToUpper() 
            : string.Format("{0}.{1}", p.ToUpper(), s.ToUpper()),
            LString,
            AliasSuffix
            .Optional()
            .Select(suf => suf.HasValue ? suf.Value : null))
            .Labelled("Alias");
        public static Parser<char, string> List
        => Try(Parser)
           .Separated(Char(','))
           .Select(els => string.Join("|", els))
           .Labelled("Alias List");
        public static Parser<char, IEnumerable<string>> Array
        => Try(Parser)
           .Separated(Char(','))
           .Between(Char('('), SkipWhitespaces.Then(Char(')')))
           .Labelled("Alias Array");
    }
}
