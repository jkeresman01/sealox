namespace SeaLox.Lox;

using System.Collections.Generic;

public abstract class Stmt
{
    public interface IVisitor<R>
    {
        R VisitExpressionStmt(Expression stmt);
        R VisitIfStmt(If stmt);
        R VisitPrintStmt(Print stmt);
        R VisitBlockStmt(Block stmt);
        R VisitVarStmt(Var stmt);
        R VisitWhileStmt(While stmt);
    }

    public class Expression : Stmt
    {
        public Expr Expr { get; set; }


        public override R Accept<R>(IVisitor<R> visitor)
        {
            return visitor.VisitExpressionStmt(this);
        }
    }

    public class If : Stmt
    {
        public Expr Condition { get; set; }
        public Stmt ThenBranch { get; set; }
        public Stmt ElseBranch { get; set; }


        public override R Accept<R>(IVisitor<R> visitor)
        {
            return visitor.VisitIfStmt(this);
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

    public class Block : Stmt
    {
        public List<Stmt> Statements { get; set; }


        public override R Accept<R>(IVisitor<R> visitor)
        {
            return visitor.VisitBlockStmt(this);
        }
    }

    public class Var : Stmt
    {
        public Token Name { get; set; }
        public Expr Initalizer { get; set; }


        public override R Accept<R>(IVisitor<R> visitor)
        {
            return visitor.VisitVarStmt(this);
        }
    }

    public class While : Stmt
    {
        public Expr Condition { get; set; }
        public Stmt Body { get; set; }


        public override R Accept<R>(IVisitor<R> visitor)
        {
            return visitor.VisitWhileStmt(this);
        }
    }


    public abstract R Accept<R>(IVisitor<R> visitor);
}