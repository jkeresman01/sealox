namespace SeaLox.Lox;

public class Interpreter : Expr.IVisitor<object>, Stmt.IVisitor<object>
{
    private static SealoxEnvironment _environment = new();
    
    public void Interpret(IEnumerable<Stmt> statements)
    {
        try
        {
            foreach (var stmt in statements)
            {
                Execute(stmt);
            }
        }
        catch (RuntimeError error)
        {
            SeaLox.RuntimeError(error);
        }
    }

    private void Execute(Stmt stmt) => stmt.Accept(this);

    private string? Stringify(object? obj)
    {
        if (obj == null)
        {
            return "nil";
        }

        if (obj is string str)
        {
            if (str.EndsWith(".0"))
            {
                str = str.Substring(0, str.Length - 2);
            }

            return str;
        }

        return obj.ToString();
    }

    public object VisitBinaryExpr(Expr.Binary expr)
    {
        var left = Evaluate(expr.Left);
        var right = Evaluate(expr.Right);

        switch (expr.Operator.TokenType)
        {
            case TokenType.Minus:
                CheckNumberOperand(expr.Operator, right);
                return (double)left - (double)right;
            case TokenType.Slash:
                CheckNumberOperands(expr.Operator, left, right);
                return (double)left / (double)right;
            case TokenType.Star:
                CheckNumberOperands(expr.Operator, left, right);
                return (double)left * (double)right;
            case TokenType.Plus:
                return left switch
                {
                    double doubleLeft when right is double doubleRight
                        => doubleLeft + doubleRight,
                    string strLeft when right is string strRight
                        => strLeft + strRight,
                    _ => throw new RuntimeError(expr.Operator, "Operands must be two numbers or two strings!")
                };
            case TokenType.Greater:
                CheckNumberOperands(expr.Operator, left, right);
                return (double)left > (double)right;
            case TokenType.GreaterEqual:
                CheckNumberOperands(expr.Operator, left, right);
                return (double)left >= (double)right;
            case TokenType.Less:
                CheckNumberOperands(expr.Operator, left, right);
                return (double)left < (double)right;
            case TokenType.LessEqual:
                CheckNumberOperands(expr.Operator, left, right);
                return (double)left <= (double)right;
            case TokenType.BangEqual:
                return !IsEqual(left, right);
            case TokenType.EqualEqual:
                return IsEqual(left, right);
        }

        return null;
    }

    private void CheckNumberOperand(Token token, object operand)
    {
        if (operand is double)
        {
            return;
        }

        throw new RuntimeError(token, "Operand must be number");
    }

    private void CheckNumberOperands(Token op, object left, object right)
    {
        if (left is double && right is double)
        {
            return;
        }

        throw new RuntimeError(op, "Operands must be number");
    }

    private bool IsEqual(object? left, object? right)
    {
        if (left is null && right is null)
        {
            return true;
        }

        return left is not null && left.Equals(right);
    }


    public object VisitGroupingExpr(Expr.Grouping expr) => Evaluate(expr.Expression);
    
    public object VisitLiteralExpr(Expr.Literal expr) => expr.Value!;
    
    public object VisitVariableExpr(Expr.Variable expr) => _environment.Get(expr.Name);
    
    public object? VisitUnaryExpr(Expr.Unary expr)
    {
        var right = Evaluate(expr.Right);

        switch (expr.Operator.TokenType)
        {
            case TokenType.Minus:
                return -(double)right;
            case TokenType.Bang:
                return !IsTruthy(right);
        }

        return null;
    }

    private bool IsTruthy(object? obj)
    {
        if (obj == null)
        {
            return false;
        }

        if (obj is bool boolean)
        {
            return boolean;
        }

        return true;
    }

    private object Evaluate(Expr expr) => expr.Accept(this);
    
    public object VisitExpressionStmt(Stmt.Expression stmt) => Evaluate(stmt.Expr);

    public object VisitPrintStmt(Stmt.Print stmt)
    {
        var value = Evaluate(stmt.Expression);
        Console.WriteLine(Stringify(value));
        return null;
    }
    

    public object VisitVarStmt(Stmt.Var stmt)
    {
        object value = null;

        if (stmt.Initalizer != null)
        {
            
            value = Evaluate(stmt.Initalizer);
        }
        
        _environment.Define(stmt.Name.Lexeme, value);
        return null;
    }
}