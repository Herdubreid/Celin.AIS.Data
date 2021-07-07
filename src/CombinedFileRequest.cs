using Pidgin;
using Pidgin.Comment;
using System.Collections.Generic;
using System.Linq;
using static Pidgin.Parser;

namespace Celin.AIS.Data
{
    public class CombinedFileRequest
    {
        static AndOrCombinator last { get; set; } = AndOrCombinator.AND;
        public static Parser<char, DatabrowserRequest> Parser
        => Map((o, s, q) => new DatabrowserRequest()
        {
            batchDataRequest = true,
            dataRequests = s.Select(r =>
            {
                r.query = q.Count() > 0
                    ? new Query()
                    {
                        matchType = q.Count() == 1 ? q.First().Item2.ToString("G") : null,
                        condition = q.Count() == 1
                            ? q.First().Item3.Select(c => new Condition
                            {
                                controlId = $"{r.targetName}.{c.controlId}",
                                @operator = c.@operator,
                                value = c.value
                            })
                            : null,
                        complexQuery = q.Count() > 1
                            ? q.Select(cq =>
                                {
                                    var qry = new ComplexQuery
                                    {
                                        andOr = last.ToString("G"),
                                        query = new Query()
                                        {
                                            matchType = cq.Item2.ToString("G"),
                                            condition = (cq.Item3 as List<Condition>)
                                            .Select(c => new Condition
                                            {
                                                @operator = c.@operator,
                                                controlId = $"{r.targetName}.{c.controlId}",
                                                value = c.value
                                            })
                                        }
                                    };
                                    last = cq.Item1.HasValue ? cq.Item1.Value : AndOrCombinator.AND;
                                    return qry;
                                })
                            : null
                    }
                    : null;
                r.formServiceDemo = o.HasValue && o.Value.Item1.HasValue ? "TRUE" : null;
                r.maxPageSize = o.HasValue && o.Value.Item3.HasValue ? o.Value.Item3.Value : null;
                return r;
            }),
        },
         Skipper.Next(QryOptions.Parser.Optional()),
         Skipper.Next(FileSelection.Array),
         Skipper.Next(QryOp.Queries))
        .Before(CommentParser.SkipLineComment(String("//")).Optional());
    }
}
