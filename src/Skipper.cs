using Pidgin;
using Pidgin.Comment;
using static Pidgin.Parser;

namespace Celin.AIS.Data
{
    public class Skipper
    {
        readonly static Parser<char, Unit> SKIP
            = Try(CommentParser.SkipBlockComment(String("/*"), String("*/"))
                 .Or(CommentParser.SkipLineComment(String("//"))));
        public static Parser<char, T> Next<T>(Parser<char, T> parser)
            => SkipWhitespaces
                .Then(SKIP.Optional()
                  .Then(SkipWhitespaces)
                    .Then(parser));
    }
}
