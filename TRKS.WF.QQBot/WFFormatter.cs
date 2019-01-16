﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using Humanizer;
using Humanizer.Localisation;

namespace TRKS.WF.QQBot
{
    public static class WFFormatter
    {
        public static string ToString(List<Event> events)
        {
            var sb = new StringBuilder();
            sb.AppendLine("以下是游戏内所有的活动:");

            foreach (var @event in events)
            {
                var time = (@event.expiry - DateTime.Now).Humanize(int.MaxValue, CultureInfo.GetCultureInfo("zh-CN"), TimeUnit.Day, TimeUnit.Second, " ");
                sb.AppendLine($"[{@event.description}]");
                sb.AppendLine($"- 剩余点数: {@event.health}");
                sb.AppendLine($"- 结束时间: {time} 后");
                sb.AppendLine();

            }

            return sb.ToString().Trim();
        }

        public static string ToString(WFAlert alert)
        {
            var mission = alert.Mission;
            var reward = mission.Reward;
            var time = (alert.Expiry - DateTime.Now).Humanize(int.MaxValue, CultureInfo.GetCultureInfo("zh-CN"), TimeUnit.Day, TimeUnit.Second, " ");

            return $"[{mission.Node}] 等级{mission.MinEnemyLevel}~{mission.MaxEnemyLevel}:\r\n" +
                   $"- 类型:     {mission.Type} - {mission.Faction}\r\n" +
                   $"- 奖励:     {ToString(reward)}\r\n" +
                   //$"-过期时间: {alert.Expiry}({time} 后)" +
                   $"- 过期时间: {time} 后";
        }

        public static string ToString(List<Relic> relics)
        {
            var sb = new StringBuilder();
            foreach (var relic in relics)
            {
                var rewards = relic.Rewards.Split(' ').Select(reward => $"[{reward.Replace("_", " ")}]");
                var rewardstring = string.Join("", rewards);
                sb.AppendLine($"-{relic.Name}");
                sb.AppendLine($">{rewardstring}");
                sb.AppendLine();
            }

            return sb.ToString().Trim();
        }
        public static string ToString(List<RivenInfo> infos)
        {
            var weapon = infos.First().item_Class;
            var sb = new StringBuilder();
            sb.AppendLine($"下面是 {weapon} 紫卡的 {infos.Count} 条卖家信息.");
            foreach (var info in infos)
            {
                sb.Append($"[{info.user_Name}]  ");
                switch (info.user_Status)
                {
                    case 0:
                        sb.AppendLine("离线");
                        break;
                    case 1:
                        sb.AppendLine("在线");
                        break;
                    case 2:
                        sb.AppendLine("游戏中");
                        break;
                }

                sb.AppendLine($"- 价格:{info.item_Price}白鸡");
                sb.AppendLine($"- 属性:{info.item_Property}");
                sb.AppendLine();
            }

            return sb.ToString().Trim();
        }
        public static string ToString(List<Fissure> fissures)
        {
            var sb = new StringBuilder();
            foreach (var fissure in fissures)
            {
                sb.AppendLine($"[{fissure.node}]");
                sb.AppendLine($"类型:    {fissure.missionType}-{fissure.enemy}");
                sb.AppendLine($"纪元:    {fissure.tier}(T{fissure.tierNum})");
                sb.AppendLine($"{fissure.eta} 后过期");
                sb.AppendLine();
            }

            return sb.ToString().Trim();
        }
        public static string ToString(SyndicateMission mission)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"集团: {mission.syndicate}");
            sb.AppendLine();
            var count = 0;
            foreach (var job in mission.jobs)
            {
                count++;
                sb.AppendLine($"> 赏金{count}等级: {job.enemyLevels[0]} - {job.enemyLevels[1]}");
                sb.AppendLine("- 奖励:");
                foreach (var reward in job.rewardPool)
                {
                    sb.Append($"[{reward}]");                   
                }

                sb.AppendLine();
            }


