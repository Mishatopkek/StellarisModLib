using StellarisModLib.Core.Objects;

namespace StellarisModLib.Core.Parser;

public class StellarisParser
{
    private List<Token> _tokens;
    private int _position;
    private Token CurrentToken => _position < _tokens.Count ? _tokens[_position] : null;

    public StellarisParser(List<Token> tokens)
    {
        _tokens = tokens.Where(t => t.Type != TokenType.Whitespace && t.Type != TokenType.NewLine).ToList();
    }

    public StellarisDocument Parse(string fileName = null)
    {
        StellarisDocument document = new() { FileName = fileName };
            
        while (_position < _tokens.Count)
        {
            document.Children.Add(ParseObject());
        }
            
        return document;
    }

    private StellarisObject ParseObject()
    {
        if (CurrentToken.Type == TokenType.Identifier)
        {
            string identifier = CurrentToken.Value;
            _position++;
                
            // Property assignment
            if (_position < _tokens.Count && CurrentToken.Type == TokenType.Equals)
            {
                _position++;
                object value = ParseValue();
                    
                return new StellarisProperty
                {
                    Key = identifier,
                    Value = value
                };
            }
            // Named block
            else if (_position < _tokens.Count && CurrentToken.Type == TokenType.OpenBrace)
            {
                _position++;
                StellarisBlock block = new() { Name = identifier };
                    
                while (_position < _tokens.Count && CurrentToken.Type != TokenType.CloseBrace)
                {
                    block.Children.Add(ParseObject());
                }
                    
                if (_position < _tokens.Count)
                    _position++; // Skip closing brace
                    
                return block;
            }
        }
        // Conditional expression
        else if (CurrentToken.Type == TokenType.Operator)
        {
            string op = CurrentToken.Value;
            _position++;
                
            if (_position < _tokens.Count && CurrentToken.Type == TokenType.OpenBrace)
            {
                _position++;
                StellarisCondition condition = new() { ConditionOperator = op };
                    
                while (_position < _tokens.Count && CurrentToken.Type != TokenType.CloseBrace)
                {
                    condition.Operands.Add(ParseObject());
                }
                    
                if (_position < _tokens.Count)
                    _position++; // Skip closing brace
                    
                return condition;
            }
        }
            
        // If we reach here, there was a syntax error
        throw new FormatException($"Unexpected token: {CurrentToken}");
    }

    private object ParseValue()
    {
        if (CurrentToken.Type == TokenType.String)
        {
            string value = CurrentToken.Value;
            _position++;
            return value;
        }
        else if (CurrentToken.Type == TokenType.Number)
        {
            string value = CurrentToken.Value;
            _position++;
                
            // Try to parse as a number, but keep as string if it fails
            if (double.TryParse(value, out double numValue))
                return numValue;
            return value;
        }
        else if (CurrentToken.Type == TokenType.OpenBrace)
        {
            _position++;
            StellarisBlock block = new();
                
            while (_position < _tokens.Count && CurrentToken.Type != TokenType.CloseBrace)
            {
                block.Children.Add(ParseObject());
            }
                
            if (_position < _tokens.Count)
                _position++; // Skip closing brace
                
            return block;
        }
        else if (CurrentToken.Type == TokenType.Operator)
        {
            return ParseObject(); // This will parse the condition
        }
        else if (CurrentToken.Type == TokenType.Identifier)
        {
            string value = CurrentToken.Value;
            _position++;
            return value;
        }
            
        throw new FormatException($"Unexpected value token: {CurrentToken}");
    }
}