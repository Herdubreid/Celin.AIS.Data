using System;
using System.Collections.Generic;
using System.Linq;
using Pidgin;
using static Pidgin.Parser;

namespace Celin.AIS.Data
{
    public class Having
    {
        static readonly Parser<char, (AggregationType type, string attrib)> SUM =
            Try(String("sum")).Select(attrib => (AggregationType.AGGREGATIONS, attrib.ToUpper()));
        static readonly Parser<char, (AggregationType type, string attrib)> AVG =
            Try(String("avg")).Select(attrib => (AggregationType.AGGREGATIONS, attrib.ToUpper()));
        static readonly Parser<char, (AggregationType type, string attrib)> COUNT =
            Try(String("count")).Select(attrib => (AggregationType.AGGREGATIONS, attrib.ToUpper()));
        static Parser<char, Condition> Parameter
            => Map((a, o) =>
            {
                o.aggregation = a.attrib;
                return o;
            },
            OneOf(SUM, AVG, COUNT),
            QryOp.DataParameter.Between(Char('('), Char(')')));
        public static Parser<char, IEnumerable<Condition>> Parser
            => Parameter
            .Separated(Whitespace)
            .Between(String("having["), SkipWhitespaces.Then(Char(']')));
    }
}
