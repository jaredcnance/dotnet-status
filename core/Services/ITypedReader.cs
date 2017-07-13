namespace Core.Services
{
    public interface ITypedReader
    {
        T ReadAt<T>(string location);
    }
}