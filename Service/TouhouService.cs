﻿using AlphalyBot.Model;
using AlphalyBot.Tool;
using Makabaka.Models.EventArgs;
using Makabaka.Models.Messages;
using Newtonsoft.Json;
using RestSharp;
using Serilog;

namespace AlphalyBot.Service;

internal class TouhouService: IPlugin
{
    public string Name => "Touhou";
    public string Description => "Touhou";
    public string Author => "Alphaly";
    public string Version => "1.0";

    public HashSet<string> ServiceList => new List<string>();
    //来自：https://github.com/XiaoGeNintendo/TouhouSongRecognitiveTest
    private static readonly List<string> OstKey = new()
    {
        "A Sacred Lot!!!东方灵异传",
        "永远之巫女!!!东方灵异传",
        "The Positive and Negative!!!东方灵异传",
        "Highly Responsive to Prayers!!!东方灵异传",
        "东方怪奇谈!!!东方灵异传",
        "天使传说!!!东方灵异传",
        "Oriental Magician!!!东方灵异传",
        "破邪的小太刀!!!东方灵异传",
        "魔镜!!!东方灵异传",
        "The Legend of KAGE!!!东方灵异传",
        "来吧，直到倒地死去的那一刻!!!东方灵异传",
        "同归于尽!!!东方灵异传",
        "星幽剑士!!!东方灵异传",
        "鸢尾花!!!东方灵异传",
        "风之神社!!!东方灵异传",
        "灵魂安息之所!!!东方怪绮谈",
        "Plastic Space!!!秋霜玉原型",
        "Inventive City!!!秋霜玉原型",
        "秋霜玉　～ Clockworks!!!秋霜玉",
        "False Strawberry!!!秋霜玉",
        "Primrose Shiver!!!秋霜玉",
        "幻想帝都!!!秋霜玉",
        "Disastrous Gemini!!!秋霜玉",
        "华之幻想 红梦之宙!!!秋霜玉",
        "天空军团!!!秋霜玉",
        "斯普特尼克幻夜!!!秋霜玉",
        "机械马戏团　～ Reverie!!!秋霜玉",
        "卡纳维拉尔角的梦幻少女!!!秋霜玉",
        "魔法少女十字军!!!秋霜玉",
        "Antique Terror!!!秋霜玉",
        "梦机械　～ Innocent Power!!!秋霜玉",
        "幻想科学　～ Doll’s Phantom!!!秋霜玉",
        "少女神性　～ Pandora’s Box!!!秋霜玉",
        "Silk Road Alice!!!秋霜玉",
        "魔女们的舞会　～ Magus!!!秋霜玉",
        "二色莲花蝶　～ Ancients!!!秋霜玉",
        "Herselves!!!秋霜玉",
        "Titled Maid!!!秋霜玉",
        "Witch of Love Potion!!!Torte Le Magic",
        "Magical and Hopeless!!!Torte Le Magic",
        "Sacred Battle!!!Torte Le Magic",
        "稀翁玉　～ Fairy Dance!!!稀翁玉",
        "天鹅绒少女战　～ Velvet Battle!!!稀翁玉",
        "Castle Explorer -in the Sky-!!!稀翁玉",
        "俄耳甫斯的诗　～ Pseudoclassic!!!稀翁玉",
        "新幻想　～ New Fantasy!!!稀翁玉",
        "奥尔良的圣骑士!!!稀翁玉",
        "My Maid, Sweet Maid!!!稀翁玉",
        "樱花之恋塚　～ Flower of Japan!!!稀翁玉",
        "少女战士　～ Heart of Valkyrie!!!稀翁玉",
        "神秘的人偶　～ God Knows!!!稀翁玉",
        "宵暗的魔术师!!!东方幻想的音乐",
        "Magic of Life!!!东方幻想的音乐",
        "梦幻回廊!!!东方幻想的音乐",
        "蓬莱幻想　～ far East!!!东方幻想的音乐",
        "飞翔在夜晚的鸠山!!!东方幻想的音乐",
        "比赤色更红的梦!!!东方红魔乡",
        "如鬼灯般的红色之魂!!!东方红魔乡",
        "妖魔夜行!!!东方红魔乡",
        "Lunate Elf!!!东方红魔乡",
        "活泼的纯情小姑娘!!!东方红魔乡",
        "上海红茶馆　～ Chinese Tea!!!东方红魔乡",
        "明治十七年的上海爱丽丝!!!东方红魔乡",
        "巴瓦鲁魔法图书馆!!!东方红魔乡",
        "Locked Girl　～ 少女密室!!!东方红魔乡",
        "女仆与血之怀表!!!东方红魔乡",
        "月时计　～ Luna Dial!!!东方红魔乡",
        "特佩斯的年幼末裔!!!东方红魔乡",
        "献给已逝公主的七重奏!!!东方红魔乡",
        "魔法少女们的百年祭!!!东方红魔乡",
        "U.N.OWEN就是她吗？!!!东方红魔乡",
        "比红色更虚无的永远!!!东方红魔乡",
        "红楼　～ Eastern Dream...!!!东方红魔乡",
        "蓬莱传说!!!蓬莱人形",
        "人偶之森!!!蓬莱人形",
        "空中飞翔的巫女不可思议的每天!!!蓬莱人形",
        "妖妖梦　～ Snow or Cherry Petal!!!东方妖妖梦",
        "无何有之乡　～ Deep Mountain!!!东方妖妖梦",
        "Crystallize Silver!!!东方妖妖梦",
        "远野幻想物语!!!东方妖妖梦",
        "凋叶棕（withered leaf）!!!东方妖妖梦",
        "布加勒斯特的人偶师!!!东方妖妖梦",
        "人偶裁判　～ 玩弄人形的少女!!!东方妖妖梦",
        "天空的花都!!!东方妖妖梦",
        "幽灵乐团　～ Phantom Ensemble!!!东方妖妖梦",
        "东方妖妖梦　～ Ancient Temple!!!东方妖妖梦",
        "广有射怪鸟事　～ Till When？!!!东方妖妖梦",
        "Ultimate Truth!!!东方妖妖梦",
        "幽雅地绽放吧，墨染的樱花　～ Border of Life!!!东方妖妖梦",
        "Border of Life!!!东方妖妖梦",
        "妖妖跋扈!!!东方妖妖梦",
        "少女幻葬　～ Necro-Fantasy!!!东方妖妖梦",
        "妖妖跋扈　～ Who done it！!!!东方妖妖梦",
        "Necro-Fantasia!!!东方妖妖梦",
        "春风之梦!!!东方妖妖梦",
        "樱花樱花　～ Japanize Dream...!!!东方妖妖梦",
        "走在夜晚的莲台野!!!莲台野夜行",
        "少女秘封俱乐部!!!莲台野夜行",
        "古老的冥界寺!!!莲台野夜行",
        "魔术师梅莉!!!莲台野夜行",
        "月之妖鸟、化猫之幻!!!莲台野夜行",
        "过去的花　～ Fairy of Flower!!!莲台野夜行",
        "幻想的永远祭!!!莲台野夜行",
        "永夜抄　～ Eastern Night.!!!东方永夜抄",
        "幻视之夜　～ Ghostly Eyes!!!东方永夜抄",
        "蠢蠢的秋月　～ Mooned Insect!!!东方永夜抄",
        "夜雀的歌声　～ Night Bird!!!东方永夜抄",
        "已经只能听见歌声了!!!东方永夜抄",
        "令人怀念的东方之血　～ Old World!!!东方永夜抄",
        "Plain Asia!!!东方永夜抄",
        "永夜的报应　～ Imperishable Night!!!东方永夜抄",
        "灰姑娘的笼子　～ Kagome-Kagome!!!东方永夜抄",
        "狂气之瞳　～ Invisible Full Moon!!!东方永夜抄",
        "Voyage1969!!!东方永夜抄",
        "千年幻想乡　～ History of the Moon!!!东方永夜抄",
        "竹取飞翔　～ Lunatic Princess!!!东方永夜抄",
        "Voyage1970!!!东方永夜抄",
        "Extend Ash　～ 蓬莱人!!!东方永夜抄",
        "飘上月球，不死之烟!!!东方永夜抄",
        "月见草!!!东方永夜抄",
        "Eternal Dream ～ 幽玄的枫树!!!东方永夜抄",
        "东方妖怪小町!!!东方永夜抄",
        "萃梦想!!!东方萃梦想",
        "东方萃梦想!!!东方萃梦想",
        "Demystify Feast!!!东方萃梦想",
        "夜幕降临　～ Evening Star!!!东方萃梦想",
        "御伽之国的鬼岛　～ Missing Power!!!东方萃梦想",
        "夏明!!!东方萃梦想",
        "魔所!!!东方萃梦想",
        "月轮!!!东方萃梦想",
        "遍参!!!东方萃梦想",
        "内心!!!东方萃梦想",
        "间奏曲!!!东方萃梦想",
        "东风!!!东方萃梦想",
        "森闲!!!东方萃梦想",
        "仰空!!!东方萃梦想",
        "幽境!!!东方萃梦想",
        "稀客!!!东方萃梦想",
        "红夜!!!东方萃梦想",
        "战迅!!!东方萃梦想",
        "祸机!!!东方萃梦想",
        "碎月!!!东方萃梦想",
        "童祭　～ Innocent Treasures!!!梦违科学世纪",
        "华胥之梦!!!梦违科学世纪",
        "科学世纪的少年少女!!!梦违科学世纪",
        "梦境与现实的境界!!!梦违科学世纪",
        "风神少女!!!东方文花帖（书籍）",
        "花映塚　～ Higan Retour!!!东方花映塚",
        "春色小径　～ Colorful Path!!!东方花映塚",
        "Oriental Dark Flight!!!东方花映塚",
        "Flowering Night!!!东方花映塚",
        "宇佐大人的白旗!!!东方花映塚",
        "剧毒身体　～ Forsaken Doll!!!东方花映塚",
        "今昔幻想乡　～ Flower Land!!!东方花映塚",
        "彼岸归航　～ Riverside View!!!东方花映塚",
        "第六十年的东方审判　～ Fate of Sixty Years!!!东方花映塚",
        "映花之塚!!!东方花映塚",
        "此岸之塚!!!东方花映塚",
        "花如幻想一般!!!东方花映塚",
        "魂之花　～ Another Dream...!!!东方花映塚",
        "天狗的笔记　～　Mysterious Note!!!东方文花帖",
        "风的循环　～　Wind Tour!!!东方文花帖",
        "天狗正凝视着　～　Black Eyes!!!东方文花帖",
        "东之国的不眠夜!!!东方文花帖",
        "回忆京都!!!东方文花帖",
        "广重36号　～ Neo Super-Express!!!卯酉东海道",
        "53分钟的蓝色大海!!!卯酉东海道",
        "青木原的传说!!!卯酉东海道",
        "最澄澈的空与海!!!卯酉东海道",
        "欢迎来到月面旅行团!!!大空魔术",
        "天空的格林尼治!!!大空魔术",
        "轮椅上的未来宇宙!!!大空魔术",
        "卫星露天咖啡座!!!大空魔术",
        "G Free!!!大空魔术",
        "另一侧的月!!!大空魔术",
        "Japanese Saga!!!东方求闻史纪",
        "阿礼的孩子!!!东方求闻史纪",
        "Sunny Rutile Flection!!!东方三月精E",
        "因夜失眠!!!东方三月精E",
        "妖精灿烂的样子!!!东方三月精E",
        "被封印的众神!!!东方风神录",
        "眷爱众生之神　～ Romantic Fall!!!东方风神录",
        "会受稻田姬的斥责啦!!!东方风神录",
        "厄神降临之路　～ Dark Road!!!东方风神录",
        "命运的阴暗面!!!东方风神录",
        "众神眷恋的幻想乡!!!东方风神录",
        "芥川龙之介的河童　～ Candid Friend!!!东方风神录",
        "Fall of Fall　～ 秋意渐浓之瀑!!!东方风神录",
        "妖怪之山　～ Mysterious Mountain!!!东方风神录",
        "少女曾见的日本原风景!!!东方风神录",
        "信仰是为了虚幻之人!!!东方风神录",
        "御柱的墓场　～ Grave of Being!!!东方风神录",
        "神圣庄严的古战场　～ Suwa Foughten Field!!!东方风神录",
        "明日之盛，昨日之俗!!!东方风神录",
        "Native Faith!!!东方风神录",
        "山脚的神社!!!东方风神录",
        "神明降下恩惠之雨　～ Sylphid Dream!!!东方风神录",
        "Player’s Score!!!东方风神录",
        "酒鬼的雷姆利亚!!!黄昏酒场",
        "Theme of Eastern Story!!!幺乐团的历史",
        "桑尼米尔克的红雾异变!!!东方三月精S1",
        "雪月樱花之国!!!东方三月精S1",
        "Star Voyage2008!!!东方三月精S1",
        "妖怪宇宙旅行!!!东方儚月抄（漫画）",
        "绵月的符卡　～ Lunatic Blue!!!东方儚月抄（漫画）",
        "绯想天!!!东方绯想天",
        "日常坐卧!!!东方绯想天",
        "地之色乃黄色!!!东方绯想天",
        "甲论乙驳!!!东方绯想天",
        "风光明媚!!!东方绯想天",
        "散发着香气的树叶花!!!东方绯想天",
        "飞舞的水飞沫!!!东方绯想天",
        "以鱼驱蝇!!!东方绯想天",
        "放纵不羁!!!东方绯想天",
        "嘲讽的游戏!!!东方绯想天",
        "冷吟闲醉!!!东方绯想天",
        "云外苍天!!!东方绯想天",
        "黑海中的绯红　～ Legendary Fish!!!东方绯想天",
        "天衣无缝!!!东方绯想天",
        "有顶天变　～ Wonderful Heaven!!!东方绯想天",
        "幼小的有顶天!!!东方绯想天",
        "暮色苍然!!!东方绯想天",
        "AN ORDEAL FROM GOD!!!神魔讨绮传",
        "地灵们的起床!!!东方地灵殿",
        "昏暗的风穴!!!东方地灵殿",
        "被封印的妖怪 ～ Lost Place!!!东方地灵殿",
        "阻绝人迹之桥!!!东方地灵殿",
        "绿眼的嫉妒!!!东方地灵殿",
        "漫游旧地狱街道!!!东方地灵殿",
        "大江山的花之酒宴!!!东方地灵殿",
        "Heartfelt Fancy!!!东方地灵殿",
        "少女觉 ～ 3rd eye!!!东方地灵殿",
        "废狱摇篮曲!!!东方地灵殿",
        "尸体旅行　～ Be of good cheer！!!!东方地灵殿",
        "业火地幔!!!东方地灵殿",
        "灵知的太阳信仰　～ Nuclear Fusion!!!东方地灵殿",
        "Last Remote!!!东方地灵殿",
        "哈德曼的妖怪少女!!!东方地灵殿",
        "地灵们的归家!!!东方地灵殿",
        "能源黎明 ～ Future Dream...!!!东方地灵殿",
        "可爱的大战争叠奏曲!!!东方三月精S2",
        "珍奇的上海古牌!!!东方幻想麻将",
        "魔法使的忧郁!!!The Grimoire of Marisa",
        "青空之影!!!东方星莲船",
        "春之岸边!!!东方星莲船",
        "小小的贤将!!!东方星莲船",
        "封闭的云中通路!!!东方星莲船",
        "请注意万年备用伞!!!东方星莲船",
        "Sky Ruin!!!东方星莲船",
        "守旧老爹与前卫少女!!!东方星莲船",
        "幽灵客船的穿越时空之旅!!!东方星莲船",
        "Captain Murasa!!!东方星莲船",
        "魔界地方都市秘境!!!东方星莲船",
        "虎纹的毘沙门天!!!东方星莲船",
        "法界之火!!!东方星莲船",
        "感情的摩天楼　～ Cosmic Mind!!!东方星莲船",
        "夜空中的UFO恋曲!!!东方星莲船",
        "平安时代的外星人!!!东方星莲船",
        "妖怪寺!!!东方星莲船",
        "空中的归路　～　Sky Dream!!!东方星莲船",
        "你见到了那个影子吗!!!东方非想天则",
        "传说的巨神!!!东方非想天则",
        "我们的非想天则!!!东方非想天则",
        "人偶存在的风景!!!东方非想天则",
        "悠久的蒸汽机关!!!东方非想天则",
        "未知物Ｘ　～ Unfound Adventure!!!东方非想天则",
        "空中飘浮的物体Ｘ!!!东方非想天则",
        "巨大的影子与渺小的结局!!!东方非想天则",
        "妖精冒险谭!!!东方三月精S3",
        "两个世界!!!东方三月精S3",
        "Newshound!!!DS东方文花帖",
        "你所在之城的怪事!!!DS东方文花帖",
        "现代妖怪殖民地!!!DS东方文花帖",
        "复仇女神的要塞!!!DS东方文花帖",
        "无间之钟　～ Infinite Nightmare!!!DS东方文花帖",
        "曲名不详!!!DS东方文花帖",
        "神域的捉迷藏生活!!!东方三月精O1",
        "春之冰精!!!妖精大战争",
        "赌上性命去恶作剧!!!妖精大战争",
        "时刻旺盛的好奇心!!!妖精大战争",
        "午夜的妖精舞会!!!妖精大战争",
        "妖精大战争　～ Fairy Wars!!!妖精大战争",
        "Loose Rain!!!妖精大战争",
        "Magus Night!!!妖精大战争",
        "春之冰精　- 静 -!!!妖精大战争",
        "欲望深重的灵魂!!!东方神灵庙",
        "死灵的夜樱!!!东方神灵庙",
        "Ghost Lead!!!东方神灵庙",
        "欢迎来到妖怪寺!!!东方神灵庙",
        "门前的妖怪小姑娘!!!东方神灵庙",
        "在美妙的墓地里住下吧!!!东方神灵庙",
        "Rigid Paradise!!!东方神灵庙",
        "Desire Drive!!!东方神灵庙",
        "古老的元神!!!东方神灵庙",
        "梦殿大祀庙!!!东方神灵庙",
        "大神神话传!!!东方神灵庙",
        "小小欲望的星空!!!东方神灵庙",
        "圣德传说 ～ True Administrator!!!东方神灵庙",
        "后院的妖怪参拜道!!!东方神灵庙",
        "佐渡的二岩!!!东方神灵庙",
        "神社的新风!!!东方神灵庙",
        "Desire Dream!!!东方神灵庙",
        "卫星鸟船!!!鸟船遗迹",
        "特洛伊群的密林!!!鸟船遗迹",
        "天鸟船神社!!!鸟船遗迹",
        "天鸟船神社的结界!!!鸟船遗迹",
        "漂浮于宇宙之中的幻想乡!!!鸟船遗迹",
        "绿意盎然的疗养院!!!伊奘诺物质",
        "被牛引到善光寺!!!伊奘诺物质",
        "阿加尔塔之风!!!伊奘诺物质",
        "收集日本各地的不可思议!!!伊奘诺物质",
        "幻想乡的二岩!!!东方心绮楼",
        "亡失的情感!!!东方心绮楼",
        "尘世不变的悲观主义!!!东方心绮楼",
        "心绮楼囃子!!!东方心绮楼",
        "有人气的场所!!!东方心绮楼",
        "无人气的场所!!!东方心绮楼",
        "丑时三刻的村庄!!!东方心绮楼",
        "本日的头版头条!!!东方心绮楼",
        "晓云!!!东方心绮楼",
        "官板黄昏新闻!!!东方心绮楼",
        "尘世不变的悲观主义～心绮楼囃子!!!暗黑能乐集心绮楼",
        "出演者选择!!!暗黑能乐集心绮楼",
        "没用上的场所!!!暗黑能乐集心绮楼",
        "Lastword发动!!!暗黑能乐集心绮楼",
        "不可思议的驱魔棒!!!东方辉针城",
        "Mist Lake!!!东方辉针城",
        "秘境的人鱼!!!东方辉针城",
        "往来于运河的人与妖!!!东方辉针城",
        "柳树下的杜拉罕!!!东方辉针城",
        "满月的竹林!!!东方辉针城",
        "孤独的狼人!!!东方辉针城",
        "Magical Storm!!!东方辉针城",
        "幻想净琉璃!!!东方辉针城",
        "沉向空中的辉针城!!!东方辉针城",
        "Reverse Ideology!!!东方辉针城",
        "针小棒大的天守阁!!!东方辉针城",
        "辉光之针的小人族　～ Little Princess!!!东方辉针城",
        "魔力的雷云!!!东方辉针城",
        "原初的节拍　～ Pristine Beat!!!东方辉针城",
        "小槌的魔力!!!东方辉针城",
        "非常非常神奇的道具们!!!东方辉针城",
        "燃起犯规的狼烟!!!弹幕天邪鬼",
        "以犯规对不可能的弹幕!!!弹幕天邪鬼",
        "Midnight Spell Card!!!弹幕天邪鬼",
        "浪漫逃飞行!!!弹幕天邪鬼",
        "永远的三日天下!!!弹幕天邪鬼",
        "震撼心灵的都市传说!!!东方深秘录",
        "幻想乡不可思议发现!!!东方深秘录",
        "有珠子的日常!!!东方深秘录",
        "显现的传承形态!!!东方深秘录",
        "时代风潮的造访!!!东方深秘录",
        "价值不明!!!东方深秘录",
        "相信可能性!!!东方深秘录",
        "知晓真相之人!!!东方深秘录",
        "外界民俗!!!东方深秘录",
        "各自的结局!!!东方深秘录",
        "被揭晓的深秘!!!东方深秘录",
        "灵异任你选!!!东方深秘录",
        "七玉搜集大摊牌!!!东方深秘录",
        "公正的争夺!!!东方深秘录",
        "对跖点之钟!!!东方深秘录",
        "竹林大火!!!东方深秘录",
        "华狭间的战场!!!东方深秘录",
        "Last Occultism　～ 现世的秘术师!!!东方深秘录",
        "容貌错乱　～ Horrible Night!!!深秘乐曲集·补",
        "宇宙巫女现身!!!东方绀珠传",
        "忘不了，那曾依藉的绿意!!!东方绀珠传",
        "兔已着陆!!!东方绀珠传",
        "湖上倒映着洁净的月光!!!东方绀珠传",
        "九月的南瓜!!!东方绀珠传",
        "飞翔于宇宙的不可思议巫女!!!东方绀珠传",
        "永远的春梦!!!东方绀珠传",
        "冻结的永远之都!!!东方绀珠传",
        "逆转的命运之轮!!!东方绀珠传",
        "遥遥38万公里的航程!!!东方绀珠传",
        "星条旗的小丑!!!东方绀珠传",
        "故乡之星倒映之海!!!东方绀珠传",
        "Pure Furies　～ 心之所在!!!东方绀珠传",
        "前所未见的噩梦世界!!!东方绀珠传",
        "Pandemonic Planet!!!东方绀珠传",
        "从神社所见之月!!!东方绀珠传",
        "宇宙巫女归还!!!东方绀珠传",
        "二人不值一提的博物志!!!燕石博物志",
        "Dr. Latency的令人不眠之瞳!!!燕石博物志",
        "比普朗克更短的须臾!!!燕石博物志",
        "薛定谔的怪猫!!!燕石博物志",
        "禁忌的膜壁!!!燕石博物志",
        "Bar·Old Adam!!!旧约酒馆",
        "燕石博物志所带来的黑暗!!!旧约酒馆",
        "Outsider Cocktail!!!旧约酒馆",
        "旧世界的冒险酒馆!!!旧约酒馆",
        "宿醉者的同床异梦!!!旧约酒馆",
        "识文解意的爱书人!!!东方铃奈庵",
        "Mysterious Shrine!!!8BIT MUSIC POWER FINAL",
        "樱花舞落的天空!!!东方天空璋",
        "希望之星直升青霄!!!东方天空璋",
        "盛夏的妖精梦!!!东方天空璋",
        "无色之风吹拂妖怪之山!!!东方天空璋",
        "深山的遭遇!!!东方天空璋",
        "徜徉于樱色之海!!!东方天空璋",
        "一对的神兽!!!东方天空璋",
        "幻想的白色旅人!!!东方天空璋",
        "魔法的笠地藏!!!东方天空璋",
        "禁断之门对面，是此世还是彼世!!!东方天空璋",
        "Crazy Back Dancers!!!东方天空璋",
        "Into Backdoor!!!东方天空璋",
        "被秘匿的四个季节!!!东方天空璋",
        "门再也进不去了!!!东方天空璋",
        "秘神摩多罗　～ Hidden Star in All Seasons.!!!东方天空璋",
        "不自然的自然!!!东方天空璋",
        "白色旅人!!!东方天空璋",
        "Mushroom·Waltz!!!东方凭依华",
        "圣辇船飞向天空!!!东方凭依华",
        "法力之下的平等!!!东方凭依华",
        "恒常不变的参庙祀!!!东方凭依华",
        "光辉的天球仪!!!东方凭依华",
        "泽之河童的技术力!!!东方凭依华",
        "地底绽放的蔷薇!!!东方凭依华",
        "在深绿的狸森里!!!东方凭依华",
        "不灭的赤魂!!!东方凭依华",
        "落日映照下的逆城!!!东方凭依华",
        "克服万千之试炼!!!东方凭依华",
        "永远延续的回廊!!!东方凭依华",
        "Sleep Sheep·Parade!!!东方凭依华",
        "到达有顶天!!!东方凭依华",
        "凭坐处于梦与现实之间　～ Necro-Fantasia!!!东方凭依华",
        "今宵是飘逸的自我主义者(Live ver)　～ Egoistic Flowers.!!!东方凭依华",
        "凭投依合!!!东方凭依华",
        "连带责人!!!东方凭依华",
        "合缘奇缘!!!东方凭依华",
        "异心同体!!!东方凭依华",
        "豪言壮语!!!东方凭依华",
        "智略纵横!!!东方凭依华",
        "意气洋洋!!!东方凭依华",
        "开演间近!!!东方凭依华",
        "行云流水!!!东方凭依华",
        "异变的种子!!!东方凭依华",
        "疑惑的萌芽!!!东方凭依华",
        "含苞待放的凭依华!!!东方凭依华",
        "通往真相的枝叶!!!东方凭依华",
        "争奇斗艳的凭依华!!!东方凭依华",
        "飞舞飘散的凭依华吹雪!!!东方凭依华",
        "凭依华!!!完全凭依唱片名录",
        "噩梦日记!!!秘封噩梦日记",
        "Lucid Dreamer!!!秘封噩梦日记",
        "Lunatic Dreamer!!!秘封噩梦日记",
        "Nightmare Diary!!!秘封噩梦日记",
        "沉默的兽灵!!!东方鬼形兽",
        "只有地藏知晓的哀叹!!!东方鬼形兽",
        "Jelly Stone!!!东方鬼形兽",
        "Lost River!!!东方鬼形兽",
        "石之婴儿与水中牛!!!东方鬼形兽",
        "不朽的曼珠沙华!!!东方鬼形兽",
        "Seraphic Chicken!!!东方鬼形兽",
        "Unlocated Hell!!!东方鬼形兽",
        "Tortoise Dragon　～ 幸运与不幸!!!东方鬼形兽",
        "Beast Metropolis!!!东方鬼形兽",
        "陶瓷的杖刀人!!!东方鬼形兽",
        "Electric Heritage!!!东方鬼形兽",
        "寄世界于偶像　～ Idoratrize World!!!东方鬼形兽",
        "闪耀的弱肉强食法则!!!东方鬼形兽",
        "圣德太子的天马　～ Dark Pegasus!!!东方鬼形兽",
        "畜生们的休息!!!东方鬼形兽",
        "从地下的归还!!!东方鬼形兽",
        "独一无二的投影!!!INDIE Live Expo",
        "未知之花 魅知之旅!!!未知之花 魅知之旅",
        "架起虹桥的幻想乡!!!东方虹龙洞",
        "妖异们的骤雨!!!东方虹龙洞",
        "大吉猫咪!!!东方虹龙洞",
        "深绿掩映的断崖!!!东方虹龙洞",
        "Banditry Technology!!!东方虹龙洞",
        "驹草盛开的万年积雪!!!东方虹龙洞",
        "Smoking Dragon!!!东方虹龙洞",
        "日渐荒废的工业遗址!!!东方虹龙洞",
        "神代矿石!!!东方虹龙洞",
        "渴盼已久的逢魔之刻!!!东方虹龙洞",
        "天魔之山漫天星!!!东方虹龙洞",
        "Lunar Rainbow!!!东方虹龙洞",
        "熙攘市场今何在　～ Immemorial Marketeers!!!东方虹龙洞",
        "幻想地下大轨道网!!!东方虹龙洞",
        "灭杀龙王的公主!!!东方虹龙洞",
        "风暴过后的星期日!!!东方虹龙洞",
        "虹色的世界!!!东方虹龙洞",
        "日日是红日 ～ Folksy Touhou days!!!东方音焰火",
        "被水淹没的沉愁地狱!!!东方刚欲异闻",
        "东方刚欲异闻!!!东方刚欲异闻",
        "天理人欲!!!东方刚欲异闻",
        "大地之底，刚欲之海!!!东方刚欲异闻",
        "贪欲之兽的记忆!!!东方刚欲异闻",
        "全有机体的记忆　～ Memory of Fossil Energy.!!!东方刚欲异闻",
        "七石之狼、登天吞云!!!虹色的北斗七星",
        "收藏家的忧郁午后!!!弹幕狂们的黑市",
        "令人心跳的司空见惯幻想乡!!!弹幕狂们的黑市",
        "妖怪上钩!!!弹幕狂们的黑市",
        "黑市无处不在!!!弹幕狂们的黑市",
        "拿起弹幕吧，弹幕狂们!!!弹幕狂们的黑市",
        "第100次黑市!!!弹幕狂们的黑市",
        "兽之智慧!!!东方兽王园",
        "世间万物皆可爱!!!东方兽王园",
        "魔兽紧急升空!!!东方兽王园",
        "悠久山上鬼!!!东方兽王园",
        "Tiny Shangri-la!!!东方兽王园",
        "勇敢又慵懒的妖兽!!!东方兽王园",
        "吸血怪兽卓柏卡布拉!!!东方兽王园",
        "不回首的黄泉路!!!东方兽王园",
        "越轨者们的无碍光　～ Kingdam of Nothingness.!!!东方兽王园",
        "兽王们的小憩!!!东方兽王园",
        "野兽有智慧吗!!!东方兽王园",
        "日出七夕坂!!!七夕坂梦幻能",
        "不等式的廷克·贝尔!!!七夕坂梦幻能",
        "梦幻能 ～ Taboo Marionette!!!七夕坂梦幻能",
        "独自一人的常陆行路!!!七夕坂梦幻能",
        "毕竟就算不是夜晚也有鬼怪!!!七夕坂梦幻能",
        "向着无现之里 ～ chain link？!!!（连缘）连缘无现里",
        "Exotic Crystal!!!（连缘）连缘无现里",
        "Melanin in Black ～ Colt Snake!!!（连缘）连缘无现里",
        "摇动铁锚的趋势 ～ Lost Anchorage!!!（连缘）连缘无现里",
        "搅拌刚体金刚 ～ Let's Joke！!!!（连缘）连缘无现里",
        "Extra Mind!!!（连缘）连缘无现里",
        "In this World ～ Monochrome_eye!!!（连缘）连缘无现里",
        "超越时空之翼 ～ M_theory!!!（连缘）连缘无现里",
        "解冻冰结之心!!!（连缘）连缘无现里",
        "Air Master！ ～ Soul_Dualism!!!（连缘）连缘无现里",
        "将虚实表里反转!!!（连缘）连缘无现里",
        "End of History!!!（连缘）连缘无现里",
        "掌上之星!!!（连缘）连缘无现里",
        "Empire Power ～ Eternal_Force!!!（连缘）连缘无现里",
        "凤雏天奔!!!（连缘）连缘无现里",
        "腐朽呛人的记忆　～ BEGAN!!!（连缘）连缘无现里",
        "永劫舞蹈机关 ～ Shall_We_Dance！！!!!（连缘）连缘无现里",
        "蓊翳逝去的现实 ～ human_VISION!!!（连缘）连缘无现里",
        "散落亡佚的梦幻 ～ bard_VISION!!!（连缘）连缘无现里",
        "妖异亦忌之憩 ～ Need umbrella！!!!（连缘）连缘蛇从剑",
        "无损之功德 ～ go_to_work...？!!!（连缘）连缘蛇从剑",
        "唐红色之长靴!!!（连缘）连缘蛇从剑",
        "Deep Waver!!!（连缘）连缘蛇从剑",
        "粘稠的海索面 ～ Act the FOOL HAHAHA!!!（连缘）连缘蛇从剑",
        "若迷失归路 ～ KEEP IN!!!（连缘）连缘蛇从剑",
        "Prismic Accelerator!!!（连缘）连缘蛇从剑",
        "舐糠及米 ～ What&#39;s the matter？!!!（连缘）连缘蛇从剑",
        "一文不值的聚财者!!!（连缘）连缘蛇从剑",
        "Radical History ～ 穿过双眸的记忆!!!（连缘）连缘蛇从剑",
        "埴轮相扑 ～ End of immolation!!!（连缘）连缘蛇从剑",
        "将悠久之沉眠，共刹那之瞬间!!!（连缘）连缘蛇从剑",
        "在深邃徒云之下 ～ Mow Down！!!!（连缘）连缘蛇从剑",
        "独一无二的辉煌!!!（连缘）连缘蛇从剑",
        "湮没瞬间的雪中胜景!!!（连缘）连缘蛇从剑",
        "Mono Eye ～ Ironic FATE!!!（连缘）连缘蛇从剑",
        "破阵勇者们的闹剧 ～ Routine eventually!!!（连缘）连缘蛇从剑",
        "井绳嘶鸣通小路 ～ Stream VISION!!!（连缘）连缘蛇从剑",
        "薄冰上的花 ～ Snowy country!!!（连缘）连缘灵烈传",
        "过于奢靡的平稳 ～ be motivated!!!（连缘）连缘灵烈传",
        "Mukuro Mancer ～ 空壳之梦!!!（连缘）连缘灵烈传",
        "阴翳冷气，沉寂灵气!!!（连缘）连缘灵烈传",
        "Get Ready Go ～ to run around！!!!（连缘）连缘灵烈传",
        "Falling Ghost!!!（连缘）连缘灵烈传",
        "幽暗海峡启航!!!（连缘）连缘灵烈传",
        "各自的期许和足迹!!!（连缘）连缘灵烈传",
        "空五倍子色的专注!!!（连缘）连缘灵烈传",
        "Keep the History!!!（连缘）连缘灵烈传",
        "Double Keeper ～ 玄皓双生鸟!!!（连缘）连缘灵烈传",
        "梦中遗忘的神圣萤火!!!（连缘）连缘灵烈传",
        "Moon Child ～ Homunculus Dream!!!（连缘）连缘灵烈传",
        "After All!!!（连缘）连缘灵烈传",
        "被缘排斥的名字!!!（连缘）连缘灵烈传",
        "开拓天空的迷失道!!!（连缘）连缘灵烈传",
        "显界Overhaul!!!（连缘）连缘灵烈传",
        "潜藏于无现之物!!!（连缘）连缘灵烈传",
        "徒然终结的行路!!!（连缘）连缘灵烈传",
        "蜉蝣之城 ～ Phantom VISION!!!（连缘）连缘灵烈传",
        "被掀开的枪火罩 ～ Slow Starter!!!（连缘）连缘天影战记",
        "前进吧人之道，妖之道!!!（连缘）连缘天影战记",
        "妖之病垂 ～ Outbreak!!!（连缘）连缘天影战记",
        "海峡今天也是风平浪静!!!（连缘）连缘天影战记",
        "军靴声的幻听!!!（连缘）连缘天影战记",
        "浓雾吞没的昼梦!!!（连缘）连缘天影战记",
        "生长着的石墙，移动着的城郭!!!（连缘）连缘天影战记",
        "铁与油的理想乡!!!（连缘）连缘天影战记",
        "阳炎之城 ～ Phantom ROAD!!!（连缘）连缘天影战记",
        "天之顶 ～ Brilliant Pagoda!!!（连缘）连缘天影战记",
        "影之顶 ～ Haze Castle!!!（连缘）连缘天影战记",
        "焚烧小鸟们的黑羽!!!（连缘）连缘天影战记",
        "必须前进的道路!!!（连缘）连缘天影战记",
        "花儿啊，凋零吧 ～ Consumed with JEALOUSY！!!!（连缘）连缘天影战记",
        "Nightmare Syndrome!!!（连缘）连缘天影战记",
        "Made in Black ～ Double Snake!!!（连缘）连缘天影战记",
        "雨水染上唐红色 ～DYE～!!!（连缘）连缘天影战记",
        "Emperor Road ～ So cute！!!!（连缘）连缘天影战记",
        "Slug Agent!!!（连缘）连缘天影战记",
        "青岚 ～ Plastic Vortex!!!（连缘）连缘天影战记",
        "Act the Fool ～ HAHAHA!!!（连缘）连缘天影战记",
        "Let's 'n Go ～ to run away！!!!（连缘）连缘天影战记",
        "Killing Superior ～ Giant killing!!!（连缘）连缘天影战记",
        "醉狂之舞 ～ This drunkard！!!!（连缘）连缘天影战记",
        "Prismic Drive!!!（连缘）连缘天影战记",
        "Necro Master!!!（连缘）连缘天影战记",
        "幽灵波动风 ～ not steady body!!!（连缘）连缘天影战记",
        "绚烂至死 ～ Toxic gem!!!（连缘）连缘天影战记",
        "英雄参见！！!!!（连缘）连缘天影战记",
        "Overheat Down!!!（连缘）连缘天影战记",
        "見切り千両、目玉万両 ～ MONEY GAME!!!（连缘）连缘天影战记",
        "张开的眼瞳 ～ Mono EYE!!!（连缘）连缘天影战记",
        "SILVER!!!（连缘）连缘天影战记",
        "Microcosm ～ Homunculus nightmare!!!（连缘）连缘天影战记",
        "Loom the History!!!（连缘）连缘天影战记",
        "Super Haniwa ～ NEXT FORM！！!!!（连缘）连缘天影战记",
        "触不到月亮的手 ～ Moon Child!!!（连缘）连缘天影战记",
        "大天 ～ Spirit of nagara!!!（连缘）连缘天影战记",
        "亡月之王!!!（连缘）连缘天影战记",
        "常轨外的飞翔之羽 ～ Eleven-dimensional!!!（连缘）连缘天影战记",
        "Another Mind!!!（连缘）连缘天影战记",
        "Eternal Power ～ BIG STAR!!!（连缘）连缘天影战记",
        "被斩断的血缘!!!（连缘）连缘天影战记",
        "百命独缘 ～ Chimera soul!!!（连缘）连缘天影战记",
        "决坏Overflow!!!（连缘）连缘天影战记",
        "MO-NA-D-2 ～ 记忆伪笔!!!（连缘）连缘天影战记",
        "Lost Jackpot!!!（连缘）连缘天影战记",
        "震道之真钢 ～ Cyclops Tech!!!（连缘）连缘天影战记",
        "永劫模仿演舞 ～ Spell Collector!!!（连缘）连缘天影战记",
        "崩坏的永劫舞蹈机关!!!（连缘）连缘天影战记",
        "MO-NA-D-1 ～ 记忆追踪!!!（连缘）连缘天影战记",
        "天影战记 ～ Playing soldiers!!!（连缘）连缘天影战记",
        "祭典之后的沉默!!!（连缘）连缘天影战记",
        "三种道路，三种未来 ～ Triple VISION!!!（连缘）连缘天影战记"
    };

