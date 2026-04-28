namespace SeaLox.Lox;

public class Parser(IList<Token> tokens)
{
    private class ParseError : Exception;
    
    private int _current;


    public Expr Parse()
    {
        try
        {
            return Expression();
        }
        catch (ParseError error)
        {
            return null;
        }
    }

    private Expr Expression() => Equality();

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
            return new Expr.Literal { Value = false };
        }

        if (Match(TokenType.True))

        {
            return new Expr.Literal { Value = true };
        }

        if (Match(TokenType.Nil))
        {
            return new Expr.Literal { Value = null };
        }

        if (Match(TokenType.Number, TokenType.String))
        {
            return new Expr.Literal { Value = Previous().Literal };
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