using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Worker.Helpers
{
    /// <summary>
    /// Azure Fcns do not currently support binding redirect
    /// https://github.com/Azure/azure-webjobs-sdk-script/issues/992
    /// </summary>
    internal static class BindingRedirect
    {
        public static void AddRedirects()
        {
            RedirectAssembly("System.Runtime.InteropServices.RuntimeInformation", new Version("4.0.1.0"), "b03f5f7f11d50a3a");
        }

        public static void RedirectAssembly(
            string shortName,
            Version targetVersion,
            string publicKeyToken)
        {
            ResolveEventHandler handler = null;

            handler = (sender, args) =>
            {
                var requestedAssembly = new AssemblyName(args.Name);

                if (requestedAssembly.Name != shortName)
                {
                    return null;
                }

                var targetPublicKeyToken = new AssemblyName("x, PublicKeyToken=" + publicKeyToken)
                    .GetPublicKeyToken();
                requestedAssembly.Version = targetVersion;
                requestedAssembly.SetPublicKeyToken(targetPublicKeyToken);
                requestedAssembly.CultureInfo = CultureInfo.InvariantCulture;

                AppDomain.CurrentDomain.AssemblyResolve -= handler;

                return Assembly.Load(requestedAssembly);
            };

            AppDomain.CurrentDomain.AssemblyResolve += handler;
        }
    }
}
