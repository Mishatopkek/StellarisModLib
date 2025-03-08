namespace StellarisModLib.Core.Objects;

/// <summary>
/// Represents the types of tokens in the Stellaris syntax
/// </summary>
public enum TokenType
{
    Identifier,
    String,
    Number,
    Equals,
    OpenBrace,
    CloseBrace,
    OpenBracket,
    CloseBracket,
    Operator, // AND, OR, NOT, NOR
    Comment,
    Whitespace,
    NewLine
}