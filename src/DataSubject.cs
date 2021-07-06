using Pidgin;
using static Pidgin.Parser;
namespace Celin.AIS.Data
{
    public class DataSubject
    {
        public string Type { get; protected set; }
        public string Name { get; set; }
        static readonly Parser<char, char> Prefix =
            OneOf('f', 'F', 'v', 'V');
        public static Parser<char, DataSubject> Parser
        => Prefix
            .Then(LetterOrDigit
             .ManyString(), (p, b) => new DataSubject()
             {
                 Type = "fF".Contains(p) ? "table" : "view",
                 Name = p + b
             })
            .Labelled("Table or View Name");
    }
}
