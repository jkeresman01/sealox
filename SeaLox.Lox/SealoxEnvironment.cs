namespace SeaLox.Lox;

public class SealoxEnvironment
{
    private readonly Dictionary<string, object> _values = new ();

    public void Define(string name, object value) => _values.Add(name, value);
    
    public object Get(Token name)
        => _values.TryGetValue(name.Lexeme, out var value)
            ? value
            : throw new RuntimeError(name, $"$Undefined variable '{name.Lexeme}'.");
}