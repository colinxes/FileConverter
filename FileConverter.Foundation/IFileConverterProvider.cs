using System.Reflection;

namespace FileConverter.Foundation;

public interface IFileConverterProvider
{
    IFileConverter ByFileEndings(string pSourceFileEnding, string pTargetFileEnding);
    void AddFromAssembly(Assembly pAssembly);
}