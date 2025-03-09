using System.Text;
using StellarisModLib.Core.Objects;

namespace StellarisModLib.Core.Parser;

/// <summary>
/// Enhanced version that preserves formatting
/// </summary>
public class FormattingPreservingParser
{
    private List<Token> _tokens;
    private int _position;

    public FormattingPreservingParser(string input)
    {
        StellarisLexer lexer = new(input);
        _tokens = lexer.Tokenize();
    }

    public StellarisDocument Parse(string fileName = null)
    {
        StellarisDocument document = new() { FileName = fileName };
        document.Children = ParseTopLevelObjects();
        return document;
    }

    private List<StellarisObject> ParseTopLevelObjects()
    {
        List<StellarisObject> objects = [];
        StringBuilder leadingWhitespace = new();
            
        // Collect initial comments and whitespace
        while (_position < _tokens.Count && 
               (IsWhitespaceOrNewline(_tokens[_position]) || _tokens[_position].Type == TokenType.Comment))
        {
            if (_tokens[_position].Type == TokenType.Comment)
            {
                // Handle top-level comments as special objects
                StellarisProperty comment = new()
                {
                    Key = _tokens[_position].Value,
                    Value = null
                };
                objects.Add(comment);
            }
            else
            {
                leadingWhitespace.Append(_tokens[_position].Value);
            }
            _position++;
        }
            
        while (_position < _tokens.Count)
        {
            StellarisObject obj = ParseObject();
            if (obj != null)
            {
                // Apply collected whitespace to the first object
                if (objects.Count == 0 && leadingWhitespace.Length > 0)
                {
                    obj.LeadingWhitespace = leadingWhitespace.ToString();
                    leadingWhitespace.Clear();
                }
                objects.Add(obj);
            }
                
            // Skip any trailing whitespace or comments between top-level objects
            while (_position < _tokens.Count && 
                   (IsWhitespaceOrNewline(_tokens[_position]) || _tokens[_position].Type == TokenType.Comment))
            {
                if (_tokens[_position].Type == TokenType.Comment)
                {
                    if (objects.Count > 0)
                    {
                        objects[objects.Count - 1].Comments.Add(_tokens[_position].Value);
                    }
                }
                else if (objects.Count > 0)
                {
                    objects[objects.Count - 1].TrailingWhitespace += _tokens[_position].Value;
                }
                _position++;
            }
        }
            
        return objects;
    }

    private StellarisObject ParseObject()
    {
        StringBuilder leadingWhitespace = new();
            
        // Collect whitespace before this object
        while (_position < _tokens.Count && IsWhitespaceOrNewline(_tokens[_position]))
        {
            leadingWhitespace.Append(_tokens[_position].Value);
            _position++;
        }
            
        if (_position >= _tokens.Count)
            return null;
            
        // Handle identifiers (property keys or named blocks)
        if (_tokens[_position].Type == TokenType.Identifier)
        {
            string identifier = _tokens[_position].Value;
            _position++;
                
            // Collect whitespace after the identifier
            StringBuilder afterIdentifierSpace = new();
            while (_position < _tokens.Count && IsWhitespaceOrNewline(_tokens[_position]))
            {
                afterIdentifierSpace.Append(_tokens[_position].Value);
                _position++;
            }
                
            // Property assignment with equals
            if (_position < _tokens.Count && _tokens[_position].Type == TokenType.Equals)
            {
                _position++;
                    
                // Collect whitespace after equals
                StringBuilder afterEqualsSpace = new();
                while (_position < _tokens.Count && IsWhitespaceOrNewline(_tokens[_position]))
                {
                    afterEqualsSpace.Append(_tokens[_position].Value);
                    _position++;
                }
                    
                object value = ParseValue();
                    
                // Collect trailing whitespace
                StringBuilder trailingWhitespace = new();
                while (_position < _tokens.Count && IsWhitespaceOrNewline(_tokens[_position]))
                {
                    trailingWhitespace.Append(_tokens[_position].Value);
                    _position++;
                }
                    
                return new StellarisProperty
                {
                    LeadingWhitespace = leadingWhitespace.ToString(),
                    Key = identifier,
                    Value = value,
                    Operator = "=",
                    TrailingWhitespace = trailingWhitespace.ToString()
                };
            }
            // Named block
            else if (_position < _tokens.Count && _tokens[_position].Type == TokenType.OpenBrace)
            {
                StellarisBlock block = ParseBlock(identifier, leadingWhitespace.ToString(), afterIdentifierSpace.ToString());
                return block;
            }
        }
        // Conditional operators (AND, OR, NOT, NOR)
        else if (_tokens[_position].Type == TokenType.Operator)
        {
            string op = _tokens[_position].Value;
            _position++;
                
            // Collect whitespace after operator
            StringBuilder afterOpSpace = new();
            while (_position < _tokens.Count && IsWhitespaceOrNewline(_tokens[_position]))
            {
                afterOpSpace.Append(_tokens[_position].Value);
                _position++;
            }
                
            if (_position < _tokens.Count && _tokens[_position].Type == TokenType.OpenBrace)
            {
                StellarisCondition condition = new()
                {
                    LeadingWhitespace = leadingWhitespace.ToString(),
                    ConditionOperator = op,
                    OperatorWhitespace = afterOpSpace.ToString()
                };
                    
                _position++; // Skip opening brace
                    
                // Collect whitespace after opening brace
                StringBuilder afterBraceSpace = new();
                while (_position < _tokens.Count && IsWhitespaceOrNewline(_tokens[_position]))
                {
                    afterBraceSpace.Append(_tokens[_position].Value);
                    _position++;
                }
                condition.OpenBraceWhitespace = afterBraceSpace.ToString();
                    
                // Parse operands
                while (_position < _tokens.Count && _tokens[_position].Type != TokenType.CloseBrace)
                {
                    StellarisObject operand = ParseObject();
                    if (operand != null)
                        condition.Operands.Add(operand);
                }
                    
                // Collect whitespace before closing brace
                StringBuilder beforeCloseBraceSpace = new();
                while (_position > 0 && _position - 1 < _tokens.Count && 
                       IsWhitespaceOrNewline(_tokens[_position - 1]))
                {
                    beforeCloseBraceSpace.Insert(0, _tokens[_position - 1].Value);
                    _position--;
                }
                    
                if (_position < _tokens.Count && _tokens[_position].Type == TokenType.CloseBrace)
                {
                    _position++; // Skip closing brace
                        
                    // Collect trailing whitespace
                    StringBuilder trailingWhitespace = new();
                    while (_position < _tokens.Count && IsWhitespaceOrNewline(_tokens[_position]))
                    {
                        trailingWhitespace.Append(_tokens[_position].Value);
                        _position++;
                    }
                    condition.CloseBraceWhitespace = beforeCloseBraceSpace.ToString();
                    condition.TrailingWhitespace = trailingWhitespace.ToString();
                }
                    
                return condition;
            }
        }
            
        // If we reach here, there was a syntax error or unrecognized structure
        _position++; // Skip the problematic token
        return null;
    }

