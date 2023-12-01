using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "uMMORPG Level Up/Level Up Rewards Data", order = 999)]
public class LevelUpRewardData : ScriptableObject
{
    public List<LevelUpReward> LevelUpRewards;

    public void GiveRewards(Player player, int level)
    {
        LevelUpReward TierReward = LevelUpRewards[level];

        if (TierReward.items.Length > 0)
        {
            // for now it's handle like this, could in future send to mail,
            // or have it be like a gift you unwrap?
            // or simply a UI that waits for you to claim it.
            // or could just save to database every level that's claimed reward for.
            foreach (var item in TierReward.items)
            {
                player.inventory.Add(new Item(item.item), item.amount);
            }
        }

        if (TierReward.rewards.HasRewards)
        {
            TierReward.rewards.GiveRewards(player);
        }

    }
}


[System.Serializable]
public struct LevelUpReward
{
    public string name;
    public ScriptableItemAndAmount[] items;
    public GeneralRewards rewards;
}

[System.Serializable]
public struct GeneralRewards
{
    public int gold;
    public int storeCoins;
    public int skillExperience;

    public bool HasRewards => gold != 0 || storeCoins != 0 || skillExperience != 0;

    public void GiveRewards(Player player)
    {
        player.gold += gold;
        player.itemMall.coins += storeCoins;
        ((PlayerSkills)player.skills).skillExperience += skillExperience;
    }
}