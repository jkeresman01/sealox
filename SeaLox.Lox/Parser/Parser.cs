using System.Linq.Expressions;

namespace SeaLox.Lox;

public class Parser(IList<Token> tokens)
{
    private class ParseError : Exception;

    private int _current;


    public IEnumerable<Stmt> Parse()
    {
        var statements = new List<Stmt>();

        while (!IsAtEnd())
        {
            statements.Add(Declaration());
        }

        return statements;
    }

    private Stmt Declaration()
    {
        try
        {
            return Match(TokenType.Var) ? VarDeclaration() : Statement();
        }
        catch (ParseError parseError)
        {
            Synchronize();
            return null;
        }
    }

    private void Synchronize()
    {
        Advance();

        while (!IsAtEnd())
        {
            if (Previous().TokenType == TokenType.Semicolon)
            {
                return;
            }

            switch (Peek().TokenType)
            {
                case TokenType.Class:
                case TokenType.For:
                case TokenType.Fun:
                case TokenType.If:
                case TokenType.Print:
                case TokenType.Return:
                case TokenType.Var:
                case TokenType.While:
                    return;
            }

            Advance();
        }
    }

    private Stmt VarDeclaration()
    {
        var name = Consume(TokenType.Identifier, "Expected variable name");

        Expr initalizer = null;

        while (Match(TokenType.Equal))
        {
            initalizer = Expression();
        }

        Consume(TokenType.Semicolon, "Expected ';' after variable declaration");

        return new Stmt.Var
        {
            Name = name,
            Initalizer = initalizer!
        };
    }

    private Stmt Statement()
    {
        if (Match(TokenType.If))
        {
            return IfStatement();
        }
        
        if (Match(TokenType.While))
        {
            return WhileStatement();
        }
        
        if (Match(TokenType.Print))
        {
            return PrintStatement();
        }

        if (Match(TokenType.LeftBrace))
        {
            return new Stmt.Block
            {
                Statements = Block()
            };
        }

        return ExpressionStatement();
    }

    private Stmt WhileStatement()
    {
        Consume(TokenType.LeftParen, "Expect '(' after while statement");
        var condition = Expression();
        Consume(TokenType.RightParen, "Expect ')' after while statement");
        var body = Statement();

        return new Stmt.While
        {
            Condition = condition,
            Body = body
        };
    }

    private Stmt IfStatement()
    {
        Consume(TokenType.LeftParen, "Expect '('  after if statement");
        var condition = Expression();
        Consume(TokenType.RightParen, "Expect ')' after if statement");

        var thenBranch = Statement();
        Stmt elseBranch = null;

        if (Match(TokenType.Else))
        {
            elseBranch = Statement();
        }

        return new Stmt.If
        {
            Condition = condition,
            ElseBranch = elseBranch!,
            ThenBranch = thenBranch
        };
    }

    private List<Stmt> Block()
    {
        var stmts = new List<Stmt>();

        while (!Check(TokenType.RightBrace) && !IsAtEnd())
        {
            stmts.Add(Declaration());
        }

        Consume(TokenType.RightBrace, "Expect '}' after block.");
        return stmts;
    }

    private Stmt ExpressionStatement()
    {
        var value = Expression();
        Consume(TokenType.Semicolon, "Expected ';' after expression");
        return new Stmt.Expression
        {
            Expr = value
        };
    }

    private Stmt PrintStatement()
    {
        var value = Expression();
        Consume(TokenType.Semicolon, "Expected ';' after expression");
        return new Stmt.Print { Expression = value };
    }

    private Expr Expression() => Assignment();

    private Expr Assignment()
    {
        var expr = Or();

        if (Match(TokenType.Equal))
        {
            var equals = Previous();
            var value = Assignment();

            if (expr is Expr.Variable variable)
            {
                return new Expr.Assign
                {
                    Name = variable.Name,
                    Value = value
                };
            }

            Error(equals, "Invalid assignment target");
        }

        return expr;
    }

