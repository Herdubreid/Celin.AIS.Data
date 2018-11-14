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
        {
            get
            {
                return Plain
                    .Or(Quoted)
                    .Labelled("Literal");
            }
        }
        public static Parser<char, IEnumerable<string>> Array
        {
            get
            {
                return Try(Parser)
                    //.Between(SkipWhitespaces)
                    .Separated(Char(','))
                    .Labelled("Literal Array");
            }
        }
    }
}
