namespace SeaLox.Lox;

using System.Collections.Generic;

public abstract class Expr
{
    public interface IVisitor<R>
    {
        R VisitBinaryExpr(Binary expr);
        R VisitGroupingExpr(Grouping expr);
        R VisitLiteralExpr(Literal expr);
        R VisitLogicalExpr(Logical expr);
        R VisitVariableExpr(Variable expr);
        R VisitAssignExpr(Assign expr);
        R VisitUnaryExpr(Unary expr);
    }

    public class Binary : Expr
    {
        public Expr Left { get; set; }
        public Token Operator { get; set; }
        public Expr Right { get; set; }


        public override R Accept<R>(IVisitor<R> visitor)
        {
            return visitor.VisitBinaryExpr(this);
        }
    }

    public class Grouping : Expr
    {
        public Expr Expression { get; set; }


        public override R Accept<R>(IVisitor<R> visitor)
        {
            return visitor.VisitGroupingExpr(this);
        }
    }

    public class Literal : Expr
    {
        public Object Value { get; set; }


        public override R Accept<R>(IVisitor<R> visitor)
        {
            return visitor.VisitLiteralExpr(this);
        }
    }

    public class Logical : Expr
    {
        public Expr Left { get; set; }
        public Token Op { get; set; }
        public Expr Right { get; set; }


        public override R Accept<R>(IVisitor<R> visitor)
        {
            return visitor.VisitLogicalExpr(this);
        }
    }

    public class Variable : Expr
    {
        public Token Name { get; set; }


        public override R Accept<R>(IVisitor<R> visitor)
        {
            return visitor.VisitVariableExpr(this);
        }
    }

    public class Assign : Expr
    {
        public Token Name { get; set; }
        public Expr Value { get; set; }


        public override R Accept<R>(IVisitor<R> visitor)
        {
            return visitor.VisitAssignExpr(this);
        }
    }

    public class Unary : Expr
    {
        public Token Operator { get; set; }
        public Expr Right { get; set; }


        public override R Accept<R>(IVisitor<R> visitor)
        {
            return visitor.VisitUnaryExpr(this);
        }
    }


    public abstract R Accept<R>(IVisitor<R> visitor);
}