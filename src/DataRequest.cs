using Pidgin;
using System.Collections.Generic;
using static Pidgin.Parser;
namespace Celin.AIS.Data
{
    public class DataRequest
    {
        public static Parser<char, DatabrowserRequest> Parser
        => Map((s, a, q, mx) => new DatabrowserRequest()
        {
            outputType = "GRID_DATA",
            targetName = s.Name.ToUpper(),
            targetType = s.Type,
            dataServiceType = a.HasValue && a.Value.Aggregation != null ? "AGGREGATION" : "BROWSE",
            returnControlIDs = a.HasValue ? a.Value.Aliases : null,
            aggregation = a.HasValue ? a.Value.Aggregation : null,
            findOnEntry = "TRUE",
            query = q.HasValue
                        ? new Query()
                        {
                            matchType = q.Value.Item1.ToString("G"),
                            condition = q.Value.Item2 as List<Condition>
                        }
                     : null,
            maxPageSize = mx.HasValue ? mx.Value : null
        },
         DataSubject.Parser,
         SkipWhitespaces
         .Then(
          DataAction.Parser
          .Optional()
         ),
         SkipWhitespaces
         .Then(
          QryOp.Query
          .Optional()
         ),
         SkipWhitespaces
         .Then(
           MaxPageOption.Parser
           .Optional()
          ));
    }
}
