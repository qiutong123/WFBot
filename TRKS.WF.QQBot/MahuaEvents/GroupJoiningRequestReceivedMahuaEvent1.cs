using Newbe.Mahua.MahuaEvents;
using System;
using Newbe.Mahua;
using System.Text.RegularExpressions;

namespace TRKS.WF.QQBot.MahuaEvents
{
    /// <summary>
    /// 入群申请接收事件
    /// </summary>
    public class GroupJoiningRequestReceivedMahuaEvent1
        : IGroupJoiningRequestReceivedMahuaEvent
    {
        private readonly IMahuaApi _mahuaApi;

        public GroupJoiningRequestReceivedMahuaEvent1(
            IMahuaApi mahuaApi)
        {
            _mahuaApi = mahuaApi;
        }

        public void ProcessJoinGroupRequest(GroupJoiningRequestReceivedContext context)
        {
            if (HotUpdateInfo.PreviousVersion) return;

            if (Config.Instance.AcceptJoiningRequest)
            {
                _mahuaApi.AcceptGroupJoiningRequest(context.GroupJoiningRequestId, context.ToGroup, context.FromQq);
                string[] sArry = Regex.Split(context.Message, "答案：", RegexOptions.IgnoreCase);
                if(sArry.Length == 2)
                {
                    _mahuaApi.SetGroupMemberCard(context.ToGroup, context.FromQq, sArry[1]);
                    Messenger.SendGroup(context.ToGroup, $"大佬 [CQ:at,qq={context.FromQq}] 加入了本群，群地位-1.");
                }       
                Messenger.SendDebugInfo($"{context.FromQq}加入了群{context.ToGroup}.");
            }
        }
    }
}