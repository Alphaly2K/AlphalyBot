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
            //            if (e.Message == "测试")
            //            {
            //                await e.ReplyAsync(new TextSegment("耶！"));
            //            }
            //            else if (e.Message == "私聊测试")
            //            {
            //                await e.Context.SendPrivateMessageAsync(e.UserId, new TextSegment("私聊测试！"));
            //            }
            //            else if (e.Message == "表情测试")
            //            {
            //                await e.ReplyAsync(new FaceSegment(0));
            //            }
            //            else if (e.Message == "图片测试")
            //            {
            //                //await e.Reply(new ImageSegment("base64://iVBORw0KGgoAAAANSUhEUgAAABQAAAAVCAIAAADJt1n/AAAAKElEQVQ4EWPk5+RmIBcwkasRpG9UM4mhNxpgowFGMARGEwnBIEJVAAAdBgBNAZf+QAAAAABJRU5ErkJggg=="));
            //                await e.ReplyAsync(ImageSegment.FromLocalFile("test.png"));
            //            }
            //            else if (e.Message == "视频测试")
            //            {
            //                // 注意：这里的uri路径是相对于Lagrange.Onebot程序所在的路径，不是Makabaka程序所在的路径
            //                // 也就是说，你要把test.mp4放到Lagrange.Onebot程序同目录下，才能发
            //                await e.ReplyAsync(new VideoSegment("/home/Lagrange.OneBot/test.mp4"));
            //            }

            //            else if (e.Message == "获取收藏表情测试")
            //            {
            //                List<string> faces = await e.Context.FetchCustomFaceAsync();
            //                await e.ReplyAsync(new TextSegment(string.Join("\n", faces)));
            //            }
            //            else if (e.Message == "获取群组历史消息记录测试")
            //            {
            //                GetGroupMsgHistoryRes res = await e.Context.GetGroupMsgHistoryAsync(e.GroupId, e.MessageId, 5);
            //                var sb = new StringBuilder();
            //                foreach (var message in res.Messages)
            //                {
            //                    sb.Append('[');
            //                    sb.Append(message.UserId);
            //                    sb.Append("]: ");
            //                    sb.Append(message.Message);
            //                    sb.AppendLine();
            //                }
            //                await e.ReplyAsync(new TextSegment(sb.ToString()));
            //            }
            //            else if (e.Message == "上传群文件测试")
            //            {
            //                await e.Context.UploadGroupFileAsync(e.GroupId, "Fleck.dll", "Fleck.dll");
            //            }
            //            else if (e.Message == "获取群根目录文件列表测试")
            //            {
            //                GetGroupRootFilesRes res = await e.Context.GetGroupRootFilesAsync(e.GroupId);
            //                var sb = new StringBuilder();
            //                sb.AppendLine("文件夹列表：");
            //                foreach (var folder in res.Folders)
            //                {
            //                    sb.Append('[');
            //                    sb.Append(folder.FolderId);
            //                    sb.Append(']');
            //                    sb.AppendLine(folder.FolderName);
            //                }
            //                sb.AppendLine("文件列表：");
            //                foreach (var file in res.Files)
            //                {
            //                    sb.Append('[');
            //                    sb.Append(file.FileId);
            //                    sb.Append(']');
            //                    sb.Append('[');
            //                    sb.Append(file.Busid);
            //                    sb.Append(']');
            //                    sb.AppendLine(file.FileName);
            //                }
            //                await e.ReplyAsync(new TextSegment(sb.ToString()));
            //            }
            //            else if (e.Message == "群组戳一戳测试")
            //            {
            //                await e.Context.GroupPokeAsync(e.GroupId, e.UserId);
            //            }

        }
    }
}