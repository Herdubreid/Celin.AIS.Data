using Pidgin;
using System;
using static Pidgin.Parser;

namespace Celin.AIS.Data
{
    using QryOptionDef = ValueTuple<Maybe<bool>, Maybe<bool>, Maybe<string>>;
    class QryOptions
    {
        protected static readonly Parser<char, bool> DEMO =
            String("-demo")
             .ThenReturn(true)
             .Labelled("Demo");
        protected static readonly Parser<char, bool> V2 =
            String("-v2")
             .ThenReturn(true)
             .Labelled("Version 2");
        protected static readonly Parser<char, string> MAX =
            String("-max")
             .Then(SkipWhitespaces)
             .Then(Char('=').Optional())
             .Then(SkipWhitespaces)
             .Then(Digit.ManyString())
             .Labelled("Max Records");
        public static Parser<char, QryOptionDef> Parser
            => Map((d, v, m) => new QryOptionDef(d, v, m),
            SkipWhitespaces
             .Then(Try(DEMO)
             .Optional()),
            SkipWhitespaces
            .Then(Try(V2)
            .Optional()),
            SkipWhitespaces
            .Then(Try(MAX)
            .Optional()));
    }
}
