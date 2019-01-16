using System;
using System.Collections.Generic;

namespace TRKS.WF.QQBot
{
    [Configuration("WFConfig")]
    internal class Config : Configuration<Config>
    {
        public List<string> WFGroupList = new List<string>();

        public List<string> InvationRewardList = new List<string>();

        public string Code = "*******";

        public string QQ;

        public bool AcceptInvitation;

        public bool AcceptJoiningRequest;

        public string ClientId;

        public string ClientSecret;

        public string AcessToken;

        public DateTime Last_update;

        public string GithubOAuthKey;

        public bool IsSlashRequired = true;

        public int CallperMinute = 0;
    }

    [Configuration("CoquettishConfig")]
    internal class CoquettishConfig : Configuration<CoquettishConfig>
    {
        public List<CoquettishWord> CoquettishWords = new List<CoquettishWord>();
    }
    public class CoquettishWord
    {
        public CoquettishWord(string word, string result)
        {
            this.word = word;
            this.result = result;
        }

        public string word { get;}
        public string result { get;}
    }

    [Configuration("GraceGroupMangerConfig")]
    internal class GraceGroupMangerConfig : Configuration<GraceGroupMangerConfig>
    {
        public List<GraceGroupConfig> groupConfigs = new List<GraceGroupConfig>();
    }

    [Configuration("GraceGroupConfig")]
    internal class GraceGroupConfig : Configuration<GraceGroupConfig>
    {
        public string GroupQQ;
        public bool IsBanPostLotto = true;
        public bool Alert = false;
        public bool JoinRequestMember = true;
        public List<string> AutoMessage = new List<string>();
    }


}