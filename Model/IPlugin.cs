using Makabaka.Models.EventArgs;

namespace AlphalyBot.Model;

public interface IPlugin
{
    public string Name { get; }
    public string Description { get; }
    public string Author { get; }
    public HashSet<string> ServiceList { get; }
    public string Version { get; }
    public Task Execute(GroupMessageEventArgs groupMessage);
    
}