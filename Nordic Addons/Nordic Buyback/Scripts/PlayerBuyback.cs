using UnityEngine;
using Mirror;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Linq;

public partial class PlayerNpcTrading
{
    #region Fields

    [Header("Buyback Settings:")]
    [SerializeField] private int MaximumBuybackAmount = 10;

    #endregion

    #region Synclists & Lists

    // the synclist that holds 
    private readonly SyncList<ItemSlot> buybackSlots = new SyncList<ItemSlot>();

    // event we'll have UI redrawn in.
    public UnityEvent<List<ItemSlot>> ItemSlotChanged;

    #endregion

    #region Mirror callbacks

    public override void OnStartLocalPlayer()
    {
        // we want to subscribe to the synclist so that we can draw UI from it, just easier to manage.
        buybackSlots.Callback += OnBuybackChanged;

        // our new method to redraw the buyback UI.
        ItemSlotChanged.AddListener(UINpcTrading.singleton.RefreshUI);
    }


    #endregion

    #region Callback Method

    private void OnBuybackChanged(SyncList<ItemSlot>.Operation op, int itemIndex, ItemSlot oldItem, ItemSlot newItem)
    {
        // just to manage the prefabs we need to draw so we don't have to run this in Update()
        ItemSlotChanged.Invoke(buybackSlots.ToList());
    }

    #endregion

    #region Server Methods

    [ServerCallback]
    public void HandleBuyback(int index, int amount)
    {
        // the item the player just sold.
        ItemSlot soldItemSlot = new ItemSlot(inventory.slots[index].item, amount);

        // check if the same item already excists.
        int excistingItemAmount = GetItemInBuyBackAmount(soldItemSlot, out int _index);

        // is the same item type already in the buyback list?
        if (excistingItemAmount != -1)
        {
            // caching the maxStack.
            int maxStack = soldItemSlot.item.data.maxStack;

            // check if the item can stack.
            if (maxStack > 1)
            {
                // the total amount of the sold item and the currently excisting item in the buy back list.
                int newStackAmount = (amount + excistingItemAmount);

                // does it fit or should we just make a new entry? (traditionally items sold at another time don't stack with other items so we won't bother unless it's a direct fit).
                if (maxStack <= newStackAmount)
                {
                    // get the old duplicated entry's slot so that we can modify it.
                    ItemSlot oldSlot = buybackSlots[_index];

                    // add the values together.
                    oldSlot = new ItemSlot(oldSlot.item, newStackAmount);

                    // set dirty and update.
                    buybackSlots[_index] = oldSlot;

                    // nothing else to do here.
                    return;
                }
            }
        }

        // add it to buyback.
        buybackSlots.Add(soldItemSlot);

        // removes the oldest entry if the amount is higher than max amount (10 by default).
        if (buybackSlots.Count > MaximumBuybackAmount) buybackSlots.RemoveAt(0);

    }

    #endregion

    #region Commands

    // buy buyback logic, we don't send the amount because you have to buy everything back,
    // it's simple here if a misstake was made.   
    [Command]
    public void CmdBuyBuybackItem(int index)
    {
        // validate: close enough, npc alive and valid index?
        // use collider point(s) to also work with big entities
        if (player.state == "IDLE" &&
            player.target != null &&
            player.target.health.current > 0 &&
            player.target is Npc npc &&
            npc.trading != null && // only if Npc offers trading
            Utils.ClosestDistance(player, npc) <= player.interactionRange &&
            0 <= index && index < npc.trading.saleItems.Length)
        {
            ItemSlot slot = buybackSlots[index];

            Item buybackItem = slot.item;

            // valid amount?
            if (1 <= slot.amount && slot.amount <= buybackItem.maxStack)
            {
                // the cost is the sell price so you can buy it back at the same rate until cleared.
                long price = buybackItem.sellPrice * slot.amount;

                // enough gold and enough space in inventory?
                if (player.gold >= price && inventory.CanAdd(buybackItem, slot.amount))
                {
                    // remove the buyback item.
                    buybackSlots.RemoveAt(index);

                    // pay for it, add to inventory
                    player.gold -= price;

                    inventory.Add(buybackItem, slot.amount);
                }
            }
        }
    }

    #endregion

    #region Helpers

    /// <summary>
    /// Returns the amount of an item if there is a duplicated entry in the buyback list and the slots index inside the Buyback list.
    /// </summary>
    private int GetItemInBuyBackAmount(ItemSlot slot, out int _index)
    {
        // try to find a item of the same type.
        for (int i = 0; i < buybackSlots.Count; i++)
        {
            // if there is a match:
            if (buybackSlots[i].item.data == slot.item.data)
            {
                // return the i so we have a buyback index reference.
                _index = i;

                // return the amount.
                return buybackSlots[i].amount;
            }
        }

        // nothing interesting here.
        _index = -1;

        // nothing interesting here.
        return -1;
    }


    #endregion

    #region Validate
    private void OnValidate()
    {
        // there is no reason for other players to know the buyback info, let's save some bandwidth while we can!
        if (syncMode != SyncMode.Owner) 
            syncMode = SyncMode.Owner;
    }
    #endregion
}