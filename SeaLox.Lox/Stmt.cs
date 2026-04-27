namespace SeaLox.Lox;

using System.Collections.Generic;

abstract class Stmt
{
    public interface IVisitor<R>
    {
        R VisitPrintStmt(Print stmt);
        R VisitVarStmt(Var stmt);
    }

    public class Print : Stmt
    {
        public Expr expression { get; set; }


        public override R Accept<R>(IVisitor<R> visitor)
        {
            return visitor.VisitPrintStmt(this);
        }
    }

    public class Var : Stmt
    {
        public Token name { get; set; }
        public Expr initalizer { get; set; }


        public override R Accept<R>(IVisitor<R> visitor)
        {
            return visitor.VisitVarStmt(this);
        }
    }


    public abstract R Accept<R>(IVisitor<R> visitor);
}