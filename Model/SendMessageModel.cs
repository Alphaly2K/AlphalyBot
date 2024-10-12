namespace AlphalyBot.Model;

internal class SendMessageModel
{
    public class Rootobject
    {
        public long group_id { get; set; }
        public Message message { get; set; }
    }

    public class Message
    {
        public string type { get; set; }
        public Data data { get; set; }
    }

    public class Data
    {
        public string file { get; set; }
    }
}