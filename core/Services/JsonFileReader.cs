using Newtonsoft.Json;
using System.IO;

namespace Core.Services
{
    public class JsonFileReader : ITypedReader
    {
        public T ReadAt<T>(string location)
        {
            var json = File.ReadAllText(location);
            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}
