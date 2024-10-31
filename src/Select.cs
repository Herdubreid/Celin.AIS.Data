using Pidgin;
using System.Collections.Generic;
using System.Linq;
using static Pidgin.Parser;

namespace Celin.AIS.Data
{
    public class Select
    {
        static Parser<char, IEnumerable<AggregationItem>> OrderBy
            => Map((d, a) => (a.Select(e => new AggregationItem { direction = d.ToUpper(), column = e.ToString() })),
                 Base.Tok("desc").Or(Base.Tok("asc")),
                 Alias.Array)
               .Labelled("Order By");
        public static Parser<char, (string list, IEnumerable<AggregationItem> order)> Parser
            => Map((l, o) 
                => (l.HasValue ? l.Value : string.Empty, o.HasValue ? o.Value.SelectMany(e => e) : Enumerable.Empty<AggregationItem>()),
                 List.Parser.Optional(),
                 Skipper.Next(
                     OrderBy
                     .Before(SkipWhitespaces)
                     .Separated(Whitespace)
                     .Between(Base.Tok("by["), SkipWhitespaces.Then(Char(']'))).Optional()))
                .Labelled("Select");
    }
}
