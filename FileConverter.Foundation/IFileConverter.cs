namespace FileConverter.Foundation;

public interface IFileConverter
{
    public string SourceFileEnding { get; }
    public string TargetFileEnding { get; }
    

    public void Convert(string pInputFile, string pExportFile);
}