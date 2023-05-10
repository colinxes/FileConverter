using System.Reflection;
using FileConverter.Foundation;

namespace ImageConverter;

public class FileConverterProvider : IFileConverterProvider
{
    private readonly HashSet<IFileConverter> _converters = new();
    
    public IFileConverter ByFileEndings(string pSourceFileEnding, string pTargetFileEnding)
    {
        var fileConverter = _converters.SingleOrDefault(pConverter => string.Equals(pConverter.TargetFileEnding, pTargetFileEnding, StringComparison.CurrentCultureIgnoreCase) 
                                                                      && string.Equals(pConverter.SourceFileEnding, pSourceFileEnding, StringComparison.CurrentCultureIgnoreCase));

        return fileConverter ?? throw new NotSupportedException($"No converter found for file ending: {pTargetFileEnding}");
    }

    public void AddFromAssembly(Assembly pAssembly)
    {
        IEnumerable<Type> fileConverterTypes = pAssembly
            .GetTypes()
            .Where(pType => pType.IsFileConverter());

        IEnumerable<IFileConverter> fileConverterInstances = fileConverterTypes
            .Select(pType => (IFileConverter)Activator.CreateInstance(pType)!);

        foreach (var instance in fileConverterInstances) 
            _converters.Add(instance);
    }
}