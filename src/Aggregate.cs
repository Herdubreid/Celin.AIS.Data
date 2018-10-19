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
            String("sum").Select(attrib => (AggregationType.AGGREGATIONS, attrib.ToUpper()));
        static readonly Parser<char, (AggregationType type, string attrib)> MIN =
            String("min").Select(attrib => (AggregationType.AGGREGATIONS, attrib.ToUpper()));
        static readonly Parser<char, (AggregationType type, string attrib)> MAX =
            String("max").Select(attrib => (AggregationType.AGGREGATIONS, attrib.ToUpper()));
        static readonly Parser<char, (AggregationType type, string attrib)> AVG =
            String("avg").Select(attrib => (AggregationType.AGGREGATIONS, attrib.ToUpper()));
        static readonly Parser<char, (AggregationType type, string attrib)> COUNT =
            String("count").Select(attrib => (AggregationType.AGGREGATIONS, attrib.ToUpper()));
        static readonly Parser<char, (AggregationType type, string attrib)> COUNT_DISTINCT =
            String("count_distinct").Select(attrib => (AggregationType.AGGREGATIONS, attrib.ToUpper()));
        static readonly Parser<char, (AggregationType type, string attrib)> AVG_DISTINCT =
            String("avg_distinct").Select(attrib => (AggregationType.AGGREGATIONS, attrib.ToUpper()));
        static readonly Parser<char, (AggregationType type, string attrib)> GROUP =
            String("group").ThenReturn((AggregationType.GROUP_BY, ""));
        static readonly Parser<char, (AggregationType type, string attrib)> DESC =
            String("desc").Select(attrib => (AggregationType.ORDER_BY, attrib.ToUpper()));
        static readonly Parser<char, (AggregationType type, string attrib)> ASC =
            String("asc").Select(attrib => (AggregationType.ORDER_BY, attrib.ToUpper()));
        public static Parser<char, (AggregationType type, IEnumerable<AggregationItem> items)> Parser
        => Map((t, a)
               => (t.type, a.Select(e =>
                   new AggregationItem()
                   {
                       aggregation = t.type == AggregationType.AGGREGATIONS ? t.attrib : null,
                       direction = t.type == AggregationType.ORDER_BY ? t.attrib : null,
                       column = e.ToString()
                   })),
             OneOf(SUM,
                   MIN,
                   MAX,
                   AVG,
                   COUNT,
                   COUNT_DISTINCT,
                   AVG_DISTINCT,
                   GROUP,
                   DESC,
                   ASC)
                .Before(Whitespace),
              Alias.Array)
             .Labelled("Aggregate Item");
        public static Parser<char, Aggregation> Array
        => Parser
           .Between(SkipWhitespaces)
           .Separated(Char(';'))
            .Select(a => new Aggregation()
            {
                aggregations = a
                .Where(e => e.type == AggregationType.AGGREGATIONS)
                .SelectMany(e => e.items) as List<AggregationItem>,
                groupBy = a
                .Where(e => e.type == AggregationType.GROUP_BY)
                .SelectMany(e => e.items) as List<AggregationItem>,
                orderBy = a
                .Where(e => e.type == AggregationType.ORDER_BY)
                .SelectMany(e => e.items) as List<AggregationItem>
            })
           .Labelled("Aggregation");
    }
}
