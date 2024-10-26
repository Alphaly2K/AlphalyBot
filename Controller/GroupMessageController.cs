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
        AddDelegate();
        if (_gmDict.TryGetValue(_groupMessage.Message, out var value)) _action = value;
        var messageSplit = groupMessage.Message.ToString().Split(" ");
        if (messageSplit.Length > 0 && _gmCommandDict.ContainsKey(messageSplit[0]))
            _commandAction = _gmCommandDict[messageSplit[0]];
    }

    private void AddDelegate()
    {
        _gmDict.Add("今日运势", Fortune.TodaysFortune);
        _gmDict.Add("东方原曲认知", TouhouService.TouhouOstRecog);
        _gmDict.Add("随机东方原曲", TouhouService.RandomTouhouOst);
        _gmCommandDict.Add("/bili", BiliService.BiliQuery);
        _gmCommandDict.Add("/service", ServiceManager.ServiceMgr);
        _gmCommandDict.Add("/gal", GalService.GalServiceInit);
        _gmCommandDict.Add("/th", TouhouService.TouhouServiceInit);
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

    private delegate Task GmDelegate(GroupMessageEventArgs groupMessage);

    private readonly GroupMessageEventArgs _groupMessage;
    private readonly GmDelegate? _action;
    private readonly Dictionary<string, GmDelegate> _gmDict = new();

    #endregion

    #region Command

    private delegate Task GmCommandDelegate(GroupMessageEventArgs groupMessage);

    private readonly GmCommandDelegate? _commandAction;
    private readonly Dictionary<string, GmCommandDelegate> _gmCommandDict = new();

    #endregion
}