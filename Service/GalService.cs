using AlphalyBot.Model;
using AlphalyBot.Tool;
using CodeHollow.FeedReader;
using HtmlAgilityPack;
using Makabaka.Models.EventArgs;
using Makabaka.Models.Messages;
using Serilog;

namespace AlphalyBot.Service;

internal class GalService
{
    private readonly GroupMessageEventArgs _groupMessage;

    private GalService(GroupMessageEventArgs groupMessage)
    {
        _groupMessage = groupMessage;
    }

    public static async Task GalServiceInit(GroupMessageEventArgs groupMessage)
    {
        var handler = new CommandHandler(groupMessage, typeof(GalService));
        await handler.ExecAsync();
    }

    [Service(Services.MonthlyGal, prompt: "monthlynew")]
    private async Task MonthlyNew(string prompt)
    {
        var pictureVisibility = prompt == "true";
        Log.Information("GalgameMonthlyNew: Command from {0} in {1}, picture visibility = {2}", _groupMessage.UserId,
            _groupMessage.GroupId, pictureVisibility);
        var feed = await FeedReader.ReadAsync("https://rsshub.ktachibana.party/ymgal/game/release");
        if (feed != null)
        {
            Message segments = new()
            {
                new TextSegment(feed.Title + "\n\n")
            };
            foreach (var item in feed.Items)
            {
                HtmlDocument doc = new();
                doc.LoadHtml(item.Description);
                var aNode = doc.DocumentNode.SelectSingleNode("//img");
                var bNodes = doc.DocumentNode.SelectNodes("//li");
                if (pictureVisibility) segments.Add(new ImageSegment(aNode.GetAttributeValue("src", "")));

                segments.Add(new TextSegment(item.Title + "\n"));
                segments.Add(new TextSegment($"制作社：{bNodes[0].InnerText}\n"));
                for (short i = 1; i < bNodes.Count; i++) segments.Add(new TextSegment($"{bNodes[i].InnerText} "));
                segments.Add(new TextSegment("\n\n"));
            }

            _ = await _groupMessage.ReplyAsync(segments);
        }
    }
}