            return sb.ToString().Trim();
        }
        public static string ToString(WFInvasion inv)
        {
            var sb = new StringBuilder();
            var completion = Math.Floor(inv.completion);

            sb.AppendLine($"地点: [{inv.node}]");

            sb.AppendLine($"> 进攻方: {inv.attackingFaction}");
            if (!inv.vsInfestation)
                sb.AppendLine($"奖励: {ToString(inv.attackerReward)}");
            sb.AppendLine($"进度: {completion}%");
            // sb.AppendLine();

            sb.AppendLine($"> 防守方: {inv.defendingFaction}");
            sb.AppendLine($"奖励: {ToString(inv.defenderReward)}");
            sb.Append($"进度 {100 - completion}%");
            return sb.ToString();
        }

        public static string ToString(CetusCycle cycle)
        {
            var time = (cycle.Expiry - DateTime.Now).Humanize(int.MaxValue, CultureInfo.GetCultureInfo("zh-CN"),
                TimeUnit.Hour, TimeUnit.Second, " ");
            var status = cycle.IsDay ? "白天" : "夜晚";
            var nextTime = !cycle.IsDay ? "白天" : "夜晚";

            var sb = new StringBuilder();
            sb.AppendLine($"现在地球平原的时间是: {status}");
            //sb.AppendLine($"将在 {cycle.Expiry} 变为 {nextTime}");
            sb.Append($"距离 {nextTime} 还有 {time}");

            return sb.ToString();
        }

        public static string ToString(VallisCycle cycle)
        {
            var time = (cycle.expiry - DateTime.Now).Humanize(int.MaxValue, CultureInfo.GetCultureInfo("zh-CN"),
                TimeUnit.Hour, TimeUnit.Second, " ");
            var temp = cycle.isWarm ? "温暖" : "寒冷";
            var nextTemp = !cycle.isWarm ? "温暖" : "寒冷";
            var sb = new StringBuilder();
            sb.AppendLine($"现在金星平原的温度是: {temp}");
            //sb.AppendLine($"将在{cycle.expiry} 变为 {nextTemp}");
            sb.Append($"距离 {nextTemp} 还有 {time}");

            return sb.ToString();
        }

        public static string ToString(Sortie sortie)
        {
            var sb = new StringBuilder();
            sb.AppendLine("指挥官, 下面是今天的突击任务.");
            sb.AppendLine($"> 阵营: {sortie.faction}");
            sb.AppendLine($"> 头头: {sortie.boss}");
            sb.AppendLine();
            foreach (var variant in sortie.variants)
            {
                sb.AppendLine($"[{variant.node}]");
                sb.AppendLine($"- 类型:{variant.missionType}");
                sb.AppendLine($"- 状态:{variant.modifier}");
            }

            return sb.ToString().Trim();
        }

        public static string ToString(VoidTrader trader)
        {
            var sb = new StringBuilder();
            if (trader.active)
            {
                var time = (DateTime.Now - trader.expiry).Humanize(int.MaxValue,
                    CultureInfo.GetCultureInfo("zh-CN"), TimeUnit.Day, TimeUnit.Second, " ");
                sb.AppendLine($"虚空商人已抵达: {trader.location}");
                sb.AppendLine($"携带商品:");
                foreach (var inventory in trader.inventory)
                {
                    sb.AppendLine($"         [{inventory.item}] {inventory.ducats}金币 + {inventory.credits}现金");
                }
                //sb.Append($"结束时间:{trader.expiry}({time} 后)");
                sb.Append($"结束时间: {time} 后");
            }
            else
            {
                var time = (DateTime.Now - trader.activation).Humanize(int.MaxValue,
                    CultureInfo.GetCultureInfo("zh-CN"), TimeUnit.Day, TimeUnit.Second, " ");
                //sb.Append($"虚空商人将在{trader.activation}({time} 后)抵达{trader.location}");
                sb.Append($"虚空商人将在 {time} 后 抵达{trader.location}");
            }

            return sb.ToString().Trim();
        }

        public static string ToString(WMInfo info)
        {
            var sb = new StringBuilder();
            var itemItemsInSet = info.include.item.items_in_set;
            var item = itemItemsInSet.Where(i => i.zh.item_name != i.en.item_name).ToList().Last();
            sb.AppendLine($"下面是物品: {item.zh.item_name} 按价格从小到大的{info.payload.orders.Length}条信息");
            sb.AppendLine();
            foreach (var order in info.payload.orders)
            {
                sb.AppendLine($"[{order.user.ingame_name}]   {order.user.status}");
                sb.AppendLine($"{order.order_type}  {order.platinum} 白鸡");
                sb.AppendLine(
                    $"- 快捷回复: /w {order.user.ingame_name} Hi! I want to buy: {item.en.item_name} for {order.platinum} platinum. (warframe.market)");
                sb.AppendLine();
            }
            // 以后不好看了再说
            return sb.ToString().Trim();
        }
        public static string ToString(WMInfoEx info)
        {
            var sb = new StringBuilder();
            var item = info.info;
            sb.AppendLine($"下面是物品: {item.zhName} 按价格从小到大的{info.orders.Length}条信息");
            sb.AppendLine();
            
            foreach (var order in info.orders)
            {
                sb.AppendLine($"[{order.userName}]   {order.status}");
                sb.AppendLine($"{order.order_Type}  {order.platinum} 白鸡");
                sb.AppendLine(
                    $"- 快捷回复: /w {order.userName} Hi! I want to buy: {item.enName} for {order.platinum} platinum. (warframe.market)");
                sb.AppendLine();
            }
            // 这已经很难看了好吧
            return sb.ToString().Trim();
        }

        public static string ToString(Defenderreward reward)
        {
            var rewards = new List<string>();
            if (reward.credits > 0)
            {
                rewards.Add($"{reward.credits} cr");
            }

            foreach (var item in reward.countedItems)
            {
                rewards.Add($"{item.count}x{item.type}");
            }

            return string.Join(" + ", rewards);
        }

        public static string ToString(Attackerreward reward)
        {
            var rewards = new List<string>();
            if (reward.credits > 0)
            {
                rewards.Add($"{reward.credits} cr");
            }

            foreach (var item in reward.countedItems)
            {
                rewards.Add($"{item.count}x{item.type}");
            }

            return string.Join(" + ", rewards);
        }
        public static string ToString(Reward reward)
        {
            var rewards = new List<string>();
            if (reward.Credits > 0)
            {
                rewards.Add($"{reward.Credits} cr");
            }

            foreach (var item in reward.Items)
            {
                rewards.Add(item);
            }

            foreach (var item in reward.CountedItems)
            {
                rewards.Add($"{item.Count}x{item.Type}");
            }

            return string.Join(" + ", rewards);
        }
    }
}
