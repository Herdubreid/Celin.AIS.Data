using System.Collections.Immutable;
using Pidgin;
using static Pidgin.Parser;
namespace Celin.AIS.Data
{
    public class DataAction
    {
        public string Aliases { get; set; } = null;
        public Aggregation Aggregation { get; set; } = null;
        public static Parser<char, DataAction> Parser
        => Try(List.Parser
               .Select(l => new DataAction() { Aliases = l })
           .Or(Aggregate.Array
               .Select(a => new DataAction() { Aggregation = a })));
    }
}
