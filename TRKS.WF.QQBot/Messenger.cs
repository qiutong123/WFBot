﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newbe.Mahua;
using Settings;

namespace TRKS.WF.QQBot
{
    public static class Messenger
    {
        public static Dictionary<string, int> GroupCallDic = new Dictionary<string, int>();

        public static void IncreaseCallCounts(string group)
        {
            if (GroupCallDic.ContainsKey(group))
            {
                GroupCallDic[group]++;
            }
            else
            {
                GroupCallDic[group] = 1;
            }
            Task.Delay(TimeSpan.FromSeconds(60)).ContinueWith(task => GroupCallDic[group] = 0);

        }
        public static void SendDebugInfo(string content)
        {
            if (content.StartsWith("System.Threading.ThreadAbortException")) return;
            
            if (Config.Instance.QQ.IsNumber())
                SendPrivate(Config.Instance.QQ, content);
        }

        public static void SendPrivate(string qq, string content)
        {
            using (var robotSession = MahuaRobotManager.Instance.CreateSession())
            {
                var api = robotSession.MahuaApi;
                api.SendPrivateMessage(qq, content);
            }
        }

        private static Dictionary<string, string> previousMessageDic = new Dictionary<string, string>();
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void SendGroup(string qq, string content)
        {
            if (previousMessageDic.ContainsKey(qq) && content == previousMessageDic[qq]) return;

            previousMessageDic[qq] = content;

            using (var robotSession = MahuaRobotManager.Instance.CreateSession())
            {
                var api = robotSession.MahuaApi;
                api.SendGroupMessage(qq, content);
            }
            Thread.Sleep(1000); //我真的很生气 为什么傻逼tencent服务器就不能让我好好地发通知 NMSL
        }

        public static void Broadcast(string content)
        {
            Task.Factory.StartNew(() =>
            {
                var count = 1;
                foreach (var group in Config.Instance.WFGroupList)
                {
                    SendGroup(@group, content + Environment.NewLine + ($"发送次序: {count++}"));
                    Thread.Sleep(6000); //我真的很生气 为什么傻逼tencent服务器就不能让我好好地发通知 NMSL
                }
            }, TaskCreationOptions.LongRunning);
        }
        public static void SendHelpdoc(string group)
        {
            SendGroup(@group, @"欢迎查看机器人唯一指定帮助文档
宣传贴地址:https://warframe.love/thread-230.htm
开源地址:https://github.com/TheRealKamisama/WFBot (具体的使用方法请见此链接.)
本机器人是我参考开源地址的机器人做的二次开发，新增了一些功能，
如果有胖友想要布置自己的机器人，请参考开源地址或者找我，我的QQ1821554528.");
            if (File.Exists("data/image/帮助文档.png"))
            {
                SendGroup(@group, @"[CQ:image,file=\帮助文档.png]");
            }
            else
            {
                SendGroup(@group, @"欢迎查看破机器人的帮助文档,如有任何bug和崩溃请多多谅解.
作者:TheRealKamisama 二次开发者:黑商
    警报: 可使用 /警报 来查询当前的所有警报.
        新警报也会自动发送到启用了通知功能的群.
    入侵: 可使用 /入侵 来查询当前的所有入侵.
        新入侵也会自动发送到启用了通知功能的群.
    突击: 可使用 /突击 来查询当前的所有突击.
        突击的奖励池为一般奖励池.
    平原时间: 可使用 /平原 来查询 地球平原 现在的时间 和 奥布山谷 (金星平原) 现在的温度.
    活动:可使用 /活动 来查看目前的所有活动
    虚空商人信息: 可使用 /虚空商人 (或奸商) 来查询奸商的状态.
        如果虚空商人已经抵达将会输出所有的商品和价格, 长度较长.
    WarframeMarket 可使用 /查询 [物品名称]
        物品名不区分大小写, 无需空格
        物品名必须标准
        查询一个物品需要后面加一套
        查询 prime 版物品必须加 prime 后缀
        prime 不可以缩写成 p
        查询未开紫卡请输入: 手枪未开紫卡
    紫卡市场: 可使用 /紫卡 [武器名称]
        数据来自 WFA 紫卡市场
    地球赏金: 可使用 /地球赏金 来查询地球平原的全部赏金任务.
        目前不需要输入数字了.
    金星赏金: 可使用 /金星赏金 来查询金星平原的全部赏金任务.
        目前不需要输入数字了.
    裂隙: 可使用 /裂隙 来查询全部裂隙.
        目前不需要输入任何关键词了.
    遗物: 可使用 /遗物 [关键词] (eg. 后纪 s3, 前纪 B3) 来查询所有与关键词有关的遗物.
	启用群通知:[添加群{空格}<口令>{空格}<群号>],如[添加群 ******* 12345678]
    禁用群通知:[删除群{空格}<口令>{空格}<群号>],如[禁用群 ******* 12345678]
");
            }
        }

        /* 当麻理解不了下面的代码
        public static void SendToGroup(this string content, string qq)
        {
            using (var robotSession = MahuaRobotManager.Instance.CreateSession())
            {
                var api = robotSession.MahuaApi;
                api.SendGroupMessage(qq, content);
            }
        }

        public static void SendToPrivate(this string content, string qq)
        {
            using (var robotSession = MahuaRobotManager.Instance.CreateSession())
            {
                var api = robotSession.MahuaApi;
                api.SendPrivateMessage(qq, content);
            }
        }
        */
    }
}
