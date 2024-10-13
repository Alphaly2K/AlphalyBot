namespace AlphalyBot.Model;

internal abstract class BilibiliVideoModel
{
    public class Rootobject
    {
        public int code { get; set; }
        public string message { get; set; }
        public int ttl { get; set; }
        public Data data { get; set; }
    }

    public class Data
    {
        public string bvid { get; set; }
        public long aid { get; set; }
        public int videos { get; set; }
        public int tid { get; set; }
        public string tname { get; set; }
        public int copyright { get; set; }
        public string pic { get; set; }
        public string title { get; set; }
        public int pubdate { get; set; }
        public int ctime { get; set; }
        public string desc { get; set; }
        public Desc_V2[] desc_v2 { get; set; }
        public int state { get; set; }
        public int duration { get; set; }
        public int mission_id { get; set; }
        public Rights rights { get; set; }
        public Owner owner { get; set; }
        public Stat stat { get; set; }
        public Argue_Info argue_info { get; set; }
        public string dynamic { get; set; }
        public long cid { get; set; }
        public Dimension dimension { get; set; }
        public object premiere { get; set; }
        public int teenage_mode { get; set; }
        public bool is_chargeable_season { get; set; }
        public bool is_story { get; set; }
        public bool is_upower_exclusive { get; set; }
        public bool is_upower_play { get; set; }
        public bool is_upower_preview { get; set; }
        public int enable_vt { get; set; }
        public string vt_display { get; set; }
        public bool no_cache { get; set; }
        public Page[] pages { get; set; }
        public Subtitle subtitle { get; set; }
        public Staff[] staff { get; set; }
        public bool is_season_display { get; set; }
        public User_Garb user_garb { get; set; }
        public Honor_Reply honor_reply { get; set; }
        public string like_icon { get; set; }
        public bool need_jump_bv { get; set; }
        public bool disable_show_up_info { get; set; }
        public int is_story_play { get; set; }
        public bool is_view_self { get; set; }
    }

    public class Rights
    {
        public int bp { get; set; }
        public int elec { get; set; }
        public int download { get; set; }
        public int movie { get; set; }
        public int pay { get; set; }
        public int hd5 { get; set; }
        public int no_reprint { get; set; }
        public int autoplay { get; set; }
        public int ugc_pay { get; set; }
        public int is_cooperation { get; set; }
        public int ugc_pay_preview { get; set; }
        public int no_background { get; set; }
        public int clean_mode { get; set; }
        public int is_stein_gate { get; set; }
        public int is_360 { get; set; }
        public int no_share { get; set; }
        public int arc_pay { get; set; }
        public int free_watch { get; set; }
    }

    public class Owner
    {
        public int mid { get; set; }
        public string name { get; set; }
        public string face { get; set; }
    }

    public class Stat
    {
        public long aid { get; set; }
        public int view { get; set; }
        public int danmaku { get; set; }
        public int reply { get; set; }
        public int favorite { get; set; }
        public int coin { get; set; }
        public int share { get; set; }
        public int now_rank { get; set; }
        public int his_rank { get; set; }
        public int like { get; set; }
        public int dislike { get; set; }
        public string evaluation { get; set; }
        public int vt { get; set; }
    }

    public class Argue_Info
    {
        public string argue_msg { get; set; }
        public int argue_type { get; set; }
        public string argue_link { get; set; }
    }

    public class Dimension
    {
        public int width { get; set; }
        public int height { get; set; }
        public int rotate { get; set; }
    }

    public class Subtitle
    {
        public bool allow_submit { get; set; }
        public List[] list { get; set; }
    }

    public class List
    {
        public long id { get; set; }
        public string lan { get; set; }
        public string lan_doc { get; set; }
        public bool is_lock { get; set; }
        public string subtitle_url { get; set; }
        public int type { get; set; }
        public string id_str { get; set; }
        public int ai_type { get; set; }
        public int ai_status { get; set; }
        public Author author { get; set; }
    }

    public class Author
    {
        public int mid { get; set; }
        public string name { get; set; }
        public string sex { get; set; }
        public string face { get; set; }
        public string sign { get; set; }
        public int rank { get; set; }
        public int birthday { get; set; }
        public int is_fake_account { get; set; }
        public int is_deleted { get; set; }
        public int in_reg_audit { get; set; }
        public int is_senior_member { get; set; }
        public object name_render { get; set; }
    }

    public class User_Garb
    {
        public string url_image_ani_cut { get; set; }
    }

    public class Honor_Reply
    {
        public Honor[] honor { get; set; }
    }

    public class Honor
    {
        public long aid { get; set; }
        public int type { get; set; }
        public string desc { get; set; }
        public int weekly_recommend_num { get; set; }
    }

    public class Desc_V2
    {
        public string raw_text { get; set; }
        public int type { get; set; }
        public int biz_id { get; set; }
    }

    public class Page
    {
        public long cid { get; set; }
        public int page { get; set; }
        public string from { get; set; }
        public string part { get; set; }
        public int duration { get; set; }
        public string vid { get; set; }
        public string weblink { get; set; }
        public Dimension1 dimension { get; set; }
    }

    public class Dimension1
    {
        public int width { get; set; }
        public int height { get; set; }
        public int rotate { get; set; }
    }

    public class Staff
    {
        public int mid { get; set; }
        public string title { get; set; }
        public string name { get; set; }
        public string face { get; set; }
        public Vip vip { get; set; }
        public Official official { get; set; }
        public int follower { get; set; }
        public int label_style { get; set; }
    }

    public class Vip
    {
        public int type { get; set; }
        public int status { get; set; }
        public long due_date { get; set; }
        public int vip_pay_type { get; set; }
        public int theme_type { get; set; }
        public Label label { get; set; }
        public int avatar_subscript { get; set; }
        public string nickname_color { get; set; }
        public int role { get; set; }
        public string avatar_subscript_url { get; set; }
        public int tv_vip_status { get; set; }
        public int tv_vip_pay_type { get; set; }
        public int tv_due_date { get; set; }
        public Avatar_Icon avatar_icon { get; set; }
    }

    public class Label
    {
        public string path { get; set; }
        public string text { get; set; }
        public string label_theme { get; set; }
        public string text_color { get; set; }
        public int bg_style { get; set; }
        public string bg_color { get; set; }
        public string border_color { get; set; }
        public bool use_img_label { get; set; }
        public string img_label_uri_hans { get; set; }
        public string img_label_uri_hant { get; set; }
        public string img_label_uri_hans_static { get; set; }
        public string img_label_uri_hant_static { get; set; }
    }

    public class Avatar_Icon
    {
        public int icon_type { get; set; }
        public Icon_Resource icon_resource { get; set; }
    }

    public class Icon_Resource
    {
    }

    public class Official
    {
        public int role { get; set; }
        public string title { get; set; }
        public string desc { get; set; }
        public int type { get; set; }
    }
}