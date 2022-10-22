using Pidgin;
using Pidgin.Comment;
using System.Linq;
using static Pidgin.Parser;

namespace Celin.AIS.Data
{
    public class CombinedFileRequest
    {
        static AndOrCombinator last { get; set; } = AndOrCombinator.AND;
        public static Parser<char, Request> Parser
        => Map((o, s, q) => new DatabrowserRequest()
        {
            batchDataRequest = true,
            formServiceDemo = o.HasValue && o.Value.Item1.HasValue ? Request.TRUE : null,
            dataRequests = s.Select(r =>
            {
                r.maxPageSize = o.HasValue && o.Value.Item3.HasValue ? o.Value.Item3.Value : null;
                r.query = q.Count() > 0
                    ? new Query()
                    {
                        matchType = q.Count() == 1 ? q.First().Item2.ToString("G") : null,
                        condition = q.Count() == 1
                            ? q.First().Item3.Select(c => new Condition
                            {
                                controlId = c.controlId.Contains('.')
                                    ? c.controlId.Split('.')[0].Equals(r.targetName)
                                        ? c.controlId
                                        : null
                                    : $"{r.targetName}.{c.controlId}",
                                @operator = c.@operator,
                                value = c.value
                            }).Where(c => c.controlId != null)
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
                                        condition = cq.Item3
                                        .Select(c => new Condition
                                        {
                                            controlId = c.controlId.Contains('.')
                                                ? c.controlId.Split('.')[0].Equals(r.targetName)
                                                    ? c.controlId
                                                    : null
                                                : $"{r.targetName}.{c.controlId}",
                                            @operator = c.@operator,
                                            value = c.value
                                        }).Where(c => c.controlId != null)
                                    }
                                };
                                last = cq.Item1.HasValue ? cq.Item1.Value : AndOrCombinator.AND;
                                return qry;
                            })
                            : null
                    }
                    : null;
                return r;
            }),
        } as Request,
         Skipper.Next(QryOptions.Parser.Optional()),
         Skipper.Next(FileSelection.Array),
         Skipper.Next(QryOp.DataQueries))
        .Before(CommentParser.SkipLineComment(String("//")).Optional());
    }
}
