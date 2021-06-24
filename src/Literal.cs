using System.Collections.Generic;
using Pidgin;
using static Pidgin.Parser;
namespace Celin.AIS.Data
{
    public class Literal
    {
        static readonly Parser<char, string> Plain =
            Try(LetterOrDigit)
                .Then(LetterOrDigit
                      .ManyString(), (h, t) => h + t);
        static readonly Parser<char, string> Quoted =
            AnyCharExcept('"')
                .ManyString()
                .Between(Char('"'));
        public static Parser<char, string> Parser
            => Plain
               .Or(Quoted)
               .Labelled("Literal");
        public static Parser<char, IEnumerable<string>> Array
            => Try(Parser)
               .Separated(Char(','))
               .Labelled("Literal Array");
    }
}