    private readonly GroupMessageEventArgs _groupMessage;

    private TouhouService(GroupMessageEventArgs groupMessage)
    {
        _groupMessage = groupMessage;
    }

    [Shortcut("Touhou OST Recognise", "东方原曲认知")]
    public static async Task TouhouOstRecog(GroupMessageEventArgs groupMessage)
    {
        var touhou = new TouhouService(groupMessage);
        await touhou.TouhouOstRecog(string.Empty);
    }

    [Shortcut("Random Touhou OST", "随机东方原曲")]
    public static async Task RandomTouhouOst(GroupMessageEventArgs groupMessage)
    {
        var touhou = new TouhouService(groupMessage);
        await touhou.RandomTouhouOst();
    }

    public static async Task TouhouServiceInit(GroupMessageEventArgs groupMessage)
    {
        var handler = new CommandHandler(groupMessage, typeof(TouhouService));
        await handler.ExecAsync();
    }

    public async Task Execute(GroupMessageEventArgs groupMessage)
    {
        var handler = new CommandHandler(groupMessage, typeof(TouhouService));
        await handler.ExecAsync();
    }
    
    [Service("Random Touhou OST", prompt: "randomost")]
    private async Task RandomTouhouOst()
    {
        Log.Information("RandomTouhouOst: Command from {0} in {1}", _groupMessage.UserId, _groupMessage.GroupId);
        var rnd = new Random();
        var ost = OstKey[rnd.Next(0, OstKey.Count - 1)];
        var tmp = Environment.CurrentDirectory;
        var uri = @$"file:///{tmp}\\audio\\{ost}.mp3";
        await SendAudioFile(_groupMessage.GroupId, uri);
        Log.Information("RandomTouhouOst: Sent audio file to {0}", _groupMessage.GroupId);
        await _groupMessage.ReplyAsync(new TextSegment(TextFormat(ost)));
    }

