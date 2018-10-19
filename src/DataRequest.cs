using System.Collections.Generic;
using Pidgin;
using static Pidgin.Parser;
namespace Celin.AIS.Data
{
    public class DataRequest
    {
        public static Parser<char, DatabrowserRequest> Parser
        => Map((s, a, q) => new DatabrowserRequest()
        {
            targetName = s.Name,
            targetType = s.Type,
            dataServiceType = a.HasValue && a.Value.Aggregation != null ? "AGGREGATE" : "BROWSE",
            returnControlIDs = a.HasValue ? a.Value.Aliases : null,
            aggregation = a.HasValue ? a.Value.Aggregation : null,
            query = q.HasValue
                     ? new Query() { condition = q.Value as List<Condition> }
                     : null
        },
         DataSubject.Parser,
         Try(Whitespace)
         .Then(Try(DataAction.Parser)
                .Optional()),
         QryOp.Array
         .Optional());
    }
}
