using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newbe.Mahua;
using Newtonsoft.Json;
using System.Data.SQLite;
using System.Text.RegularExpressions;


namespace TRKS.WF.QQBot
{
    class WFGraceMange
    {
        public GraceGroupMangerConfig mangerConfig = new GraceGroupMangerConfig(); 
        private enum OperationConfig{
            IsBanPostLotto,
            Alert,
            JoinRequestMember
        }
        private Dict[] dicts = GetDictFromWFA();

        public GroupMemberInfo GetGroupMemberInfo(IMahuaApi _mahuaApi, string group, string qq)
        {
            GroupMemberInfo info = new GroupMemberInfo();
            int index = QQisGroupMember(_mahuaApi, group, qq);
            if ( index < 0) 
            {
                info.Authority = GroupMemberAuthority.Unknown;
                return info;
            }
            var members = _mahuaApi.GetGroupMemebersWithModel(group).Model.ToList();
            return members[index];
        }
        public int QQisGroupMember(IMahuaApi _mahuaApi, string group, string qq)
        {
            GroupMemberInfo info = new GroupMemberInfo();
            List<GroupMemberInfo> members;
            try
            {
                members = _mahuaApi.GetGroupMemebersWithModel(group).Model.ToList();
            }
            catch
            {
                return -1;
            }
            int count = members.Count;
            for (int i = 0;i < count;i++)
            {
                if (members[i].Qq == qq)
                {
                    return i;
                }
            }
            return -1; 
        }
        private static Dict[] GetDictFromWFA()
        {
            var dicts = WebHelper.DownloadJson<Dict[]>(
                        "https://raw.githubusercontent.com/Richasy/WFA_Lexicon/master/WF_Dict.json");
            return dicts;
        }
        public void SendTranslateResult(string group, string str)
        {
            string msg = "";
            if (str == "")
            {
                msg = "你是在为难我胖虎?!";
                Messenger.SendGroup(group, msg);
                return;
            }
            str = str.ToLower();
            int count = 0;
            msg = $"以下是物品{str}的翻译结果\n\n";
            foreach(var dict in dicts)
            {
                if(dict.Zh.ToLower().IndexOf(str) >= 0)
                {
                    msg += dict.Zh + "  |==|  " + dict.En + "\n";
                    count++;
                }
                if (dict.En.ToLower().IndexOf(str) >= 0)
                {
                    msg += dict.En + "  |==|  " + dict.Zh + "\n";
                    count++;
                }
                if (count == 10)
                {
                    msg += "\n\n若未查找到您需要的物品，请确认后再翻译";
                }
            }
            if (count == 0)
                msg = $"不存在物品[{str}],请确认物品名再翻译";
            Messenger.SendGroup(group, msg);
        }
        public void BanPostLotto(IMahuaApi _mahuaApi, string group, string qq)
        {
            mangerConfig = GraceGroupMangerConfig.Instance;
            int ret = QQinMangerConfig(group, mangerConfig);
            if(ret < 0)
            {
                GraceGroupConfig cfg = new GraceGroupConfig();
                cfg.GroupQQ = group;
                mangerConfig.groupConfigs.Add(cfg);
                mangerConfig.ToJsonString().SaveToFile(GraceGroupMangerConfig.SavePath);
            }
            else
            {
                if(!GraceGroupMangerConfig.Instance.groupConfigs[ret].IsBanPostLotto)
                {
                    Messenger.SendGroup(group, "禁言抽奖功能已被禁用，请输入[开启禁言抽奖]来启用此功能(需要管理员或群主权限)");
                    return;
                }
            }
            GroupMemberInfo botInfo = GetGroupMemberInfo(_mahuaApi, group, _mahuaApi.GetLoginQq());
            GroupMemberAuthority botAuthority = botInfo.Authority;
            if (GroupMemberAuthority.Unknown == botAuthority || GroupMemberAuthority.Normal == botAuthority)
            {
                Messenger.SendGroup(group, "机器人的权限不足，想要使用禁言抽奖功能，请将机器人设置为管理员!");
                return;
            }

            int hours = 0, minutes = 0, seconds = 0;
            Random rd = new Random();
            minutes = rd.Next(1, 11);
            TimeSpan time = new TimeSpan(hours, minutes, seconds);

            GroupMemberInfo member = GetGroupMemberInfo(_mahuaApi, group, qq);
            if (member.Authority < botAuthority)
            {
                _mahuaApi.BanGroupMember(group, qq, time);
                string nick = member.InGroupName;
                if (nick == "")
                    nick = qq;

                Messenger.SendGroup(group, $"恭喜 {nick} 抽中{minutes}分钟套餐");

            }
            else
            {
                Messenger.SendGroup(group, $"[CQ:at,qq={qq}] ,权限不足，抽个鸡儿");
            }
        }
        private int QQinMangerConfig(string groupQQ, GraceGroupMangerConfig mangerCfg)
        {
            int count = mangerCfg.groupConfigs.Count;
            for (int i = 0; i < count; i++) 
            {
                if (mangerCfg.groupConfigs[i].GroupQQ == groupQQ)
                    return i;
            }
            return -1;
        }
        private void SetOperationToGroupMangerConfig(string groupQQ, bool status, OperationConfig enumCfg)
        {
            mangerConfig = GraceGroupMangerConfig.Instance;
            int index = QQinMangerConfig(groupQQ, mangerConfig);
            if (index < 0)
            {
                GraceGroupConfig cfg = new GraceGroupConfig();
                cfg.GroupQQ = groupQQ;
                switch(enumCfg)
                {
                    case OperationConfig.Alert:
                        cfg.Alert = status;break;
                    case OperationConfig.IsBanPostLotto:
                        cfg.IsBanPostLotto = status;break;
                    case OperationConfig.JoinRequestMember:
                        cfg.JoinRequestMember = status;break;
                    default:
                        return;
                }
                mangerConfig.groupConfigs.Add(cfg);
            }
            else
            {
                switch (enumCfg)
                {
                    case OperationConfig.Alert:
                        mangerConfig.groupConfigs[index].Alert = status; break;
                    case OperationConfig.IsBanPostLotto:
                        mangerConfig.groupConfigs[index].IsBanPostLotto = status; break;
                    case OperationConfig.JoinRequestMember:
                        mangerConfig.groupConfigs[index].JoinRequestMember = status; break;
                    default:
                        return;
                } 
            }
            mangerConfig.ToJsonString().SaveToFile(GraceGroupMangerConfig.SavePath);
            return;
        }
        public void SetIsBanPostLottoToGroupMangerConfig(string groupQQ, bool status)
        {
            SetOperationToGroupMangerConfig(groupQQ, status, OperationConfig.IsBanPostLotto);
        }
        public void SetAlertToGroupMangerConfig(string groupQQ, bool status)
        {
            SetOperationToGroupMangerConfig(groupQQ, status, OperationConfig.Alert);
        }
        public void SetJoinRequestMemberToGroupMangerConfig(string groupQQ, bool status)
        {
            SetOperationToGroupMangerConfig(groupQQ, status, OperationConfig.JoinRequestMember);
        }
        public void AddAutoMessageToGroupMangerConfig(string groupQQ, string message)
        {
            mangerConfig = GraceGroupMangerConfig.Instance;
            int index = QQinMangerConfig(groupQQ, mangerConfig);
            if(index < 0)
            {
                GraceGroupConfig cfg = new GraceGroupConfig();
                cfg.GroupQQ = groupQQ;
                message = message.Trim();
                string[] strs = Regex.Split(message, "==");
                if (2 == strs.Count())
                {
                    strs[0] = strs[0].Trim();
                    strs[1] = strs[1].Trim();
                    message = strs[0] + "==" + strs[1];
                    cfg.AutoMessage.Add(message);
                    mangerConfig.groupConfigs.Add(cfg);
                    mangerConfig.ToJsonString().SaveToFile(GraceGroupMangerConfig.SavePath);
                    Messenger.SendGroup(groupQQ, "添加成功");
                }
                else
                {
                    mangerConfig.groupConfigs.Add(cfg);
                    mangerConfig.ToJsonString().SaveToFile(GraceGroupMangerConfig.SavePath);
                    Messenger.SendGroup(groupQQ, "指令格式有误，请参照示例：[自定义回复 hello==world]");
                }
            }
            else
            {
                message = message.Trim();
                string[] strs = Regex.Split(message, "==");
                if (2 == strs.Count())
                {
                    strs[0] = strs[0].Trim();
                    foreach(var str in mangerConfig.groupConfigs[index].AutoMessage)
                    {
                        if (strs[0] == Regex.Split(str, "==")[0])
                        {
                            Messenger.SendGroup(groupQQ, "自定义回复已存在");
                            return;
                        }
                    }
                    strs[1] = strs[1].Trim();
                    message = strs[0] + "==" + strs[1];
                    mangerConfig.groupConfigs[index].AutoMessage.Add(message);
                    mangerConfig.ToJsonString().SaveToFile(GraceGroupMangerConfig.SavePath);
                    Messenger.SendGroup(groupQQ, "添加成功");
                }
                else
                {
                    Messenger.SendGroup(groupQQ, "指令格式有误，请参照示例：[自定义回复 hello==world]");
                }
            }
        }
    }
}
