using System.Diagnostics;
using System.Runtime.Loader;
using System.Text;
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
using Serilog.Events;

namespace AlphalyBot;

internal static class Program
{
    private const string ConfigPath = "configs/config.json";
    private const string ProgramConfigPath = "configs/startconfig.json";
    private const string FortuneConfigPath = "configs/todaysfortune.json";
    public static List<long> Admins;
    public static FortuneServiceConfig? FortuneConfig;
    public static readonly Dictionary<long, short> TouhouOst = new();

    private static async Task Main(string[] args)
    {
        Console.Clear();
        var beforeDt = DateTime.Now;
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .Enrich.FromLogContext()
            .WriteTo.File("log.txt")
            .WriteTo.Console().CreateLogger()
            ;
        // 加载配置
        ForwardWebSocketServiceConfig? config = null;
        ProgramConfigModel? programConfig = null;
        if (File.Exists(ConfigPath))
            config = JsonConvert.DeserializeObject<ForwardWebSocketServiceConfig>(
                await File.ReadAllTextAsync(ConfigPath));
        if (File.Exists(ProgramConfigPath))
        {
            programConfig =
                JsonConvert.DeserializeObject<ProgramConfigModel>(await File.ReadAllTextAsync(ProgramConfigPath));
        }
        else
        {
            Log.Error("Main: Config file could not be found.");
            Log.Information("Main: Config file created");
            programConfig = new ProgramConfigModel();
            await File.WriteAllTextAsync(ProgramConfigPath,
                JsonConvert.SerializeObject(programConfig, Formatting.Indented));
            Environment.Exit(0);
        }

        if (File.Exists(FortuneConfigPath))
            FortuneConfig =
                JsonConvert.DeserializeObject<FortuneServiceConfig>(await File.ReadAllTextAsync(FortuneConfigPath));
        else
            Log.Warning("Fortune: Today's fortune config file could not be found. Creating a new one.");

        config ??= new ForwardWebSocketServiceConfig();
        FortuneConfig ??= new FortuneServiceConfig();
        await File.WriteAllTextAsync(ConfigPath, JsonConvert.SerializeObject(config, Formatting.Indented));
        await File.WriteAllTextAsync(ProgramConfigPath,
            JsonConvert.SerializeObject(programConfig, Formatting.Indented));
        await File.WriteAllTextAsync(FortuneConfigPath,
            JsonConvert.SerializeObject(FortuneConfig, Formatting.Indented));
        // 初始化服务
        Debug.Assert(programConfig != null, nameof(programConfig) + " != null");
        Admins = programConfig.Admins;
        var service = ServiceFactory.CreateForwardWebSocketService(config);
        Log.Information("Main: Config loaded");
        service.OnGroupMessage += OnGroupMessage;
        service.OnPrivateMessage += OnPrivateMessage;
        service.OnGroupRequest += OnGroupRequest;
        SqlConnector connector = new(programConfig.SqlConnectionString, "utf8mb4_general_ci");
        await connector.Init();
        Log.Information("Main: Database connection created");
        // 启动服务
        CancellationTokenSource cts = new();
        AssemblyLoadContext.Default.Unloading += ctx => cts.Cancel();
        Console.CancelKeyPress += (sender, eventArgs) => cts.Cancel();
        await service.StartAsync();
        var afterDt = DateTime.Now;
        Log.Information("Main: Done ({0}ms)!", afterDt.Subtract(beforeDt).TotalMilliseconds);
        await Task.Run(async () =>
        {
            while (true)
            {
                var command = Console.ReadLine();
                if (command == "exit")
                {
                    Log.Information("Main: Task ended");
                    await service.StopAsync();
                    Environment.Exit(0);
                }
            }
        });

        // 等待取消信号
        try
        {
            await Task.Delay(Timeout.Infinite, cts.Token);
        }
        catch (TaskCanceledException)
        {
            // 服务停止
        }

        Log.Information("Main: Task ended");
        await service.StopAsync();
    }

    private static async void OnGroupRequest(object? sender, GroupRequestEventArgs e)
    {
        Log.Information("GroupRequest: Received group request ({0})", e.GroupId);
        _ = await e.AcceptAsync(); // 有群请求，直接同意
        Log.Information("GroupRequest: Accepted group request ({0})", e.GroupId);
    }

    private static async void OnPrivateMessage(object? sender, PrivateMessageEventArgs e)
    {
        if (e.Message == "测试")
        {
            _ = await e.ReplyAsync(new TextSegment("耶！"));
            Log.Information("PrivateMessage: Received test message from {0}", e.UserId);
        }
        else if (e.Message == "获取好友历史消息记录测试")
        {
            GetFriendMsgHistoryRes res = await e.Context.GetFriendMsgHistoryAsync(e.UserId, e.MessageId, 5);
            StringBuilder sb = new();
            foreach (var message in res.Messages)
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
            Log.Information("PrivateMessage: Received poke from ({0})", e.UserId);
            _ = await e.Context.FriendPokeAsync(e.UserId);
            Log.Information("PrivateMessage: Sent poke to ({0})", e.UserId);
        }
    }

    private static async void OnGroupMessage(object? sender, GroupMessageEventArgs e)
    {
        GroupMessageController messageController = new(e);
        await messageController.Exec();
    }
}