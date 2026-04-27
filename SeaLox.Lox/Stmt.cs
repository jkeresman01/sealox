namespace SeaLox.Lox;

using System.Collections.Generic;

abstract class Stmt
{
    public class Print : Stmt
    {
        public Expr expression { get; set; }
    }

    public class Var : Stmt
    {
        public Token name { get; set; }
        public Expr initalizer { get; set; }
    }
}