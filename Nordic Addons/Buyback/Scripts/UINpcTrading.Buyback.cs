using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class UINpcTrading
{
    [Header("Buyback: ")]
    [SerializeField] private GameObject buybackPanel;
    [SerializeField] private Transform buybackContent;
    [SerializeField] private UIBuybackSlot buybackPrefab;

    /// <summary>
    /// Will draw the whole buy/sell UI.
    /// </summary>
    /// <param name="buybackSlots"></param>
    public void RefreshUI(List<ItemSlot> buybackSlots)
    {
        buybackPanel.SetActive(buybackSlots.Count > 0);

        // we start by removing all old entries.
        buybackContent.RemoveAllChildren();

        // loop and add all the items again.
        for (int i = 0; i < buybackSlots.Count; i++)
        {
            // cache.
            int ind = i;

            // cache the slot.
            ItemSlot slot = buybackSlots[ind];

            // spawn the slot.
            UIBuybackSlot uiSlot = Instantiate(buybackPrefab, buybackContent);

            // setup the slot.
            uiSlot.Init(ind, slot);
        }

    }

}
