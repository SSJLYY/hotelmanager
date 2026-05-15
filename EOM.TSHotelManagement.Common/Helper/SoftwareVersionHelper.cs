using System.Reflection;

namespace EOM.TSHotelManagement.Common
{
    public static class SoftwareVersionHelper
    {
        public static string GetSoftwareVersion(string? configuredVersion = null, string defaultVersion = "1.0.0")
        {
            var version = Environment.GetEnvironmentVariable("APP_VERSION")
                ?? Environment.GetEnvironmentVariable("SoftwareVersion")
                ?? configuredVersion
                ?? GetVersionFromFile()
                ?? GetAssemblyVersion();

            return string.IsNullOrWhiteSpace(version) ? defaultVersion : version.Trim();
        }

        private static string? GetVersionFromFile()
        {
            try
            {
                var assembly = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();
                var rootPath = Path.GetDirectoryName(assembly.Location);
                if (string.IsNullOrWhiteSpace(rootPath))
                {
                    return null;
                }

                var versionFilePath = Path.Combine(rootPath, "version.txt");
                if (!File.Exists(versionFilePath))
                {
                    return null;
                }

                var versionContent = File.ReadAllText(versionFilePath).Trim();
                return string.IsNullOrWhiteSpace(versionContent) ? null : versionContent;
            }
            catch
            {
                return null;
            }
        }

        private static string? GetAssemblyVersion()
        {
            var assembly = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();
            return assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion
                ?? assembly.GetName().Version?.ToString(3);
        }
    }
}
