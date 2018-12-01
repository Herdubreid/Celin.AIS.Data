﻿using System.Linq;
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
                     : null
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
         ));
    }
}
