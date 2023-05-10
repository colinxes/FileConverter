using System.Diagnostics;
using FileConverter.Foundation;
using FileConverter.ImageConverter;
using ImageMagick;

namespace ImageConverter;

public static class Program
{
    static void Main(string[] args)
    {
        Console.Write("Bitte geben Sie den Quellpfad an: ");
        string? inputFilePath = Console.ReadLine();
        Console.Write("Bitte geben Sie den Zielpfad an: ");
        string? targetFilePath = Console.ReadLine();
        Console.Write("Bitte geben Sie das Quelldateiformat an: ");
        string? sourceFileEnding = Console.ReadLine()?.ToLower();
        Console.Write("Bitte geben Sie das Zieldateiformat an: ");
        string? targetFileEnding = Console.ReadLine()?.ToLower();
        Console.WriteLine();
        ValidateInputFilePath(inputFilePath);
        ValidateTagetFilePath(targetFilePath);
        ValidateSourceFileEnding(sourceFileEnding);
        ValidateTargetFileEnding(targetFileEnding);
        string errorPath = Path.Combine(inputFilePath!, "ERROR");

        try
        {
            int counter = default;
            IEnumerable<string> sourceFiles = Directory.GetFiles(inputFilePath!, $"*.{sourceFileEnding}").ToList();
            Console.WriteLine($"Es wurden {sourceFiles.Count()} Dateien für die Dateiendung \"{sourceFileEnding}\" gefunden.");

            if (!sourceFiles.Any())
            {
                Console.WriteLine("Keine Dateien gefunden!");
                Shutdown();
            }
            
            FileConverterProvider provider = new FileConverterProvider();
            IFileConverter converter = provider.ByFileEndings(sourceFileEnding!, targetFileEnding!);
            Stopwatch stopwatch = Stopwatch.StartNew();
            
            Parallel.ForEach(sourceFiles, (sourceFile) =>
            {
                HandleFile(sourceFile, errorPath, targetFilePath!, targetFileEnding!, converter);
                Interlocked.Increment(ref counter);
            });
            
            stopwatch.Stop();
            Console.WriteLine();
            Console.WriteLine($"Es wurden {sourceFiles.Count()} Dateien abgearbeitet.");
            Console.WriteLine($"Es wurden {counter} erfolgreich konvertiert.");
            Console.WriteLine($"Die Konvertierung dauerte {stopwatch.ElapsedMilliseconds}ms.");
        }
        catch (Exception exception)
        {
            Console.WriteLine($"Es ist ein Fehler beim Lesen der Dateien aufgetreten: {exception.Message}");
        }

        Shutdown();
    }

    private static void HandleFile(
        string sourceFile,
        string pErrorPath,
        string pOutputFile,
        string pFileEnding,
        IFileConverter pConverter)
    {
        string fileName = Path.GetFileName(sourceFile);
                
        try
        {
            Console.WriteLine($"Versuche folgende Datei zu konvertieren: {fileName}");
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(sourceFile);
            string outputFilePathWithEnding = Path.Combine(pOutputFile, $"{fileNameWithoutExtension}.{pFileEnding}");
            pConverter.Convert(sourceFile, outputFilePathWithEnding);
            Console.WriteLine($"Konvertierung der Datei war erfolgeich: {fileName}");
            
        }
        catch (Exception exception)
        {
            Console.WriteLine($"Es ist ein Fehler beim Konvertieren aufgetreten: {exception.Message}");

            if (!Directory.Exists(pErrorPath))
                Directory.CreateDirectory(pErrorPath);
                    
            string errorFilePath = Path.Combine(pErrorPath, fileName);
            File.Move(sourceFile, errorFilePath, true);
        }
    }
    
    private static void ValidateTargetFileEnding(string? pTargetFileEnding)
    {
        if (!string.IsNullOrWhiteSpace(pTargetFileEnding)) return;
        Console.WriteLine("Das Zieldateiformat ist ungültig.");
        Console.ReadKey();
        Shutdown();
    }
    
    private static void ValidateSourceFileEnding(string? sourceFileEnding)
    {
        if (!string.IsNullOrWhiteSpace(sourceFileEnding)) return;
        Console.WriteLine("Das Quelldateiformat ist ungültig.");
        Console.ReadKey();
        Shutdown();
    }

    private static void ValidateTagetFilePath(string? targetFilePath)
    {
        if (!string.IsNullOrWhiteSpace(targetFilePath) && Directory.Exists(targetFilePath)) return;
        
        Console.WriteLine("Der Zielpfad ist ungültig.");
        Console.ReadKey();
        Shutdown();
    }

    private static void ValidateInputFilePath(string? inputFilePath)
    {
        if (!string.IsNullOrWhiteSpace(inputFilePath) && Directory.Exists(inputFilePath))
            return;
        
        Console.WriteLine("Der Quellpfad ist ungültig.");
        Console.ReadKey();
        Shutdown();
    }
    
    private static void Shutdown()
    {
        Console.WriteLine();
        Console.WriteLine("Drücken Sie eine Taste um die Anwendung zu beenden..."); 
        Console.ReadKey();
        const int defaultShutdownCode = 0;
        Environment.Exit(defaultShutdownCode);
    }
}