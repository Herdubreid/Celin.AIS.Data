using Pidgin;
using System.Collections.Generic;
using System.Linq;
using static Pidgin.Parser;
namespace Celin.AIS.Data
{
    public class DataRequest
    {
        public static Parser<char, DatabrowserRequest> Parser
        => Map((s, o, a, q) => new DatabrowserRequest()
        {
            targetName = s.Name.ToUpper(),
            targetType = s.Type,
            dataServiceType = a.HasValue && a.Value.Aggregation != null ? "AGGREGATION" : "BROWSE",
            returnControlIDs = a.HasValue ? a.Value.Aliases : null,
            aggregation = a.HasValue ? a.Value.Aggregation : null,
            findOnEntry = "TRUE",
            query = q.Count() > 0
                        ? new Query()
                        {
                            matchType = q.Count() == 1 ? q.First().Item2.ToString("G") : null,
                            condition = q.Count() == 1 ? q.First().Item3 as List<Condition> : null,
                            complexQuery = q.Count() > 1
                                ? new List<ComplexQuery>(q.Select(r =>
                                    {
                                        return new ComplexQuery
                                        {
                                            andOr = r.Item1.HasValue ? r.Item1.Value.ToString("G") : AndOrCombinator.AND.ToString("G"),
                                            query = new Query()
                                            {
                                                matchType = r.Item2.ToString("G"),
                                                condition = r.Item3 as List<Condition>
                                            }
                                        };
                                    }))
                                : null
                        }
                        : null,
            formServiceDemo = o.HasValue && o.Value.Item1.HasValue ? "TRUE" : null,
            outputType = o.HasValue && o.Value.Item2.HasValue ? "VERSION2" : "GRID_DATA",
            maxPageSize = o.HasValue && o.Value.Item3.HasValue ? o.Value.Item3.Value : null
        },
         DataSubject.Parser,
         SkipWhitespaces
         .Then(
           QryOptions.Parser
           .Optional()),
          SkipWhitespaces
         .Then(
          DataAction.Parser
          .Optional()
         ),
         SkipWhitespaces
         .Then(
            QryOp.Queries
         ));
    }
}
