public class Scanner(string source)
{
    private readonly List<Token> _tokens = new();

    private static readonly Dictionary<string, TokenType> Keywords = new()
    {
        { "and", TokenType.And },
        { "class", TokenType.Class },
        { "else", TokenType.Else },
        { "false", TokenType.False },
        { "for", TokenType.For },
        { "fun", TokenType.Fun },
        { "if", TokenType.If },
        { "nil", TokenType.Nil },
        { "or", TokenType.Or },
        { "print", TokenType.Print },
        { "return", TokenType.Return },
        { "super", TokenType.Super },
        { "this", TokenType.This },
        { "true", TokenType.True },
        { "var", TokenType.Var },
        { "while", TokenType.While }
    };

    private int start = 0;
    private int current = 0;
    private int line = 1;

    public List<Token> ScanTokens()
    {
        while (!IsAtEnd())
        {
            start = current;
            ScanToken();
        }

        _tokens.Add(new Token
        {
            TokenType = TokenType.Eof,
            Lexeme = "",
            literal = null,
            Line = line
        });

        return _tokens;
    }

    private void ScanToken()
    {
        char c = Advance();

        switch (c)
        {
            case '(': AddToken(TokenType.LeftParen); break;
            case ')': AddToken(TokenType.RightParen); break;
            case '{': AddToken(TokenType.LeftBrace); break;
            case '}': AddToken(TokenType.RightBrace); break;
            case ',': AddToken(TokenType.Comma); break;
            case '.': AddToken(TokenType.Dot); break;
            case '-': AddToken(TokenType.Minus); break;
            case '+': AddToken(TokenType.Plus); break;
            case ';': AddToken(TokenType.Semicolon); break;
            case '*': AddToken(TokenType.Star); break;
            case '/':
                if (Match('/'))
                {
                    while (Peek() != '\n' && !IsAtEnd())
                    {
                        Advance();
                    }
                }
                else
                {
                    AddToken(TokenType.Slash);
                }

                break;
            case '!':
                AddToken(Match('=') ? TokenType.BangEqual : TokenType.Bang);
                break;
            case '=':
                AddToken(Match('=') ? TokenType.EqualEqual : TokenType.Equal);
                break;
            case '<':
                AddToken(Match('=') ? TokenType.LessEqual : TokenType.Less);
                break;
            case '>':
                AddToken(Match('=') ? TokenType.GreaterEqual : TokenType.Greater);
                break;
            case ' ':
            case '\r':
            case '\t':
                break;
            case '\n':
                line++;
                break;
            case '"': String(); break;
            default:
                if (IsDigit(c))
                {
                    Number();
                }
                else if (IsAlpha(c))
                {
                    Identifier();
                }
                else
                {
                    SeaLox.Error(line, $"Unexpected character '{c}'.");
                }

                break;
        }
    }

    private void Identifier()
    {
        while (IsAlphaNumeric(Peek()))
        {
            Advance();
        }

        var text = source.Substring(start, current - start);
        var tokenType = Keywords.GetValueOrDefault(text, TokenType.Identifier);

        AddToken(tokenType);
    }

    private bool IsAlphaNumeric(char c) => IsAlpha(c) || IsDigit(c);


    private bool IsAlpha(char c) => c is >= 'a' and <= 'z' or >= 'A' and <= 'Z' or '_';

    private void Number()
    {
        while (IsDigit(Peek()))
        {
            Advance();
        }

        if (Peek() == '.' && IsDigit(PeekNext()))
        {
            //consume the "."
            Advance();

            while (IsDigit(Peek()))
            {
                Advance();
            }
        }

        var number = source.Substring(start, current - start);

        AddToken(TokenType.Number, double.Parse(number));
    }

    private char PeekNext() => current + 1 < source.Length ? '\0' : source[current + 1];

    private bool IsDigit(char c) => c is >= '0' and <= '9';

    private void String()
    {
        while (Peek() != '"' && !IsAtEnd())
        {
            if (Peek() == '\n')
                line++;

            Advance();
        }

        if (IsAtEnd())
        {
            SeaLox.Error(line, "Unterminated string.");
            return;
        }

        Advance();

        var value = source.Substring(start + 1, current - start - 2);
        AddToken(TokenType.String, value);
    }

    private char Peek() => IsAtEnd() ? '\0' : source[current];

    private bool Match(char expected)
    {
        if (IsAtEnd())
        {
            return false;
        }

        if (source[current] != expected)
        {
            return false;
        }

        current++;

        return true;
    }

    private void AddToken(TokenType type) => AddToken(type, null);

    private void AddToken(TokenType type, object literal)
    {
        var text = source.Substring(start, current - start);
        
        _tokens.Add(new Token
        {
            TokenType = type,
            Lexeme = text,
            literal = literal,
            Line = line
        });
    }

    private char Advance() => source[current++];

    private bool IsAtEnd() => current >= source.Length;
}