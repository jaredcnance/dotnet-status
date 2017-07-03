using System.Collections.Generic;

namespace DotnetHealth.Services.Http
{
    public class Nuget
    {
        public string Name { get; set; }
        public List<string> Versions { get; set; } = new List<string>();
    }
}
