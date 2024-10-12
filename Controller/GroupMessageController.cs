using AlphalyBot.Service;
using Makabaka.Models.EventArgs;

namespace AlphalyBot.Controller;

internal class GroupMessageController
{
    public GroupMessageController(GroupMessageEventArgs groupMessage)
    {
        _groupMessage = groupMessage;
        AddDelegate();
        if (GMDict.ContainsKey(_groupMessage.Message)) _action = GMDict[_groupMessage.Message];

        var MessageSplit = groupMessage.Message.ToString().Split(" ");
        if (MessageSplit.Length > 0 && GMCommandDict.ContainsKey(MessageSplit[0]))
            _commandAction = GMCommandDict[MessageSplit[0]];
    }

    private void AddDelegate()
    {
        GMDict.Add("今日运势", Fortune.TodaysFortune);
        GMDict.Add("东方原曲认知", TouhouService.TouhouOSTRecognise);
        GMDict.Add("随机东方原曲", TouhouService.RandomTouhouOST);
        GMCommandDict.Add("/bili", BiliService.BiliQuery);
        GMCommandDict.Add("/service", ServiceManager.ServiceMgr);
        GMCommandDict.Add("/gal", GalService.GalServiceInit);
        GMCommandDict.Add("/th", TouhouService.TouhouServiceInit);
    }

    public async Task Exec()
    {
        if (_action != null) await _action.Invoke(_groupMessage);

        if (_commandAction != null) await _commandAction.Invoke(_groupMessage);
    }

    #region General

    public delegate Task GMDelegate(GroupMessageEventArgs groupMessage);

    private readonly GroupMessageEventArgs _groupMessage;
    private readonly GMDelegate? _action;
    private readonly Dictionary<string, GMDelegate> GMDict = new();

    #endregion

    #region Command

    public delegate Task GMCommandDelegate(GroupMessageEventArgs groupMessage);

    private readonly GMCommandDelegate? _commandAction;
    private readonly Dictionary<string, GMCommandDelegate> GMCommandDict = new();

    #endregion
}