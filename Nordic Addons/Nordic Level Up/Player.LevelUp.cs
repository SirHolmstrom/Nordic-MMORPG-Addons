using UnityEngine;
using Mirror;

#if UNITY_EDITOR
using UnityEditor.Events;
#endif

public partial class PlayerExperience
{
    [Header("Rewards")]
    public LevelUpRewardData RewardData;

    [ServerCallback]
    private void HandleLevelUp()
    {
        // give the rewards.
        if (RewardData != null)
            RewardData.GiveRewards(party.player, level.current);
    }

    // helper to assigning listener.
    #region Editor Only
#if UNITY_EDITOR

    private void OnValidate()
    {
        // adds it automatically.
        if (!onLevelUp.ContainsPersistentListener(""))
        {
            // add the messages handling to player chat (IE /dance /wave etc..).
            UnityEventTools.AddPersistentListener(onLevelUp, HandleLevelUp);
        }

    }

#endif
    #endregion

}