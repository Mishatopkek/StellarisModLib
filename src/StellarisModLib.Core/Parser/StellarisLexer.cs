using System.Text;
using StellarisModLib.Core.Objects;

namespace StellarisModLib.Core.Parser;

public class StellarisLexer
{
    private string _input;
    private int _position;
    private int _line = 1;
    private int _column = 1;

    private static readonly Dictionary<string, TokenType> _keywords = new()
    {
        { "AND", TokenType.Operator },
        { "OR", TokenType.Operator },
        { "NOT", TokenType.Operator },
        { "NOR", TokenType.Operator }
    };

    public StellarisLexer(string input)
    {
        _input = input;
    }

    public List<Token> Tokenize()
    {
        List<Token> tokens = [];
            
        while (_position < _input.Length)
        {
            char currentChar = _input[_position];
                
            // Whitespace
            if (char.IsWhiteSpace(currentChar) && currentChar != '\n')
            {
                string whitespace = ReadWhile(c => char.IsWhiteSpace(c) && c != '\n');
                tokens.Add(new Token { Type = TokenType.Whitespace, Value = whitespace, Line = _line, Column = _column - whitespace.Length });
            }
            // New line
            else if (currentChar == '\n' || (currentChar == '\r' && PeekNextChar() == '\n'))
            {
                _line++;
                _column = 1;
                    
                if (currentChar == '\r')
                {
                    _position++;
                }
                    
                _position++;
                tokens.Add(new Token { Type = TokenType.NewLine, Value = Environment.NewLine, Line = _line - 1, Column = _column });
            }
            // Comments
            else if (currentChar == '#')
            {
                string comment = ReadWhile(c => c != '\n' && c != '\r');
                tokens.Add(new Token { Type = TokenType.Comment, Value = comment, Line = _line, Column = _column - comment.Length });
            }
            // Strings
            else if (currentChar == '"')
            {
                int startColumn = _column;
                _position++; // Skip opening quote
                _column++;
                    
                StringBuilder sb = new();
                bool escaped = false;
                    
                while (_position < _input.Length)
                {
                    char c = _input[_position];
                        
                    if (escaped)
                    {
                        sb.Append(c);
                        escaped = false;
                    }
                    else if (c == '\\')
                    {
                        escaped = true;
                    }
                    else if (c == '"')
                    {
                        break;
                    }
                    else
                    {
                        sb.Append(c);
                    }
                        
                    _position++;
                    _column++;
                }
                    
                if (_position < _input.Length) // Skip closing quote
                {
                    _position++;
                    _column++;
                }
                    
                tokens.Add(new Token { Type = TokenType.String, Value = $"\"{sb}\"", Line = _line, Column = startColumn });
            }
            // Numbers
            else if (char.IsDigit(currentChar) || (currentChar == '-' && char.IsDigit(PeekNextChar())))
            {
                string number = ReadWhile(c => char.IsDigit(c) || c == '.' || c == '-');
                tokens.Add(new Token { Type = TokenType.Number, Value = number, Line = _line, Column = _column - number.Length });
            }
            // Identifiers and keywords
            else if (char.IsLetter(currentChar) || currentChar == '_')
            {
                string identifier = ReadWhile(c => char.IsLetterOrDigit(c) || c == '_');
                    
                if (_keywords.TryGetValue(identifier, out TokenType tokenType))
                {
                    tokens.Add(new Token { Type = tokenType, Value = identifier, Line = _line, Column = _column - identifier.Length });
                }
                else
                {
                    tokens.Add(new Token { Type = TokenType.Identifier, Value = identifier, Line = _line, Column = _column - identifier.Length });
                }
            }
            // Special characters
            else
            {
                switch (currentChar)
                {
                    case '=':
                        tokens.Add(new Token { Type = TokenType.Equals, Value = "=", Line = _line, Column = _column });
                        break;
                    case '{':
                        tokens.Add(new Token { Type = TokenType.OpenBrace, Value = "{", Line = _line, Column = _column });
                        break;
                    case '}':
                        tokens.Add(new Token { Type = TokenType.CloseBrace, Value = "}", Line = _line, Column = _column });
                        break;
                    case '[':
                        tokens.Add(new Token { Type = TokenType.OpenBracket, Value = "[", Line = _line, Column = _column });
                        break;
                    case ']':
                        tokens.Add(new Token { Type = TokenType.CloseBracket, Value = "]", Line = _line, Column = _column });
                        break;
                    default:
                        // Handle other special characters or operators
                        if (currentChar == '<' || currentChar == '>' || currentChar == '!')
                        {
                            string op = ReadWhile(c => c == '<' || c == '>' || c == '=' || c == '!');
                            tokens.Add(new Token { Type = TokenType.Operator, Value = op, Line = _line, Column = _column - op.Length });
                        }
                        else
                        {
                            // Skip unrecognized characters
                            _position++;
                            _column++;
                        }
                        break;
                }
            }
        }
            
        return tokens;
    }

    private char PeekNextChar()
    {
        if (_position + 1 < _input.Length)
            return _input[_position + 1];
        return '\0';
    }

    private string ReadWhile(Func<char, bool> predicate)
    {
        int start = _position;
            
        while (_position < _input.Length && predicate(_input[_position]))
        {
            _position++;
            _column++;
        }
            
        return _input.Substring(start, _position - start);
    }
}