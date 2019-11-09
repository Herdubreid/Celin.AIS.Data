using Pidgin;
using static Pidgin.Parser;

namespace Celin.AIS.Data
{
    class MaxPageOption
    {
        public static Parser<char, string> Parser
            => Try(String("max"))
                .Then(SkipWhitespaces)
                .Then(Digit.ManyString());
    }
}
