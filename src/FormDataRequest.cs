using Pidgin;
using Pidgin.Comment;
using System.Linq;
using static Pidgin.Parser;

namespace Celin.AIS.Data
{
    public class FormDataRequest
    {
        static AndOrCombinator last { get; set; } = AndOrCombinator.AND;
        public static Parser<char, Request> Parser
            => Map((s, v, o, c, q) => new AIS.FormRequest
            {
                findOnEntry = AIS.Request.TRUE,
                formName = s,
                version = v.HasValue ? v.Value.ToUpper() : null,
                formServiceDemo = o.HasValue && o.Value.Item1.HasValue ? Request.TRUE : null,
                outputType = o.HasValue && o.Value.Item2.HasValue ? Request.VERSION2 : Request.GRID_DATA,
                maxPageSize = o.HasValue && o.Value.Item3.HasValue
                ? o.Value.Item3.Value.Equals("no") ? "No Max" : o.Value.Item3.Value
                : null,
                returnControlIDs = c.HasValue ? c.Value : null,
                query = q.Count() > 0
                ? new Query()
                {
                    matchType = q.Count() == 1 ? q.First().Item2.ToString("G") : null,
                    condition = q.Count() == 1
                    ? q.First().Item3
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
                                    condition = r.Item3
                                }
                            };
                            last = r.Item1.HasValue ? r.Item1.Value : AndOrCombinator.AND;
                            return qry;
                        })
                        : null
                }
                : null,
            } as AIS.Request,
            Skipper.Next(FormSubject.Parser),
            Skipper.Next(LetterOrDigit.ManyString().Optional()),
            Skipper.Next(QryOptions.Parser.Optional()),
            Skipper.Next(ControlId.Parser.Optional()),
            Skipper.Next(QryOp.FormQueries))
            .Before(CommentParser.SkipLineComment(String("//")).Optional());
    }
}
