﻿using Pidgin;
using System.Collections.Generic;
using static Pidgin.Parser;

namespace Celin.AIS.Data
{
    public class FileSelection
    {
        public static Parser<char, DatabrowserRequest> Parser
            => Map((s, a) => new DatabrowserRequest
            {
                targetName = s.ToUpper(),
                targetType = DatabrowserRequest.table,
                dataServiceType = a.IsAggregation
                    ? DatabrowserRequest.AGGREGATION
                    : DatabrowserRequest.BROWSE,
                returnControlIDs = a.Aliases,
                aggregation = a.Aggregation,
                findOnEntry = Request.TRUE
            },
            Skipper.Next(OneOf('f', 'F')
                .Then(LetterOrDigit.ManyString(), (p, b) => p + b)),
            Skipper.Next(DataAction.Parser));
        public static Parser<char, IEnumerable<DatabrowserRequest>> Array
            => Parser
               .Separated(Whitespaces)
               .Between(Char('['), SkipWhitespaces.Then(Char(']')))
               .Labelled("Data Selection Array");
    }
}
