using System;
using System.Collections.Generic;
using System.Linq;
using Pidgin;
using static Pidgin.Parser;

namespace Celin.AIS.Data
{
    using QueryDef = ValueTuple<Maybe<AndOrCombinator>, QueryType, IEnumerable<Condition>>;
    public enum AndOrCombinator
    {
        AND,
        OR
    }
    public enum QueryType
    {
        MATCH_ALL,
        MATCH_ANY
    }
    public enum QueryOperator
    {
        BETWEEN,
        LIST,
        EQUAL,
        NOT_EQUAL,
        LESS,
        LESS_EQUAL,
        GREATER,
        GREATER_EQUAL,
        STR_START_WITH,
        STR_END_WITH,
        STR_CONTAIN,
        STR_BLANK,
        STR_NOT_BLANK
    }
    public class QryOp
    {
        protected static readonly Parser<char, AndOrCombinator> AND =
            String("and").ThenReturn(AndOrCombinator.AND);
        protected static readonly Parser<char, AndOrCombinator> OR =
            String("or").ThenReturn(AndOrCombinator.OR);
        protected static readonly Parser<char, QueryType> MATCH_ANY =
            String("any").ThenReturn(QueryType.MATCH_ANY);
        protected static readonly Parser<char, QueryType> MATCH_ALL =
            String("all").ThenReturn(QueryType.MATCH_ALL);
        protected static readonly Parser<char, QueryOperator> BETWEEN =
            String("bw").ThenReturn(QueryOperator.BETWEEN);
        protected static readonly Parser<char, QueryOperator> LIST =
            String("in").ThenReturn(QueryOperator.LIST);
        protected static readonly Parser<char, QueryOperator> NOT_EQUAL =
            Try(String("<>")).ThenReturn(QueryOperator.NOT_EQUAL);
        protected static readonly Parser<char, QueryOperator> LESS_EQUAL =
            Try(String("<=")).ThenReturn(QueryOperator.LESS_EQUAL);
        protected static readonly Parser<char, QueryOperator> GREATER_EQUAL =
            Try(String(">=")).ThenReturn(QueryOperator.GREATER_EQUAL);
        protected static readonly Parser<char, QueryOperator> EQUAL =
            Char('=').ThenReturn(QueryOperator.EQUAL);
        protected static readonly Parser<char, QueryOperator> GREATER =
            Char('>').ThenReturn(QueryOperator.GREATER);
        protected static readonly Parser<char, QueryOperator> LESS =
            Char('<').ThenReturn(QueryOperator.LESS);
        protected static readonly Parser<char, QueryOperator> STR_START_WITH =
            Char('^').ThenReturn(QueryOperator.STR_START_WITH);
        protected static readonly Parser<char, QueryOperator> STR_END_WITH =
            Char('$').ThenReturn(QueryOperator.STR_END_WITH);
        protected static readonly Parser<char, QueryOperator> STR_CONTAIN =
            Char('?').ThenReturn(QueryOperator.STR_CONTAIN);
        protected static readonly Parser<char, QueryOperator> STR_BLANK =
            Char('_').ThenReturn(QueryOperator.STR_BLANK);
        protected static readonly Parser<char, QueryOperator> STR_NOT_BLANK =
            Char('!').ThenReturn(QueryOperator.STR_NOT_BLANK);
        protected static readonly Parser<char, QueryOperator> QUERY_OPERATOR =
            OneOf(
                BETWEEN,
                LIST,
                NOT_EQUAL,
                LESS_EQUAL,
                GREATER_EQUAL,
                EQUAL,
                GREATER,
                LESS,
                STR_START_WITH,
                STR_END_WITH,
                STR_CONTAIN,
                STR_BLANK,
                STR_NOT_BLANK)
                .Between(SkipWhitespaces);
        public static Parser<char, Condition> DataParameter
        => Map((l, o, r) => new Condition()
        {
            controlId = l.ToString(),
            @operator = o.ToString("G"),
            value = r.Select(e => new Value()
            {
                content = e,
                specialValueId = Value.LITERAL
            }).ToArray()
        },
         SkipWhitespaces
         .Then(Alias.Parser),
         QUERY_OPERATOR,
         Literal.Array
        ).Labelled("Query Condition");
        public static Parser<char, IEnumerable<Condition>> DataParameters
        => Try(DataParameter.Before(SkipWhitespaces))
           .Separated(Whitespace)
           .Labelled("Query List");
        public static Parser<char, QueryDef> DataQuery
            => Map((t, c, x) => new QueryDef(x, t, c),
            Try(MATCH_ANY)
            .Or(MATCH_ALL),
            SkipWhitespaces
            .Then(
             DataParameters
             .Between(Char('('), SkipWhitespaces.Then(Char(')')))),
            SkipWhitespaces
            .Then(Try(AND)
            .Or(OR)
            .Optional()));
        public static Parser<char, IEnumerable<QueryDef>> DataQueries
            => DataQuery
               .Separated(Whitespaces);
        static readonly Parser<char, string> ControlId =
            Digit
            .ManyString()
            .Then(Digit
                .AtLeastOnceString()
                .Between(Char('['), SkipWhitespaces.Then(Char(']'))).Optional(), (pre, post)
                => (string.IsNullOrEmpty(pre) ? "1" : pre) + (post.HasValue ? $"[{post.Value}]" : string.Empty));
        public static Parser<char, Condition> FormParameter
        => Map((l, o, r) => new Condition()
        {
            controlId = l.ToString(),
            @operator = o.ToString("G"),
            value = r.Select(e => new Value()
            {
                content = e,
                specialValueId = Value.LITERAL
            }).ToArray()
        },
         SkipWhitespaces
         .Then(ControlId),
         QUERY_OPERATOR,
         Literal.Array
        ).Labelled("Query Condition");
        public static Parser<char, IEnumerable<Condition>> FormParameters
        => Try(FormParameter)
           .Separated(Whitespace)
           .Labelled("Query List");
        public static Parser<char, QueryDef> FormQuery
            => Map((t, c, x) => new QueryDef(x, t, c),
            Try(MATCH_ANY)
            .Or(MATCH_ALL),
            SkipWhitespaces
            .Then(
             FormParameters
             .Between(Char('('), SkipWhitespaces.Then(Char(')')))),
            SkipWhitespaces
            .Then(Try(AND)
            .Or(OR)
            .Optional()));
        public static Parser<char, IEnumerable<QueryDef>> FormQueries
            => FormQuery
               .Separated(Whitespaces);
    }
}
