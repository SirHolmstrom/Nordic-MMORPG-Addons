using System;
using UnityEngine.Events;
using Mirror;
using UnityEngine;

// this is still somewhat experimental so it will change overtime, the UnityAction we are adding to DealDamageAt
// could be very useful for other addons I had in mind but I'm not sure if I want it to be a required thing just yet.

public partial class Combat
{
    // add to Combat::DealDamageAt if the Entity is Players for now.
    [HideInInspector] public UnityEventEntityInt onServerEntityDealsDamage;

    // event called on clients only.
    public UnityAction<Player, int> onClientDamageTrackerPlayerDealsDamage;

    [ClientRpc]
    public void RPCHandlePlayerDamage(Entity damageDealer, int damage)
    {
        entity.combat.CheckCandidateDamageTracker(damageDealer, damage);
    }

    [ClientCallback]
    private void CheckCandidateDamageTracker(Entity damageDealer, int damage)
    {
        // better to do the casting on clients rather than servers,
        // we can also add tracker for damage taken if we want to later.
        if (damageDealer is Player dealer)
        {
            Player.localPlayer.combat.onClientDamageTrackerPlayerDealsDamage?.Invoke(dealer, damage);
        }

    }

    #region Might use later

    // [SERVER ONLY] just for caching while we can to prevent a future casting.
    //private Player player; // (MANUALLY)

    // wasn't used so I took it, can also be manually assigned.
    //public override void OnStartServer()
    //{
    //    //still not sure if I want this on all entities with combat components,
    //    //we can cast at start for now.
    //    if (entity is Player player)
    //    {
    //        //caching(MANUALLY)
    //        this.player = player;

    //        //adds RPC to our listener if we are players. (MANUALLY)
    //        onServerEntityDealsDamage += RPCHandlePlayerDamage;
    //    }
    //}
    #endregion

}

public partial class Player
{
    // popular way to differenciate classes, could be added to chat messages too.
    public ClassColors classColor = ClassColors.Warrior;

    /// <summary>
    /// If partyID was synced we could use this, less compuation.
    /// </summary>
    public bool InSameParty(Player damageDealer)
    {
        return party.party.partyId == damageDealer.party.party.partyId;
    }

    /// <summary>
    /// Check via string if the other player is part of our party via members.
    /// </summary>
    /// <param name="otherPlayer"></param>
    /// <returns>If the other person is a member of the targeted players party.</returns>
    public bool InSamePartyAs(string otherPlayer)
    {
        if (!party.InParty()) return false;

        // we only have access to members because uMMORPG syncs the party strcut to OWNER so we don't have the party ids.
        foreach (string members in party.party.members)
        {
            // match by names.. (I don't like this either).
            if (otherPlayer == members)
            {
                return true;
            }
        }

        return false;

    }
}