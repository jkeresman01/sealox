namespace SeaLox.Lox;

public abstract class Stmt
{
    public interface IVisitor<R>
    {
        R VisitExpressionStmt(Expression stmt);
        R VisitPrintStmt(Print stmt);
        R VisitVarStmt(Var stmt);
    }

    public class Expression : Stmt
    {
        public Expr Expr { get; set; }


        public override R Accept<R>(IVisitor<R> visitor)
        {
            return visitor.VisitExpressionStmt(this);
        }
    }

    public class Print : Stmt
    {
        public Expr Expression { get; set; }


        public override R Accept<R>(IVisitor<R> visitor)
        {
            return visitor.VisitPrintStmt(this);
        }
    }

    public class Var : Stmt
    {
        public Token name { get; set; }
        public Expr Initalizer { get; set; }


        public override R Accept<R>(IVisitor<R> visitor)
        {
            return visitor.VisitVarStmt(this);
        }
    }


    public abstract R Accept<R>(IVisitor<R> visitor);
}