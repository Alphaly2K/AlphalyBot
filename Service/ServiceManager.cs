using AlphalyBot.Tool;
using Makabaka.Models.EventArgs;

namespace AlphalyBot.Service
{
    internal enum Services
    {
        TodaysFortune = 0,
        BiliVideoQuery = 1,
        MonthlyGal = 2,
        TouhouOST =3,
    }
    internal class ServiceManager
    {
        public static int ServiceAmount = 4;
        public static string DefaultSettings = "1111";
        public static async Task ServiceMgr(GroupMessageEventArgs groupMessage)
        {
            string[] message = groupMessage.Message.ToString().Split(" ");
            if (message.Length == 3 && Program.Admins.Contains(groupMessage.Sender.UserId))
            {
                ServiceManager manager = new(groupMessage.GroupId);
                await manager.Init();
                Services service;
                try
                {
                    service = (Services)System.Enum.Parse(typeof(Services), message[2]);
                }
                catch (Exception)
                {
                    return;
                }
                if (message[1] == "enable")
                {
                    await manager.EnableService(service);
                }

                if (message[1] == "disable")
                {
                    await manager.DisableService(service);
                }
            }
        }
        private readonly long _groupId;
        public List<char> _services;
        public ServiceManager(long groupId)
        {
            _groupId = groupId;
        }
        public async Task Init()
        {
            SQLConnector connector = new();
            if (!await connector.IsGroupIdExist(_groupId))
            {
                await connector.InsertByGroupId(_groupId, DefaultSettings);
                _services = DefaultSettings.ToList<char>();
                return;
            }
            string tmp = await connector.QueryByGroupId(_groupId);
            _services = tmp.ToList<char>();
            if (_services.Count == ServiceAmount)
            {
                return;
            }

            for (int i = _services.Count - 1; i < ServiceAmount; i++)
            {
                _services.Add(DefaultSettings[i]);
            }
            await connector.ChangeByGroupId(_groupId, new string(_services.ToArray()));
        }
        public async Task EnableService(Services service)
        {
            SQLConnector connector = new();
            _services[(int)service] = '1';
            await connector.ChangeByGroupId(_groupId, new string(_services.ToArray()));
        }
        public async Task DisableService(Services service)
        {
            SQLConnector connector = new();
            _services[(int)service] = '0';
            await connector.ChangeByGroupId(_groupId, new string(_services.ToArray()));
        }
        public bool IsServiceEnabled(Services service)
        {
            return _services[(int)service] == '1';
        }
    }
}
