using AlphalyBot.Tool;
using Makabaka.Models.EventArgs;
using Makabaka.Models.Messages;

namespace AlphalyBot.Service
{
    public class Fortune
    {
        private const string ConfigPath = "configs/todaysfortune.json";
        public readonly string Message;
        public Fortune(long UserId)
        {
            // to-do
            //FortuneServiceConfig? config = null;
            //if (File.Exists(ConfigPath))
            //{
            //    config = JsonConvert.DeserializeObject<FortuneServiceConfig>(File.ReadAllText(ConfigPath));
            //}
            //config ??= new();
            //File.WriteAllText(ConfigPath, JsonConvert.SerializeObject(config, Formatting.Indented));
            DateTime dt = DateTime.Now;
            string ds = dt.ToString("yyyyMMddHHmmss");
            ds = ds.Remove(ds.Length - 6);
            int seed = (UserId.ToString() + ds).GetHashCode();
            Gaussian random = new(seed);
            List<int> fortune = new();
            double tmp;
            for (int i = 0; i < 3; i++)
            {
                tmp = random.RandomNormal(50, 12);
                fortune.Add((int)tmp);
                random = new Gaussian(seed - (5 * (i + 1)));
            }
            Message =
$@"财运 {fortune[0]}
事业运 {fortune[1]}
桃花运 {fortune[2]}
总之是一个{(fortune[0] + fortune[1] + fortune[2]) / 3}
";
        }
        public static async Task TodaysFortune(GroupMessageEventArgs groupMessage)
        {
            ServiceManager service = new(groupMessage.GroupId);
            await service.Init();
            if (service.IsServiceEnabled(Services.TodaysFortune))
            {
                Fortune fortune = new(groupMessage.Sender.UserId);
                _ = await groupMessage.ReplyAsync(new TextSegment(fortune.Message));
            }
        }
    }
}
