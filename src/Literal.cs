using Pidgin;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static Pidgin.Parser;

namespace Celin.AIS.Data;

public delegate string GetVariable(string name, int? index);
public class Literal
{
    public static GetVariable GetVariable { get; set; }
    public static readonly Regex VARIABLE = new Regex(@"^%VARIABLE:(\w+)(?>\[(\d+)\])?:%");
    static readonly Parser<char, int> Index =
        Try(DecimalNum.Between(Char('['), Char(']')));
    static readonly Parser<char, string> VariableName =
        Try(Char('@'))
            .Then(LetterOrDigit
                  .ManyString(), (h, t) => t);
    static readonly Parser<char, string> Variable =
        Map((name, index) => GetVariable(name, (index.HasValue ? index.Value : null)),
            VariableName,
            Index.Optional());
    public static readonly Parser<char, string> Plain =
        Try(LetterOrDigit)
            .Then(LetterOrDigit
                  .ManyString(), (h, t) => h + t);
    public static readonly Parser<char, string> DoubleQuoted =
        AnyCharExcept('"')
            .ManyString()
            .Between(Char('"'));
    public static readonly Parser<char, string> SingleQuoted =
        AnyCharExcept('\'')
            .ManyString()
            .Between(Char('\''));
    public static Parser<char, string> Parser
        => OneOf(Plain, Variable, DoubleQuoted, SingleQuoted)
           .Labelled("Literal");
    public static Parser<char, IEnumerable<string>> Array
        => Parser
           .SeparatedAtLeastOnce(Char(','))
           .Labelled("Literal Array");
}
