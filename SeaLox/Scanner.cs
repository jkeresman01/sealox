public class Scanner(string source)
{
    private readonly string source = source;
    private readonly List<Token> tokens = new();
    
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
        
        tokens.Add(new Token
        {
            TokenType = TokenType.Eof,
            Lexeme = "",
            literal = null,
            Line = line
        });
        
        return tokens;
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
            default:
                SeaLox.Error(line, $"Unexpected character '{c}'.");
                break;
        }
    }

    private void AddToken(TokenType type) => AddToken(type, null);

    private void AddToken(TokenType type, object literal)
    {
        var text = source.Substring(start, current);
        tokens.Add(new Token
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