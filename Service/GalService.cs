using CodeHollow.FeedReader;
using HtmlAgilityPack;
using Makabaka.Models.EventArgs;
using Makabaka.Models.Messages;

namespace AlphalyBot.Service
{
    internal class GalService
    {
        public static async Task GalServiceInit(GroupMessageEventArgs groupMessage)
        {
            ServiceManager service = new(groupMessage.GroupId);
            await service.Init();
            GalService galService = new(groupMessage);
            if (groupMessage.Message.ToString().Split(" ")[1] == "monthlynew" && await service.IsServiceEnabled(Services.MonthlyGal))
            {
                string? argument = (groupMessage.Message.ToString().Split(" ").Length == 3) ? groupMessage.Message.ToString().Split(" ")[2] : null;
                if (argument == "true")
                {
                    await galService.MonthlyNew(true);
                }

                if (argument is "false" or null)
                {
                    await galService.MonthlyNew();
                }
            }

        }
        public GalService(GroupMessageEventArgs groupMessage)
        {
            _groupMessage = groupMessage;
        }
        private readonly GroupMessageEventArgs _groupMessage;
        public async Task MonthlyNew(bool PictureVisibility = false)
        {
            Feed feed = await FeedReader.ReadAsync("https://rsshub.ktachibana.party/ymgal/game/release");
            if (feed != null)
            {
                Message segments = new()
                {
                    new TextSegment(feed.Title + "\n\n")
                };
                foreach (FeedItem? item in feed.Items)
                {
                    HtmlDocument doc = new();
                    doc.LoadHtml(item.Description);
                    HtmlNode aNode = doc.DocumentNode.SelectSingleNode("//img");
                    HtmlNodeCollection bNodes = doc.DocumentNode.SelectNodes("//li");
                    if (PictureVisibility)
                    {
                        segments.Add(new ImageSegment(aNode.GetAttributeValue("src", "")));
                    }

                    segments.Add(new TextSegment(item.Title + "\n"));
                    segments.Add(new TextSegment($"制作社：{bNodes[0].InnerText}\n"));
                    for (short i = 1; i < bNodes.Count; i++)
                    {
                        segments.Add(new TextSegment($"{bNodes[i].InnerText} "));
                    }
                    segments.Add(new TextSegment("\n\n"));
                }
                _ = await _groupMessage.ReplyAsync(segments);
            }

        }
    }
}
