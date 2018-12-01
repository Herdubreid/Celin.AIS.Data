using System.Linq;
using System.Collections.Generic;
using Pidgin;
using static Pidgin.Parser;
namespace Celin.AIS.Data
{
    public enum AggregationType
    {
        AGGREGATIONS,
        GROUP_BY,
        ORDER_BY
    }
    public class Aggregate
    {
        static readonly Parser<char, (AggregationType type, string attrib)> SUM =
            Try(String("sum")).Select(attrib => (AggregationType.AGGREGATIONS, attrib.ToUpper()));
        static readonly Parser<char, (AggregationType type, string attrib)> MIN =
            Try(String("min")).Select(attrib => (AggregationType.AGGREGATIONS, attrib.ToUpper()));
        static readonly Parser<char, (AggregationType type, string attrib)> MAX =
            Try(String("max")).Select(attrib => (AggregationType.AGGREGATIONS, attrib.ToUpper()));
        static readonly Parser<char, (AggregationType type, string attrib)> AVG =
            Try(String("avg")).Select(attrib => (AggregationType.AGGREGATIONS, attrib.ToUpper()));
        static readonly Parser<char, (AggregationType type, string attrib)> COUNT =
            Try(String("count")).Select(attrib => (AggregationType.AGGREGATIONS, attrib.ToUpper()));
        static readonly Parser<char, (AggregationType type, string attrib)> COUNT_DISTINCT =
            Try(String("count_distinct")).Select(attrib => (AggregationType.AGGREGATIONS, attrib.ToUpper()));
        static readonly Parser<char, (AggregationType type, string attrib)> AVG_DISTINCT =
            Try(String("avg_distinct")).Select(attrib => (AggregationType.AGGREGATIONS, attrib.ToUpper()));
        static readonly Parser<char, (AggregationType type, string attrib)> GROUP =
            Try(String("group")).ThenReturn((AggregationType.GROUP_BY, ""));
        static readonly Parser<char, (AggregationType type, string attrib)> DESC =
            Try(String("desc")).Select(attrib => (AggregationType.ORDER_BY, attrib.ToUpper()));
        static readonly Parser<char, (AggregationType type, string attrib)> ASC =
            Try(String("asc")).Select(attrib => (AggregationType.ORDER_BY, attrib.ToUpper()));
        public static Parser<char, (AggregationType type, IEnumerable<AggregationItem> items)> Parser
        => Map((t, a)
               => (t.type, a.Select(e =>
                new AggregationItem()
                {
                    aggregation = t.type == AggregationType.AGGREGATIONS ? t.attrib : null,
                    direction = t.type == AggregationType.ORDER_BY ? t.attrib : null,
                    column = e.ToString()
                })),
               Whitespaces
               .Then(OneOf(
                   SUM,
                   MIN,
                   MAX,
                   AVG,
                   COUNT,
                   COUNT_DISTINCT,
                   AVG_DISTINCT,
                   GROUP,
                   DESC,
                   ASC)),
              Whitespaces
               .Then(Alias.Array))
             .Labelled("Aggregate Item");
        public static Parser<char, IEnumerable<(AggregationType type, AggregationItem item)>> Array
        => Try(Parser)
            .Separated(Whitespace)
            .Between(Char('['),Char(']'))
            .Select(a => a.SelectMany(e => e.items.Select(it => (e.type, it))))
           .Labelled("Aggregation");
    }
}
