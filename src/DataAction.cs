using System.Linq;
using Pidgin;
using static Pidgin.Parser;
namespace Celin.AIS.Data
{
    public class DataAction
    {
        public string Aliases { get; protected set; } = null;
        public Aggregation Aggregation { get; protected set; } = null;
        public static Parser<char, DataAction> Parser
        => Try(List.Parser
               .Select(l => new DataAction() { Aliases = l })
           .Or(Aggregate.Array
               .Select(a => a.Any()
                       ? a.Aggregate(new DataAction()
                       {
                           Aggregation = new Aggregation()
                       }, (da, e) =>
                        {
                            switch (e.type)
                            {
                                case AggregationType.AGGREGATIONS:
                                    da.Aggregation.aggregations.Add(e.item);
                                    break;
                                case AggregationType.GROUP_BY:
                                    da.Aggregation.groupBy.Add(e.item);
                                    break;
                                case AggregationType.ORDER_BY:
                                    da.Aggregation.orderBy.Add(e.item);
                                    break;
                            }
                            return da;
                        })
                       : new DataAction()
                      )));
    }
}
