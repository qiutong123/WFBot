﻿using Newbe.Mahua;
using Newbe.Mahua.MahuaEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Humanizer;
using Settings;
using System.Text.RegularExpressions;

namespace TRKS.WF.QQBot.MahuaEvents
{
    /// <summary>
    /// 群消息接收事件
    /// </summary>
    public class GroupMessageReceivedMahuaEvent1
        : IGroupMessageReceivedMahuaEvent
    {
        private readonly IMahuaApi _mahuaApi;
        internal static readonly WFNotificationHandler _wFAlert = new WFNotificationHandler();
        private static readonly WFStatus _wFStatus = new WFStatus();
        private static readonly WMSearcher _wmSearcher = new WMSearcher();
        private static readonly RMSearcher _rmSearcher = new RMSearcher();
        private static readonly WFGraceMange _wfGraceManage = new WFGraceMange();

        public GroupMessageReceivedMahuaEvent1(
            IMahuaApi mahuaApi)
        {
            _mahuaApi = mahuaApi;
        }

        public void ProcessGroupMessage(GroupMessageReceivedContext context)
        {
            if (Messenger.GroupCallDic.ContainsKey(context.FromGroup))
            {
                if (Messenger.GroupCallDic[context.FromGroup] > Config.Instance.CallperMinute && Config.Instance.CallperMinute != 0) return;
            }
            else
            {
                Messenger.GroupCallDic[context.FromGroup] = 0;
            }

            if (HotUpdateInfo.PreviousVersion) return;

            try
            {
                var message = context.Message;
                if (message.StartsWith("/") || !Config.Instance.IsSlashRequired)
                {
                    var command = "";
                    if (message.Contains("/"))
                    {
                        command = message.Substring(1).ToLower();
                    }
                    else
                    {
                        command = message;
                        command = command.Trim();
                    }
                    var syndicates = new [] {"赏金", "平原赏金", "地球赏金", "金星赏金", "金星平原赏金", "地球平原赏金"};
                    var fissures = new [] {"裂隙", "裂缝", "虚空裂隙", "查询裂缝", "查询裂隙"};
                    if (syndicates.Any(ostron => command.StartsWith(ostron)))
                    {
                        Messenger.SendGroup(context.FromGroup, "赏金查询已改版，请使用 /金星赏金 或者 /地球赏金.");
                        /*var indexString = command.Substring(syndicates.First(ostron => command.StartsWith(ostron)).Length);
                        if (indexString.IsNumber())
                        {
                            var index = int.Parse(indexString);
                            if (index <= 5 && 0 < index)
                            {
                                _wFStatus.SendSyndicateMissions(context.FromGroup, index);
                            }
                            else
                            {
                                _wFStatus.SendSyndicateMissions(context.FromGroup, 1);
                            }

                        }
                        else
                        {
                            Messenger.SendGroup(context.FromGroup, "需要参数, e.g. /赏金 4");
                        }*/
                    }

                    if (fissures.Any(fissure => command.StartsWith(fissure)))
                    {
                        Messenger.SendGroup(context.FromGroup, "裂隙查询已经改版，请直接使用 /裂隙.");
                        /*var words = command.Split(' ').ToList();
                        if (words.Count >= 2)
                        {
                            words.RemoveAt(0);
                            _wFStatus.SendFissures(context.FromGroup, words);
                        }
                        else
                        {
                            Messenger.SendGroup(context.FromGroup, $"需要参数, e.g. /裂隙 古纪");
                        }*/

                    }
                    if (command.StartsWith("查询"))
                    {
                        if (!command.Contains("裂隙") || !command.Contains("裂缝"))
                        {
                            if (command.Length > 3)
                            {
                                var item = command.Substring(3).Format();
                                _wmSearcher.SendWMInfo(item, context.FromGroup);
                            }
                            else
                            {
                                Messenger.SendGroup(context.FromGroup, "你没输入要查询的物品.");
                            }

                        }

                    }
                    

                    if (command.StartsWith("紫卡"))
                    {
                        if (command.Length >= 3)
                        {
                            if (command.Substring(2).StartsWith(" "))
                            {
                                var weapon = command.Substring(3).Format();
                                _rmSearcher.SendRiveninfos(context.FromGroup, weapon);
                            }

                        }
                        else
                        {
                            Messenger.SendGroup(context.FromGroup, "你没输入要查询的武器.");
                        }
                    }
                    if (command.StartsWith("翻译"))
                    {
                        string[] strs = Regex.Split(command, "翻译");
                        if(strs.Count() == 2)
                        {
                            strs[1] = strs[1].Trim();
                            _wfGraceManage.SendTranslateResult(context.FromGroup, strs[1]);
                        }
                        else
                        {
                            Messenger.SendGroup(context.FromGroup, "格式错误，示例:[翻译致残突击]");
                        }
                    }
                    if (command.StartsWith("遗物"))
                    {
                        if (command.Length >= 3)
                        {
                            if (command.Substring(2).StartsWith(" "))
                            {
                                var word = command.Substring(3).Format();
                                _wFStatus.SendRelicInfo(context.FromGroup, word);
                            }
                        }
                        else
                        {
                            Messenger.SendGroup(context.FromGroup, "请在后面输入关键词.");
                        }
                    }
                    if(command.Contains("大小姐"))
                    {
                        Messenger.SendGroup(context.FromGroup, "大小姐是我的！");
                    }
                    switch (command)
                    {
                        case "警报":
                            _wFAlert.SendAllAlerts(context.FromGroup);
                            break;
                        case "平野":
                        case "夜灵平野":
                        case "平原":
                        case "夜灵平原":
                        case "金星平原":
                        case "奥布山谷":
                        case "金星平原温度":
                        case "平原温度":
                        case "平原时间":
                            _wFStatus.SendCycles(context.FromGroup);
                            break;
                        case "入侵":
                            _wFAlert.SendAllInvasions(context.FromGroup);
                            break;
                        case "突击":
                            _wFStatus.SendSortie(context.FromGroup);
                            break;
                        case "奸商":
                        case "虚空商人":
                        case "商人":
                            _wFStatus.SendVoidTrader(context.FromGroup);
                            break;
                        case "活动":
                        case "事件":
                            _wFStatus.SendEvent(context.FromGroup);
                            break;
                        case "金星赏金":
                        case "福尔图娜赏金":
                        case "金星平原赏金":
                            _wFStatus.SendFortunaMissions(context.FromGroup);
                            break;
                        case "地球赏金":
                        case "希图斯赏金":
                        case "地球平原赏金":
                            _wFStatus.SendCetusMissions(context.FromGroup);
                            break;
                        case "裂隙":
                        case "裂缝":
                            _wFStatus.SendFissures(context.FromGroup);
                            break;
                        case "help":
                        case "帮助":
                        case "功能":
                        case "救命":
                            Messenger.SendHelpdoc(context.FromGroup);
                            break;
                        case "禁言抽奖":
                            _wfGraceManage.BanPostLotto(_mahuaApi, context.FromGroup, context.FromQq);
                            break;
                    }
                }


            }
            catch (Exception e)
            {
                Messenger.SendDebugInfo(e.ToString());
            }

        }
    }
}
