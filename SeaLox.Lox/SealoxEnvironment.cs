namespace SeaLox.Lox;

public class SealoxEnvironment
{
    private readonly Dictionary<string, object> _values = new ();
    public SealoxEnvironment? Enclosing { get; set; }

    public SealoxEnvironment()
    {
        Enclosing = null;
    }

    public SealoxEnvironment(SealoxEnvironment enclosing)
    {
        Enclosing = enclosing;
    }

    public void Define(string name, object value) => _values.Add(name, value);
    
    public object Get(Token name)
    {
        if (_values.TryGetValue(name.Lexeme, out var value))
        {
            return value;
        }


        if (Enclosing != null)
        {
            return Enclosing.Get(name);
        }
        
        throw new RuntimeError(name, $"$Undefined variable '{name.Lexeme}'.");
    }

    public void Assign(Token name, object value)
    {
        if (_values.ContainsKey(name.Lexeme))
        {
            _values[name.Lexeme] = value;
            return;
        }

        if (Enclosing != null)
        {
            Enclosing.Assign(name, value);
            return;
        }
        
        throw new RuntimeError(name, $"Undefined variable '{name.Lexeme}'.");
    }
}