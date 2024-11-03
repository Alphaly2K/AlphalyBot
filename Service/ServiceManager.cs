using AlphalyBot.Model;
using AlphalyBot.Tool;
using Makabaka.Models.EventArgs;
using Serilog;

namespace AlphalyBot.Service;

public enum Services
{
    ServiceManager = -1,
    TodaysFortune = 0,
    BiliVideoQuery = 1,
    MonthlyGal = 2,
    TouhouOstRecog = 3,
    RandomTouhouOst = 4
}

internal class ServiceManager: IPlugin
{
    private const int ServiceAmount = 5;
    private const string DefaultSettings = "11111";
    private readonly long _groupId;
    private List<char> _services;

    public ServiceManager(long groupId)
    {
        _groupId = groupId;
    }

    [Service(Services.ServiceManager, true)]
    public static async Task ServiceMgr(GroupMessageEventArgs groupMessage)
    {
        var message = groupMessage.Message.ToString().Split(" ");
        if (message.Length == 3 && Program.Admins.Contains(groupMessage.Sender.UserId))
        {
            ServiceManager manager = new(groupMessage.GroupId);
            await manager.Init();
            Services service;
            try
            {
                service = (Services)Enum.Parse(typeof(Services), message[2]);
            }
            catch (Exception)
            {
                Log.Error("ServiceMgr: Service not found {0} in {1}", message[2], groupMessage.GroupId);
                return;
            }

            if (message[1] == "enable") await manager.EnableService(service);

            if (message[1] == "disable") await manager.DisableService(service);
        }
    }

    public async Task Init()
    {
        SqlConnector connector = new();
        if (!await connector.IsGroupIdExist(_groupId))
        {
            await connector.InsertByGroupId(_groupId, DefaultSettings);
            _services = DefaultSettings.ToList();
            return;
        }

        var tmp = await connector.QueryByGroupId(_groupId);
        _services = tmp.ToList();
        if (_services.Count == ServiceAmount) return;

        for (var i = _services.Count - 1; i < ServiceAmount; i++) _services.Add(DefaultSettings[i]);
        await connector.ChangeByGroupId(_groupId, new string(_services.ToArray()));
    }

    private async Task EnableService(Services service)
    {
        SqlConnector connector = new();
        _services[(int)service] = '1';
        await connector.ChangeByGroupId(_groupId, new string(_services.ToArray()));
        Log.Information("ServiceMgr: Enabled service {0} in {1}", service.ToString(), _groupId);
    }

    private async Task DisableService(Services service)
    {
        SqlConnector connector = new();
        _services[(int)service] = '0';
        await connector.ChangeByGroupId(_groupId, new string(_services.ToArray()));
        Log.Information("ServiceMgr: Disabled service {0} in {1}", service.ToString(), _groupId);
    }

    public bool IsServiceEnabled(Services service)
    {
        if (service != Services.ServiceManager)
            return _services[(int)service] == '1';
        return true;
    }
}