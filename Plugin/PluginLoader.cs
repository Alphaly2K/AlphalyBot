using System.Reflection;
using AlphalyBot.Controller;
using AlphalyBot.Model;

namespace AlphalyBot.Plugin;

public class PluginLoader
{
    private static List<Type> LoadPlugins(string folderPath)
    {
        return (from file in Directory.GetFiles(folderPath, "*.dll") select Assembly.LoadFrom(file) into assembly from type in assembly.GetTypes() where typeof(IPlugin).IsAssignableFrom(type) && !type.IsInterface select type).ToList();
    }
    public List<Type> Plugins = LoadPlugins("plugins");

    public PluginLoader()
    {
        foreach (var plugin in Plugins)
        {
            var methods = plugin.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic);
            var attrs= plugin.GetCustomAttributes<ServiceAttribute>();
            foreach (var attr in attrs)
            {
                GroupMessageController.AddCommandDelegate();
            }
        }
    }
}