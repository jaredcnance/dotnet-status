using System;
using System.IO;
using System.Reflection;

namespace CoreTests
{
    // REF: https://github.com/NuGet/NuGet.Client/blob/9c1502ee06d60ff9d8b0c9f5477b95ffed45b8cc/test/TestUtilities/Test.Utility/ResourceTestUtility.cs
    public static class ResourceTestUtility
    {
        public static string GetResource(string name, Type type)
        {
            using (var reader = new StreamReader(type.GetTypeInfo().Assembly.GetManifestResourceStream(name)))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
