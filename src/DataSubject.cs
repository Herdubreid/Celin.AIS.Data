using Pidgin;
using static Pidgin.Parser;
namespace Celin.AIS.Data
{
    public class DataSubject
    {
        public string Type { get; protected set; }
        public string Name { get; set; }
        static readonly Parser<char, char> Prefix =
            Char('f')
             .Or(Char('v'));
        public static Parser<char, DataSubject> Parser
        => Prefix
            .Then(LetterOrDigit
             .ManyString(), (p, b) => new DataSubject()
             {
                 Type = p == 'f' ? "table" : "view",
                 Name = p + b
             })
            .Labelled("Table or View Name");
    }
}
