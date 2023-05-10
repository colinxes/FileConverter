namespace FileConverter.Foundation;

public static class TypeExtensions
{
    public static bool IsFileConverter(this Type pType) 
        => pType is { IsAbstract: false, IsClass: true } && typeof(IFileConverter).IsAssignableFrom(pType);
    
}