using System.Diagnostics;
using AlphalyBot.Tool;
using Makabaka.Models.EventArgs;
using Makabaka.Models.Messages;
using Serilog;

namespace AlphalyBot.Service;

public class Fortune
{
    private readonly string _message;

    private Fortune(long userId)
    {
        var dt = DateTime.Now;
        var ds = dt.ToString("yyyyMMddHHmmss");
        ds = ds.Remove(ds.Length - 6);
        var seed = (userId + ds).GetHashCode();
        Gaussian random = new(seed);
        var fortune = new int[3];
        for (var i = 0; i < 3; i++)
        {
            Debug.Assert(Program.FortuneConfig != null, "Program.FortuneConfig != null");
            var tmp = random.RandomNormal(Program.FortuneConfig.Mu, Program.FortuneConfig.Sigma);
            fortune[i] = (int)tmp;
            random = new Gaussian(seed - 5 * (i + 1));
        }

        _message =
            $@"财运 {fortune[0]}
事业运 {fortune[1]}
桃花运 {fortune[2]}
总之{FortuneRemark(fortune.Sum())}
";
    }

    private static string FortuneRemark(int totalFortune)
    {
        return totalFortune < 20 * 3 ? "差的离谱" : totalFortune < 50 * 3 ? "不好" : totalFortune <= 75 * 3 ? "还行" : "很好";
    }

    public static async Task TodaysFortune(GroupMessageEventArgs groupMessage)
    {
        ServiceManager service = new(groupMessage.GroupId);
        await service.Init();
        if (service.IsServiceEnabled(Services.TodaysFortune))
        {
            Log.Information("Fortune: Command from {0} in {1}", groupMessage.UserId, groupMessage.GroupId);
            Fortune fortune = new(groupMessage.Sender.UserId);
            _ = await groupMessage.ReplyAsync(new TextSegment(fortune._message));
        }
    }
}