    private async Task TouhouOstRecogCheck(string answer)
    {
        int option;
        switch (answer.ToUpper())
        {
            case "A":
                option = 0;
                break;
            case "B":
                option = 1;
                break;
            case "C":
                option = 2;
                break;
            case "D":
                option = 3;
                break;
            default: return;
        }

        if (option == Program.TouhouOst[_groupMessage.Sender.UserId])
            await _groupMessage.ReplyAsync(new TextSegment("回答正确"));
        else
            await _groupMessage.ReplyAsync(new TextSegment("回答错误"));
    }

    private static string TextFormat(string text)
    {
        return $"{text.Split("!!!")[0]}({text.Split("!!!")[1]})";
    }

    private static async Task SendAudioFile(long groupId, string uri)
    {
        var client = new RestClient("http://127.0.0.1:3000/send_group_msg");
        var request = new RestRequest();
        request.AddHeader("Content-Type", "application/json");
        request.Method = Method.Post;
        var sendReq = new SendMessageModel.Rootobject
        {
            group_id = groupId,
            message = new SendMessageModel.Message
            {
                type = "record",
                data = new SendMessageModel.Data
                {
                    file = uri
                }
            }
        };
        request.AddBody(JsonConvert.SerializeObject(sendReq));
        await client.ExecuteAsync(request);
    }

