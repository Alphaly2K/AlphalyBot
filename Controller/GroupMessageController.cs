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
        if (_action != null) await _action.Invoke(_groupMessage);

        if (_commandAction != null) await _commandAction.Invoke(_groupMessage);
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