using System.Reflection;
using AlphalyBot.Model;
using AlphalyBot.Service;
using Makabaka.Models.EventArgs;
using Serilog;
using BindingFlags = System.Reflection.BindingFlags;

namespace AlphalyBot.Tool;

public class CommandHandler
{
    public CommandHandler(GroupMessageEventArgs message, Type type)
    {
        Type = type;
        GroupMessage = message;
    }

    private GroupMessageEventArgs GroupMessage { get; }
    private Type Type { get; }

    public async Task ExecAsync()
    {
        ServiceManager service = new(GroupMessage.GroupId);
        await service.Init();
        var command = GroupMessage.Message.ToString().Split(" ");
        var constructor = Type.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, Type.DefaultBinder,
            new[] { typeof(GroupMessageEventArgs) }, new ParameterModifier[] { new(1) });
        if (constructor is null)
            return;
        var instance = constructor.Invoke(new object[] { GroupMessage });
        var methods = Type.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic);
        foreach (var method in methods)
        {
            var attr = method.GetCustomAttributes(true).OfType<ServiceAttribute>().FirstOrDefault();
            if (attr is null) continue;
            if (attr.Prompt != command[1].ToLower() ||
                !service.IsServiceEnabled(attr.Service) ||
                (attr.Auth && !Program.Admins.Contains(GroupMessage.Sender.UserId))
               )
                continue;
            try
            {
                var arg = command.Length == 3 ? command[2].ToLower() : string.Empty;
                method.Invoke(instance, new object[] { arg });
            }
            catch (Exception e)
            {
                Log.Warning("CommandHandler: Bad command from {0}", GroupMessage.UserId);
#if DEBUG
                Log.Warning("CommandHandler: {0}", e.Message);
#endif
            }
        }
    }
}