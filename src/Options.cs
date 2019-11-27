using Pidgin;
using System;
using static Pidgin.Parser;

namespace Celin.AIS.Data
{
    using QryOptionDef = ValueTuple<Maybe<bool>, Maybe<bool>, Maybe<string>>;
    class QryOptions
    {
        protected static readonly Parser<char, bool> DEMO =
            String("demo").ThenReturn(true);
        protected static readonly Parser<char, bool> V2 =
            String("v2").ThenReturn(true);
        protected static readonly Parser<char, string> MAX =
            String("max")
            .Then(SkipWhitespaces)
            .Then(Char('=').Optional())
            .Then(SkipWhitespaces)
            .Then(Digit.ManyString());
        public static Parser<char, QryOptionDef> Parser
            => Map((d, v, m, end) => new QryOptionDef(d, v, m),
            Try(DEMO)
            .Optional(),
            SkipWhitespaces
            .Then(Try(V2))
            .Optional(),
            SkipWhitespaces
            .Then(MAX)
            .Optional(),
            SkipWhitespaces
            .Then(Char('.')));
    }
}