    [Service("Touhou OST Recognise", prompt: "ostrecognise")]
    private async Task TouhouOstRecog(string arg)
    {
        if (arg != string.Empty)
        {
            await TouhouOstRecogCheck(arg);
            return;
        }

        if (Program.TouhouOst.ContainsKey(_groupMessage.Sender.UserId)) return;
        Log.Information("TouhouOstRecognise: Command from {0} in {1}", _groupMessage.UserId, _groupMessage.GroupId);
        var options = RandomR.GenerateUniqueRandomNumbers(0, OstKey.Count - 1, 4);
        var result = OstKey[options[0]];
        var filename = Guid.NewGuid() + ".mp3";
        await AudioCutter.RandomClipFromAudioAsync(result, filename);
        Log.Information("TouhouOstRecognise: Clipped audio {0}", filename);
        var tmp = Environment.CurrentDirectory;
        var uri = @$"file:///{tmp}\\{filename}";
        await SendAudioFile(_groupMessage.GroupId, uri);
        Log.Information("TouhouOstRecognise: Sent audio file to {0}", _groupMessage.GroupId);
        var randomOptions = RandomR.GenerateUniqueRandomNumbers(0, 3, 4);
        var trueSelection = 0;
        while (randomOptions[trueSelection] != 0) trueSelection++;
        await _groupMessage.ReplyAsync(new TextSegment(
            $"搶曽原曲认知\n包含正作、官方出版物以及西方和连缘的OST\n使用/th ostrecognise <answer>回复答案\nA.{TextFormat(OstKey[options[randomOptions[0]]])}\nB.{TextFormat(OstKey[options[randomOptions[1]]])}\nC.{TextFormat(OstKey[options[randomOptions[2]]])}\nD.{TextFormat(OstKey[options[randomOptions[3]]])}\n"));
        Program.TouhouOst.Add(_groupMessage.Sender.UserId, (short)trueSelection);
        await Task.Run(async () =>
        {
            await Task.Delay(45000);
            Program.TouhouOst.Remove(_groupMessage.Sender.UserId);
        });
        File.Delete(filename);
    }
}