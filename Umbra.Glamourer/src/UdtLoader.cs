using System.IO;
using System.Reflection;
using Una.Drawing;

namespace Umbra.Plugin.Glamourer;

internal static class UdtLoader
{
    public static UdtDocument Load(string resourceName)
    {
        Assembly assembly = Assembly.GetExecutingAssembly();
        
        foreach (var name in assembly.GetManifestResourceNames())
        {
            if (name.ToLowerInvariant().EndsWith(resourceName))
            {
                return Una.Drawing.UdtLoader.LoadFromAssembly(assembly, name);
            }
        }
        
        throw new FileNotFoundException($"Resource {resourceName} not found in assembly {assembly.FullName}");
    }
}
