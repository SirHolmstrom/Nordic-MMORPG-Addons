using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UIBuybackSlot : MonoBehaviour
{
    #region Fields
    // texts.
    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI textItemName;
    [SerializeField] private TextMeshProUGUI textItemAmount;
    [SerializeField] private TextMeshProUGUI textBuybackPrice;

    // image.
    [Header("Image")]
    [SerializeField] private Image icon;

    // btn
    [Header("Button")]
    [SerializeField] private Button btn;

    #endregion

    #region Init

    // we use the whole ItemSlot for convinience, even if this is tampered with it's only a visual representation of the synclist.
    public void Init(int index, ItemSlot slot)
    {
        // get the data.
        ScriptableItem itemData = slot.item.data;

        // icon.
        icon.sprite = itemData.image;

        // set the name.
        textItemName.text = itemData.name;

        // set the amount.
        textItemAmount.text = slot.amount.ToString();

        // buy price, it costs the sell price to get it back, simple misstake.
        textBuybackPrice.text = (itemData.sellPrice * slot.amount).ToString();

        // setup the button.
        btn.onClick.SetListener(() => { Player.localPlayer.npcTrading.CmdBuyBuybackItem(index); } );   
    }

    #endregion
}
