using System.Diagnostics;

class SeaLox
{
    static bool hadError = false;
    
    static void Main(string[] args)
    {
        switch (args.Length)
        {
            case > 1:
                Console.WriteLine("Usage: SeaLox [script]: ");
                Environment.Exit(64);
                break;
            case 1:
                RunFile(args[0]);
                break;
            default:
                RunPrompt();
                break;
        }
    }

    private static void RunPrompt()
    {
        for (;;)
        {
            Console.Write("> ");
            var line = Console.ReadLine();

            if (line == null)
            {
                break;
            }
            
            Run(line);
            
            hadError = false;
        }
    }

    private static void RunFile(string path)
    {
        if (!File.Exists(path))
        {
            Console.WriteLine($"No such file: {path}");
        }

        var source = File.ReadAllText(path);

        if (hadError)
        {
            Environment.Exit(65);
        }
        
        Run(source);
    }

    private static void Run(string source)
    {
        var scanner = new Scanner(source);
        var tokens = scanner.ScanTokens();
        tokens.ForEach(Console.WriteLine);
    }
    
    public static void Error(int line, String message) => Report(line, "", message);

    private static void Report(int line, string where, string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"[{line}] Error {where}: {message}");
    }
}
