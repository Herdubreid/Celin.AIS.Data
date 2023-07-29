using Pidgin;
using System.Collections.Generic;
using static Pidgin.Parser;

namespace Celin.AIS.Data.Filter;

public enum Op
{
    EQ,
    NE,
    LT,
    LE,
    GE,
    GT,
}

public record Type(string Alias, Op Op, string Literal);

public class Parser : Base
{
    static readonly Parser<char, Op> EQ
        = Tok('=').ThenReturn(Op.EQ);
    static readonly Parser<char, Op> NE
        = Tok("!=").Or(Tok("<>")).ThenReturn(Op.NE);
    static readonly Parser<char, Op> LT
        = Tok('<').ThenReturn(Op.LT);
    static readonly Parser<char, Op> LE
        = Tok("<=").ThenReturn(Op.LE);
    static readonly Parser<char, Op> GT
        = Tok('>').ThenReturn(Op.GT);
    static readonly Parser<char, Op> GE
        = Tok(">=").ThenReturn(Op.GE);
    static readonly Parser<char, Op> OPS
        = OneOf(EQ, NE, LT, LE, GT, GE);
    public static Parser<char, Type> Condition
        => Map((l, o, r) => new Type(l, o, r),
            Alias.Parser,
            SkipWhitespaces
            .Then(OPS),
            LetterOrDigit.AtLeastOnceString()
            .Before(SkipWhitespaces));
    public static Parser<char, IEnumerable<Type>> Conditions
        => Condition
            .Separated(Whitespaces);
}
