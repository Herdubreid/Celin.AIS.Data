using Pidgin;
using Pidgin.Comment;
using System.Collections.Generic;
using System.Linq;
using static Pidgin.Parser;

namespace Celin.AIS.Data
{
    public class DataRequest
    {
        static AndOrCombinator last { get; set; } = AndOrCombinator.AND;
        public static Parser<char, DatabrowserRequest> Parser
        => Map((s, o, a, q, h) => new DatabrowserRequest
        {
            targetName = s.Name.ToUpper(),
            targetType = s.Type,
            dataServiceType = a.HasValue && a.Value.IsAggregation
                ? DatabrowserRequest.AGGREGATION
                : DatabrowserRequest.BROWSE,
            returnControlIDs = a.HasValue ? a.Value.Aliases : null,
            aggregation = a.HasValue ? a.Value.Aggregation : null,
            having = h.HasValue
                ? new AIS.Having
                {
                    condition = h.Value.Select(c =>
                    {
                        c.controlId = c.controlId.Contains('.')
                            ? c.controlId
                            : $"{s.Name.ToUpper()}.{c.controlId}";
                        return c;
                    })
                }
                : null,
            findOnEntry = Request.TRUE,
            query = q.Count() > 0
                ? new Query()
                {
                    matchType = q.Count() == 1 ? q.First().Item2.ToString("G") : null,
                    condition = q.Count() == 1
                    ? q.First().Item3.Select(c => new Condition
                    {
                        controlId = c.controlId.Contains('.')
                        ? c.controlId
                        : $"{s.Name.ToUpper()}.{c.controlId}",
                        @operator = c.@operator,
                        value = c.value
                    })
                    : null,
                    complexQuery = q.Count() > 1
                        ? q.Select(r =>
                            {
                                var qry = new ComplexQuery
                                {
                                    andOr = last.ToString("G"),
                                    query = new Query()
                                    {
                                        matchType = r.Item2.ToString("G"),
                                        condition = r.Item3.Select(c => new Condition
                                        {
                                            controlId = c.controlId.Contains('.')
                                            ? c.controlId
                                            : $"{s.Name.ToUpper()}.{c.controlId}",
                                            @operator = c.@operator,
                                            value = c.value
                                        })
                                    }
                                };
                                last = r.Item1.HasValue ? r.Item1.Value : AndOrCombinator.AND;
                                return qry;
                            })
                        : null
                }
                : null,
            formServiceDemo = o.HasValue && o.Value.Item1.HasValue ? Request.TRUE : null,
            outputType = o.HasValue && o.Value.Item2.HasValue ? Request.VERSION2 : Request.GRID_DATA,
            maxPageSize = o.HasValue && o.Value.Item3.HasValue
            ? o.Value.Item3.Value.Equals("no") ? "No Max" : o.Value.Item3.Value
            : null
        },
         Skipper.Next(DataSubject.Parser),
         Skipper.Next(QryOptions.Parser.Optional()),
         Skipper.Next(DataAction.Parser.Optional()),
         Skipper.Next(QryOp.Queries),
         Skipper.Next(Having.Parser.Optional()))
        .Before(CommentParser.SkipLineComment(String("//")).Optional());
    }
}
