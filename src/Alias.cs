using System.Collections.Generic;
using Pidgin;
using static Pidgin.Parser;
namespace Celin.AIS.Data
{
    public class Alias
    {
        public string Prefix { get; protected set; }
        public string Id { get; protected set; }
        public override string ToString()
        {
            return Prefix is null
                    ? Id
                    : string.Format("{0}.{1}", Prefix, Id);
        }
        protected static Parser<char, string> LString =
            Try(Letter)
                .Then(LetterOrDigit
                      .ManyString(), (h, t) => h + t);
        static readonly Parser<char, string> AliasSuffix =
            Char('.')
                .Then(LString)
                .Select(name => name);
        public static Parser<char, Alias> Parser
        => Map((p, s) => new Alias()
            {
                Prefix = s is null ? null : p,
                Id = s is null ? p : s
            },
            LString,
            AliasSuffix
            .Optional()
            .Select(suf => suf.HasValue ? suf.Value : null))
            .Labelled("Alias");
        public static Parser<char, string> List
        => Parser
           .Between(SkipWhitespaces)
           .Separated(Char(','))
           .Select(els => string.Join('|', els))
           .Labelled("Alias List");
        public static Parser<char, IEnumerable<Alias>> Array
        => Parser
           .Between(SkipWhitespaces)
           .Separated(Char(','))
           .Labelled("Alias Array");
    }
}
