using Pidgin;
using System.Collections.Generic;
using static Pidgin.Parser;

namespace Celin.AIS.Data
{
    public class DataSelection
    {
        public static Parser<char, DatabrowserRequest> Parser
            => Map((s, a) => new DatabrowserRequest
            {
                targetName = s.Name.ToUpper(),
                dataServiceType = DatabrowserRequest.BROWSE,
                findOnEntry = Request.TRUE,
                targetType = s.Type,
                returnControlIDs = a.list
            },
            Skipper.Next(DataSubject.Parser),
            Skipper.Next(Select.Parser));
        public static Parser<char, IEnumerable<DatabrowserRequest>> Array
            => Parser
               .Separated(Whitespaces)
               .Between(Char('('), SkipWhitespaces.Then(Char(')')))
               .Labelled("Data Selection Array");
    }
}
