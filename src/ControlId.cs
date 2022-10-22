using Pidgin;
using static Pidgin.Parser;

namespace Celin.AIS.Data
{
    public class ControlId
    {
        static readonly Parser<char, string> Ids =
            SkipWhitespaces
            .Then(Digit
                .AtLeastOnceString())
            .Separated(Char(','))
            .Between(SkipWhitespaces.Then(Char('[')), SkipWhitespaces.Then(Char(']')))
            .Select(e => '[' + string.Join(',', e) + ']');
        static readonly Parser<char, string> Id =
            Digit
            .ManyString()
            .Then(Ids.Optional(), (pre, post) => (string.IsNullOrEmpty(pre) ? "1" : pre) + (post.HasValue ? post.Value : string.Empty));
        static readonly Parser<char, string> SubformId =
            Digit
            .AtLeastOnceString()
            .Then(Char('_'), (pre, post) => pre + post)
            .Then(Digit.AtLeastOnceString(), (pre, post) => pre + post);
        static readonly Parser<char, string> Subform =
            SubformId
            .Then(Ids.Optional(), (pre, post) => pre + (post.HasValue ? post.Value : string.Empty));
        public static Parser<char, string> Parser
            => SkipWhitespaces
            .Then(Id);
    }
}
