using Pidgin;
using static Pidgin.Parser;

namespace Celin.AIS.Data
{
    public class FormSubject
    {
        static readonly Parser<char, char> Prefix =
            OneOf('w', 'W');
        public static Parser<char, string> Parser
            => Prefix
            .Then(LetterOrDigit.ManyString(), (p, b) => $"P{b.Remove(b.Length - 1)}_{p}{b}".ToUpper())
            .Labelled("Form Subject");
    }
}
