using AlphalyBot.Controller;
using AlphalyBot.Model;
using AlphalyBot.Tool;
using Makabaka.Configurations;
using Makabaka.Models.API.Responses;
using Makabaka.Models.EventArgs;
using Makabaka.Models.Messages;
using Makabaka.Services;
using Newtonsoft.Json;
using Serilog;
using System.Runtime.Loader;
using System.Text;

namespace AlphalyBot
{
    internal partial class Program
    {
        private const string ConfigPath = "config.json";
        private const string ProgramConfigPath = "startconfig.json";
        public static List<long> Admins;
        public static Dictionary<long, short> TouhouOST = new();
        private static async Task Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .CreateLogger(); // 配置日志

            // 加载配置
            ForwardWebSocketServiceConfig? config = null;
            ProgramConfigModel? programConfig = null;
            if (File.Exists(ConfigPath))
            {
                config = JsonConvert.DeserializeObject<ForwardWebSocketServiceConfig>(File.ReadAllText(ConfigPath));
            }
            if (File.Exists(ProgramConfigPath))
            {
                programConfig = JsonConvert.DeserializeObject<ProgramConfigModel>(File.ReadAllText(ProgramConfigPath));
            }
            else
            {
                Console.WriteLine("配置文件已创建");
                programConfig = new();
                File.WriteAllText(ProgramConfigPath, JsonConvert.SerializeObject(programConfig, Formatting.Indented));
                System.Environment.Exit(0);
            }
            config ??= new();
            File.WriteAllText(ConfigPath, JsonConvert.SerializeObject(config, Formatting.Indented));
            File.WriteAllText(ProgramConfigPath, JsonConvert.SerializeObject(programConfig, Formatting.Indented));
            // 初始化服务
            Admins = programConfig.Admins;
            IWebSocketService service = ServiceFactory.CreateForwardWebSocketService(config);
            service.OnGroupMessage += OnGroupMessage;
            service.OnPrivateMessage += OnPrivateMessage;
            service.OnGroupRequest += OnGroupRequest;
            SQLConnector connector = new(programConfig.SqlConnectionString, "utf8mb4_general_ci");
            await connector.Init();
            // 启动服务
            CancellationTokenSource cts = new();
            AssemblyLoadContext.Default.Unloading += ctx => cts.Cancel();
            Console.CancelKeyPress += (sender, eventArgs) => cts.Cancel();

            await service.StartAsync();
            Console.WriteLine("Done!");
            // 等待取消信号
            try
            {
                await Task.Delay(Timeout.Infinite, cts.Token);
            }
            catch (TaskCanceledException)
            {
                // 服务停止
            }
            await service.StopAsync();
        }

        private static async void OnGroupRequest(object? sender, GroupRequestEventArgs e)
        {
            _ = await e.AcceptAsync(); // 有群请求，直接同意
        }

        private static async void OnPrivateMessage(object? sender, PrivateMessageEventArgs e)
        {
            if (e.Message == "测试")
            {
                _ = await e.ReplyAsync(new TextSegment("耶！"));
            }
            else if (e.Message == "获取好友历史消息记录测试")
            {
                GetFriendMsgHistoryRes res = await e.Context.GetFriendMsgHistoryAsync(e.UserId, e.MessageId, 5);
                StringBuilder sb = new();
                foreach (PrivateMessage? message in res.Messages)
                {
                    _ = sb.Append('[');
                    _ = sb.Append(message.Sender.UserId);
                    _ = sb.Append(']');
                    _ = sb.Append(message.Sender.NickName);
                    _ = sb.Append(": ");
                    _ = sb.Append(message.Message);
                    _ = sb.AppendLine();
                }
                _ = await e.ReplyAsync(new TextSegment(sb.ToString()));
            }
            else if (e.Message == "私聊发送文件测试")
            {
                _ = await e.Context.UploadPrivateFileAsync(e.UserId, "Fleck.dll", "Fleck.dll");
            }
            else if (e.Message == "好友戳一戳测试")
            {
                _ = await e.Context.FriendPokeAsync(e.UserId);
            }
        }

        private static async void OnGroupMessage(object? sender, GroupMessageEventArgs e)
        {
            GroupMessageController messageController = new(e);
            await messageController.Exec();
        }
    }
}