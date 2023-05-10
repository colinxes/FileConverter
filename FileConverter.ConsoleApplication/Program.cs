using System.Diagnostics;
using FileConverter.Foundation;
using FileConverter.ImageConverter;

namespace ImageConverter;

public static class Program
{
    static void Main()
    {
        var provider = new FileConverterProvider();
        provider.AddImageConverters();
        Console.Write(Language.Program_Main_Enter_Source_Path);
        var inputFilePath = Console.ReadLine();
        Console.Write(Language.Program_Main_Enter_Destination_Path);
        var targetFilePath = Console.ReadLine();
        Console.Write(Language.Program_Main_Enter_Source_File_Format);
        var sourceFileEnding = Console.ReadLine()?.ToLower();
        Console.Write(Language.Program_Main_Enter_Destination_File_Path);
        var targetFileEnding = Console.ReadLine()?.ToLower();
        Console.WriteLine();
        
        ValidateInputFilePath(inputFilePath);
        ValidateTagetFilePath(targetFilePath);
        ValidateSourceFileEnding(sourceFileEnding);
        ValidateTargetFileEnding(targetFileEnding);

        const string errorFolderName = "ERROR";
        var errorPath = Path.Combine(inputFilePath!, errorFolderName);

        try
        {
            int counter = default;
            
            IEnumerable<string> sourceFiles = Directory
                .GetFiles(inputFilePath!, $"*.{sourceFileEnding}")
                .ToList();
            
            Console.WriteLine(
                Language.Program_Main_Found_X_Files_For_X_File_Ending, 
                sourceFiles.Count(), 
                sourceFileEnding);

            if (!sourceFiles.Any())
            {
                Console.WriteLine(Language.Program_Main_No_Files_Found);
                Shutdown();
            }
            
            var converter = provider.ByFileEndings(sourceFileEnding!, targetFileEnding!);
            var stopwatch = Stopwatch.StartNew();
            
            Parallel.ForEach(sourceFiles, (sourceFile) =>
            {
                var isSuccessful = HandleFile(
                    sourceFile, 
                    errorPath, 
                    targetFilePath!,
                    targetFileEnding!,
                    converter);
                
                if(isSuccessful)
                    Interlocked.Increment(ref counter);
            });
            
            stopwatch.Stop();
            Console.WriteLine();
            Console.WriteLine(Language.Program_Main_Converted_X_Files, sourceFiles.Count());
            Console.WriteLine(Language.Program_Main_Converted_X_Files_Successfully, counter);
            Console.WriteLine(Language.Program_Main_The_Conversion_Took_X_Milliseconds, stopwatch.ElapsedMilliseconds);
        }
        catch (Exception exception)
        {
            Console.WriteLine(Language.Program_Main_Error_While_Reading_File_X, exception.Message);
        }

        Shutdown();
    }

    private static bool HandleFile(
        string pSourceFile,
        string pErrorPath,
        string pOutputFile,
        string pFileEnding,
        IFileConverter pConverter)
    {
        var fileName = Path.GetFileName(pSourceFile);
        
        try
        {
            Console.WriteLine(Language.Program_HandleFile_Trying_To_Convert_File_X, fileName);
            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(pSourceFile);
            var outputFilePathWithEnding = Path.Combine(pOutputFile, $"{fileNameWithoutExtension}.{pFileEnding}");
            pConverter.Convert(pSourceFile, outputFilePathWithEnding);
            Console.WriteLine(Language.Program_HandleFile_Conversion_For_File_X_Was_Successful, fileName);

            return true;
        }
        catch (Exception exception)
        {
            Console.WriteLine(Language.Program_HandleFile_Error_While_Converting_File_X, exception.Message);

            if (!Directory.Exists(pErrorPath))
                Directory.CreateDirectory(pErrorPath);
                    
            var errorFilePath = Path.Combine(pErrorPath, fileName);
            File.Move(pSourceFile, errorFilePath, true);

            return false;
        }
    }
    
    private static void ValidateTargetFileEnding(string? pTargetFileEnding)
    {
        if (!string.IsNullOrWhiteSpace(pTargetFileEnding)) return;
        Console.WriteLine(Language.Program_ValidateTargetFileEnding_Invalid);
        Console.ReadKey();
        Shutdown();
    }
    
    private static void ValidateSourceFileEnding(string? sourceFileEnding)
    {
        if (!string.IsNullOrWhiteSpace(sourceFileEnding)) return;
        Console.WriteLine(Language.Program_ValidateSourceFileEnding_Invalid);
        Console.ReadKey();
        Shutdown();
    }

    private static void ValidateTagetFilePath(string? targetFilePath)
    {
        if (!string.IsNullOrWhiteSpace(targetFilePath) && Directory.Exists(targetFilePath)) return;
        
        Console.WriteLine(Language.Program_ValidateTagetFilePath_Invalid);
        Console.ReadKey();
        Shutdown();
    }

    private static void ValidateInputFilePath(string? inputFilePath)
    {
        if (!string.IsNullOrWhiteSpace(inputFilePath) && Directory.Exists(inputFilePath))
            return;
        
        Console.WriteLine(Language.Program_ValidateInputFilePath_Invalid);
        Console.ReadKey();
        Shutdown();
    }
    
    private static void Shutdown()
    {
        Console.WriteLine();
        Console.WriteLine(Language.Program_Shutdown_Press_Any_Key_To_Exit); 
        Console.ReadKey();
        Environment.Exit(default);
    }
}