    private Expr Or()
    {
        var expr = And();

        if (Match(TokenType.Or))
        {
            var op = Previous();
            var right = And();
            expr = new Expr.Logical
            {
                Left = expr,
                Op = op,
                Right = right
            };
        }

        return expr;
    }

    private Expr And()
    {
        var expr = Equality();

        while (Match(TokenType.And))
        {
            var op = Previous();
            var right = Equality();
            expr = new Expr.Logical
            {
                Left = expr,
                Op = op,
                Right = right
            };
        }

        return expr;
    }

    private Expr Equality()
    {
        var expr = Comparison();
        while (Match(TokenType.BangEqual, TokenType.EqualEqual))
        {
            var op = Previous();
            var right = Comparison();
            expr = new Expr.Binary
            {
                Left = expr,
                Operator = op,
                Right = right
            };
        }

        return expr;
    }

    private bool Match(params TokenType[] types)
    {
        if (!types.Any(Check)) return false;
        Advance();
        return true;
    }

    private Token Advance()
    {
        if (!IsAtEnd())
        {
            _current++;
        }

        return Previous();
    }

    private bool IsAtEnd() => Peek().TokenType == TokenType.Eof;

    private bool Check(TokenType type)
    {
        if (IsAtEnd()) return false;
        return Peek().TokenType == type;
    }

    private Token Peek() => tokens[_current];

    private Token Previous() => tokens[_current - 1];

    private Expr Comparison()
    {
        var expr = Term();
        while (Match(TokenType.Greater, TokenType.GreaterEqual,
                   TokenType.Less, TokenType.LessEqual))
        {
            var operatorToken = Previous();
            var right = Term();
            expr = new Expr.Binary
            {
                Left = expr,
                Operator = operatorToken,
                Right = right
            };
        }

        return expr;
    }

    private Expr Term()
    {
        var expr = Factor();

        while (Match(TokenType.Minus, TokenType.Plus))
        {
            var operatorToken = Previous();
            var right = Factor();
            expr = new Expr.Binary
            {
                Left = expr,
                Operator = operatorToken,
                Right = right
            };
        }

        return expr;
    }

    private Expr Factor()
    {
        var expr = Unary();

        while (Match(TokenType.Star, TokenType.Slash))
        {
            var opToken = Previous();
            var right = Unary();

            expr = new Expr.Binary
            {
                Left = expr,
                Operator = opToken,
                Right = right
            };
        }

        return expr;
    }

    private Expr Unary()
    {
        if (Match(TokenType.Bang, TokenType.Minus))
        {
            var opToken = Previous();
            var right = Unary();

            return new Expr.Unary
            {
                Operator = opToken,
                Right = right
            };
        }

        return Primary();
    }

    private Expr Primary()
    {
        if (Match(TokenType.False))
        {
            return new Expr.Literal
            {
                Value = false
            };
        }

        if (Match(TokenType.True))

        {
            return new Expr.Literal
            {
                Value = true
            };
        }

        if (Match(TokenType.Nil))
        {
            return new Expr.Literal
            {
                Value = null
            };
        }

        if (Match(TokenType.Number, TokenType.String))
        {
            return new Expr.Literal
            {
                Value = Previous().Literal
            };
        }

        if (Match(TokenType.Identifier))
        {
            return new Expr.Variable
            {
                Name = Previous()
            };
        }

        if (Match(TokenType.LeftParen))
        {
            var expr = Expression();
            Consume(TokenType.RightParen, "Expected ')' after expression");
            return new Expr.Grouping { Expression = expr };
        }

        throw Error(Peek(), "Expected expression");
    }

    private Token Consume(TokenType type, string message)
    {
        if (Check(type))
        {
            return Advance();
        }

        throw Error(Peek(), message);
    }

    private ParseError Error(Token token, string message)
    {
        SeaLox.Error(token, message);
        throw new ParseError();
    }
}