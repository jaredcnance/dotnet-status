using System.Collections.Generic;
using System.Xml.Serialization;

namespace DotnetHealth.Services.Http
{
    [XmlRoot("Project")]
    public class Csproj
    {
        [XmlElement("ItemGroup")]
        public List<ItemGroup> ItemGroups { get; set; } = new List<ItemGroup>();
    }

    public class ItemGroup
    {
        [XmlElement("PackageReference")]
        public List<PackageReference> PackageReferences { get; set; } = new List<PackageReference>();
    }

    public class PackageReference
    {
        [XmlAttribute]
        public string Include {get;set;}

        [XmlAttribute]
        public string Version {get;set;}
    }
}
