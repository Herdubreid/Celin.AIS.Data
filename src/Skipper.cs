using Pidgin;
using Pidgin.Comment;
using static Pidgin.Parser;

namespace Celin.AIS.Data
{
    public class Skipper
    {
        public static Parser<char, T> Next<T>(Parser<char, T> parser)
        {
            return
                SkipWhitespaces
                .Then(Try(CommentParser.SkipBlockComment(String("/*"), String("*/"))
                 .Or(CommentParser.SkipLineComment(String("//")))).Optional()
                 .Then(SkipWhitespaces))
                .Then(parser);
        }
    }
}
