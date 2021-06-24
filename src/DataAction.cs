using System.Collections.Generic;
using System.Linq;
using Pidgin;
using static Pidgin.Parser;
namespace Celin.AIS.Data
{
    public class DataAction
    {
        public bool IsAggregation { get; set; }
        public string Aliases { get; protected set; } = null;
        public Aggregation Aggregation { get; protected set; } = null;
        public static Parser<char, DataAction> Parser
        => Try(Aggregate.Array
               .Select(a => a.Any()
                       ? a.Aggregate(new DataAction()
                       {
                           IsAggregation = true,
                           Aggregation = new Aggregation
                           {
                               aggregations = new List<AggregationItem>(),
                               groupBy = new List<AggregationItem>(),
                               orderBy = new List<AggregationItem>()
                           }
                       }, (da, e) =>
                        {
                            switch (e.type)
                            {
                                case AggregationType.AGGREGATIONS:
                                    (da.Aggregation.aggregations as List<AggregationItem>).Add(e.item);
                                    break;
                                case AggregationType.GROUP_BY:
                                    (da.Aggregation.groupBy as List<AggregationItem>).Add(e.item);
                                    break;
                                case AggregationType.ORDER_BY:
                                    (da.Aggregation.orderBy as List<AggregationItem>).Add(e.item);
                                    break;
                            }
                            return da;
                        })
                       : new DataAction()
                      ))
            .Or(Select.Parser
                .Select(s => new DataAction()
                {
                    Aliases = s.list,
                    Aggregation = s.order.Count() > 0
                        ? new Aggregation { orderBy = s.order.ToList() }
                        : null
                }));
    }
}
