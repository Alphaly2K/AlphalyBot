using System.Diagnostics;
using System.Reflection;
using AlphalyBot.Model;
using AlphalyBot.Service;
using Makabaka.Models.EventArgs;

namespace AlphalyBot.Controller;

internal class GroupMessageController
{
    public GroupMessageController(GroupMessageEventArgs groupMessage)
    {
        _groupMessage = groupMessage;
        if (_gmDict.TryGetValue(_groupMessage.Message, out var value)) _action = value;
        var messageSplit = groupMessage.Message.ToString().Split(" ");
        if (messageSplit.Length > 0 && _gmCommandDict.ContainsKey(messageSplit[0]))
            _commandAction = _gmCommandDict[messageSplit[0]];
    }

    public static void AddShortcutDelegate(string shortcut, GmShortcutDelegate action)
    {
        GmShortcutsDict.Add(shortcut, action);
    }
    
    public static void AddCommandDelegate(string commandNamespace, GmCommandDelegate action)
    {
        GmCommandsDict.Add(commandNamespace, action);
    }

    public async Task Exec()
    {
        if (_action != null)
        {
            var serviceAttribute = _action.GetMethodInfo().GetCustomAttributes(false).OfType<ServiceAttribute>()
                .FirstOrDefault();
            if (serviceAttribute == null)
            {
                await _action.Invoke(_groupMessage);
                return;
            }

            if (serviceAttribute.Auth && !Program.Admins.Contains(_groupMessage.Sender.UserId))
                return;
            ServiceManager service = new(_groupMessage.GroupId);
            await service.Init();
            Debug.Assert(serviceAttribute != null, nameof(serviceAttribute) + " != null");
            if (service.IsServiceEnabled(serviceAttribute.Service))
                await _action.Invoke(_groupMessage);
        }

        if (_commandAction != null)
        {
            var serviceAttribute = _commandAction.GetMethodInfo().GetCustomAttributes(false).OfType<ServiceAttribute>()
                .FirstOrDefault();
            if (serviceAttribute == null)
            {
                await _commandAction.Invoke(_groupMessage);
                return;
            }

            if (serviceAttribute.Auth && !Program.Admins.Contains(_groupMessage.Sender.UserId))
                return;
            ServiceManager service = new(_groupMessage.GroupId);
            await service.Init();
            if (service.IsServiceEnabled(serviceAttribute.Service))
                await _commandAction.Invoke(_groupMessage);
        }
    }

    #region General

    public delegate Task GmShortcutDelegate(GroupMessageEventArgs groupMessage);

    private readonly GroupMessageEventArgs _groupMessage;
    private readonly GmShortcutDelegate? _action;
    private static readonly Dictionary<string, GmShortcutDelegate> GmShortcutsDict = new();

    #endregion

    #region Command

    public delegate Task GmCommandDelegate(GroupMessageEventArgs groupMessage);

    private readonly GmCommandDelegate? _commandAction;
    private static readonly Dictionary<string, GmCommandDelegate> GmCommandsDict = new();

    #endregion
}