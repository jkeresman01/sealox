public class Token
{
    public TokenType TokenType { get; set; }
    public string Lexeme { get; set; }
    public object? literal { get; set; }
    public int Line { get; set; }

    public override string ToString() => $"{TokenType}: {Lexeme}: {literal}";
}