    private StellarisBlock ParseBlock(string name, string leadingWhitespace, string afterNameSpace)
    {
        StellarisBlock block = new()
        {
            LeadingWhitespace = leadingWhitespace,
            Name = name,
            OpenBraceWhitespace = afterNameSpace
        };
            
        _position++; // Skip opening brace
            
        // Collect whitespace after opening brace
        StringBuilder afterBraceSpace = new();
        while (_position < _tokens.Count && IsWhitespaceOrNewline(_tokens[_position]))
        {
            afterBraceSpace.Append(_tokens[_position].Value);
            _position++;
        }
            
        // Parse child elements
        while (_position < _tokens.Count && _tokens[_position].Type != TokenType.CloseBrace)
        {
            StellarisObject child = ParseObject();
            if (child != null)
                block.Children.Add(child);
        }
            
        // Collect whitespace before closing brace
        StringBuilder beforeCloseBraceSpace = new();
        while (_position > 0 && _position - 1 < _tokens.Count && 
               IsWhitespaceOrNewline(_tokens[_position - 1]))
        {
            beforeCloseBraceSpace.Insert(0, _tokens[_position - 1].Value);
            _position--;
        }
            
        if (_position < _tokens.Count && _tokens[_position].Type == TokenType.CloseBrace)
        {
            _position++; // Skip closing brace
                
            // Collect trailing whitespace
            StringBuilder trailingWhitespace = new();
            while (_position < _tokens.Count && IsWhitespaceOrNewline(_tokens[_position]))
            {
                trailingWhitespace.Append(_tokens[_position].Value);
                _position++;
            }
                
            block.CloseBraceWhitespace = beforeCloseBraceSpace.ToString();
            block.TrailingWhitespace = trailingWhitespace.ToString();
        }
            
        return block;
    }

    private object ParseValue()
    {
        if (_position >= _tokens.Count)
            return null;
            
        if (_tokens[_position].Type == TokenType.String || 
            _tokens[_position].Type == TokenType.Number || 
            _tokens[_position].Type == TokenType.Identifier)
        {
            string value = _tokens[_position].Value;
            _position++;
            return value;
        }
        else if (_tokens[_position].Type == TokenType.OpenBrace)
        {
            return ParseBlock(null, "", "");
        }
        else if (_tokens[_position].Type == TokenType.Operator)
        {
            return ParseObject(); // Parse conditional expression
        }
            
        _position++; // Skip unrecognized token
        return null;
    }

    private bool IsWhitespaceOrNewline(Token token)
    {
        return token.Type == TokenType.Whitespace || token.Type == TokenType.NewLine;
    }
}