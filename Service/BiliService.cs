using System.Net;
using AlphalyBot.Model;
using Makabaka.Models.EventArgs;
using Makabaka.Models.Messages;
using Newtonsoft.Json;
using RestSharp;
using Serilog;

namespace AlphalyBot.Service;

internal class BiliService
{
    private readonly RestClient _client = new(@"https://api.bilibili.com/x/web-interface/view");
    private readonly RestRequest _request = new();

    private BiliService(string bv)
    {
        _ = _request.AddParameter("bvid", bv);
    }

    private async Task<BilibiliVideoModel.Rootobject?> Exec()
    {
        var response = await _client.ExecuteAsync(_request);
        BilibiliVideoModel.Rootobject? video = new();
        if (response.StatusCode == HttpStatusCode.OK && response.Content != null)
            video = JsonConvert.DeserializeObject<BilibiliVideoModel.Rootobject>(response.Content);
        return video;
    }

    [Service(Services.BiliVideoQuery)]
    public static async Task BiliQuery(GroupMessageEventArgs groupMessage)
    {
        ServiceManager service = new(groupMessage.GroupId);
        await service.Init();
        if (groupMessage.Message.ToString().Split(" ").Length == 2 && service.IsServiceEnabled(Services.BiliVideoQuery))
        {
            Log.Information("BiliQuery: Command from ({0}) in {1}", groupMessage.UserId, groupMessage.GroupId);
            var bvNumber = groupMessage.Message.ToString().Split(" ")[1];
            BiliService bili = new(bvNumber);
            var video = await bili.Exec();
            Message segments = new();
            if (video?.data != null)
            {
                segments.Add(new ImageSegment(video.data.pic));
                segments.Add(new TextSegment(video.data.title + "\n"));
                segments.Add(new TextSegment("简介：" + (video.data.desc ?? "无") + "\n"));
                segments.Add(new TextSegment("UP主：" + video.data.owner.name + "\n"));
                segments.Add(new TextSegment("浏览：" + video.data.stat.view + "\n"));
                segments.Add(new TextSegment("点赞：" + video.data.stat.like + "\n"));
                segments.Add(new TextSegment("投币：" + video.data.stat.coin + "\n"));
                segments.Add(new TextSegment("Link：" + $"https://www.bilibili.com/{bvNumber}"));
            }
            else
            {
                segments.Add(new TextSegment("该BV号不存在或该视频无法访问"));
            }

            _ = await groupMessage.ReplyAsync(segments);
        }
    }
}