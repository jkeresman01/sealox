namespace SeaLox.Tool;

public class AstGenerator
{
    private const string Stmt = "Stmt";
    private const string Expr = "Expr";
    
    static void Main(string[] args)
    {
        if (args.Length != 1)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Usage: SeaLox.Tool.exe <path>");
            Environment.Exit(64);
        }

        var outputDir = args[0];

        IDictionary<string, string> expressions = new Dictionary<string, string>
        {
            { "Binary", "Expr Left, Token Operator, Expr Right" },
            { "Grouping", "Expr Expression" },
            { "Literal", "Object Value" },
            { "Logical", "Expr Left, Token Op, Expr Right" },
            { "Variable", "Token Name" },
            { "Assign", "Token Name, Expr Value" },
            { "Unary", "Token Operator, Expr Right" }
        };

        DefineAst(outputDir,Expr, expressions);

        IDictionary<string, string> statements = new Dictionary<string, string>
        {
            { "Expression", "Expr Expr" },
            { "If", "Expr Condition, Stmt ThenBranch, Stmt ElseBranch" },
            { "Print", "Expr Expression" },
            { "Block", "List<Stmt> Statements" },
            { "Var", "Token Name, Expr Initalizer" },
            { "While", "Expr Condition, Stmt Body" }
        };
        
        DefineAst(outputDir, Stmt, statements);
    }

    private static void DefineAst(
        string outputDir,
        string baseName,
        IDictionary<string, string> types)
    {
        var path = outputDir + "/" + baseName + ".cs";

        Console.WriteLine("Generating " + path);

        using StreamWriter writer = new(path, false);

        DefineAbstractNamespaces(writer);
        DefineBaseClass(writer, baseName);
        DefineVisitorInterface(writer, types, baseName);
        DefineTypes(baseName, writer, types);
        DefineClosing(writer);
    }

    private static void DefineVisitorInterface(
        StreamWriter writer,
        IDictionary<string, string> types,
        string baseName)
    {
        writer.WriteLine();
        writer.WriteLine("        public interface IVisitor<R>");
        writer.WriteLine("        {");

        foreach (var type in types.Keys)
        {
            writer.WriteLine($"              R Visit{type}{baseName}({type} {baseName.ToLower()});");
        }

        writer.WriteLine("        }");
    }

    private static void DefineAbstractNamespaces(StreamWriter writer)
    {
        writer.WriteLine("namespace SeaLox.Lox;");
        writer.WriteLine();
        writer.WriteLine("using System.Collections.Generic;");
        writer.WriteLine();
    }

    private static void DefineBaseClass(StreamWriter writer, string baseName)
    {
        writer.WriteLine($"public abstract class {baseName}");
        writer.WriteLine("{");
        writer.WriteLine();
    }

    private static void DefineTypes(
        string basename,
        StreamWriter writer,
        IDictionary<string, string> types)
    {
        foreach (var type in types.Keys)
        {
            DefineType(basename, writer, type, types[type]);
        }
    }

    private static void DefineType(
        string basename,
        StreamWriter writer,
        string className,
        string props)
    {
        writer.WriteLine($"public class {className} : {basename}");
        writer.WriteLine("{");

        var properties = props.Split(", ");

        foreach (var property in properties)
        {
            writer.WriteLine("     public " + property + " { get; set; }");
        }

        writer.WriteLine();
        writer.WriteLine();

        writer.WriteLine("      public override R Accept<R>(IVisitor<R> visitor)");
        writer.WriteLine("      {");
        writer.WriteLine($"          return visitor.Visit{className}{basename}(this);");
        writer.WriteLine("      }");
        writer.WriteLine();

        writer.WriteLine("}");
        writer.WriteLine();
    }

    private static void DefineClosing(StreamWriter writer)
    {
        writer.WriteLine();
        writer.WriteLine();
        writer.WriteLine("        public abstract R Accept<R>(IVisitor<R> visitor);");
        writer.WriteLine("}");
    }
}