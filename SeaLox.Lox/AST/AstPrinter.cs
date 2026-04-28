using System.Text;

namespace SeaLox.Lox;

public class AstPrinter : Expr.IVisitor<string>
{
    public string Print(Expr expr) => expr.Accept(this);
    
    public string VisitBinaryExpr(Expr.Binary expr)
        => Parenthesize(expr.Operator.Lexeme!, expr.Left, expr.Right);
    
    public string VisitGroupingExpr(Expr.Grouping expr)
        => Parenthesize("group",  expr.Expression);

    public string VisitLiteralExpr(Expr.Literal expr)
        => expr.Value is null ? "nil" : expr.Value.ToString()!;

    public string VisitUnaryExpr(Expr.Unary expr)
        => Parenthesize(expr.Operator.Lexeme!, expr.Right);
    
    private string Parenthesize(string name, params Expr[] exprs)
    {
        var sb = new StringBuilder();
        
        sb.Append("(").Append(name);

        foreach (Expr expr in exprs)
        {
            sb.Append(" ");
            sb.Append(expr.Accept(this));
        }

        sb.Append(")");
        
        return sb.ToString();
    }
    
    public string VisitLogicalExpr(Expr.Logical expr) => throw new NotImplementedException();

    public string VisitVariableExpr(Expr.Variable expr) => throw new NotImplementedException();
    
    public string VisitAssignExpr(Expr.Assign expr) => throw new NotImplementedException();
   

}