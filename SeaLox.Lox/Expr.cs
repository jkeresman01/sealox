namespace SeaLox.Lox;

using System.Collections.Generic;

abstract class Expr
{
    public class Binary : Expr
    {
        public Expr Left { get; set; }
        public Token Operator { get; set; }
        public Expr Right { get; set; }
    }

    public class Grouping : Expr
    {
        public Expr Expression { get; set; }
    }

    public class Literal : Expr
    {
        public Object Value { get; set; }
    }

    public class Unary : Expr
    {
        public Token Operator { get; set; }
        public Expr Right { get; set; }
    }
}