using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIDamageTrackerBar : MonoBehaviour
{
    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI playerNameText;
    [SerializeField] private TextMeshProUGUI damageDisplayText;

    [Header("Images")]
    [SerializeField] private Image percentBarFill;
    [SerializeField] private Image classIconImage;

    [Header("Trigger")]
    public EventTrigger trigger;
    private DamageTrackerUI uiManager;
    private DamageSource damageSourceData;

    /// <summary>
    /// Initialize the damage tracking bar for the specific player.
    /// </summary>
    public void Initialize(Player player, DamageTrackerUI uiManager, ScrollRect scrollView)
    {
        this.damageSourceData = new DamageSource(player);
        this.uiManager = uiManager;

        // set player's name.
        playerNameText.text = damageSourceData.Name;
        // set class icon.
        classIconImage.sprite = damageSourceData.Player.classIcon;

        ConfigureEventTrigger(scrollView);

    }

    /// <summary>
    /// Refresh the displayed damage value.
    /// </summary>
    public void RefreshDamage(int value)
    {
        damageDisplayText.text = value.ToString();
    }

    /// <summary>
    /// Get the player associated with this UI element.
    /// </summary>
    public Player GetAssignedPlayer()
    {
        return damageSourceData.Player;
    }

    /// <summary>
    /// Configures event triggers for scroll view interaction.
    /// </summary>
    private void ConfigureEventTrigger(ScrollRect scrollView)
    {
        if (!scrollView) return;

        // Setup event triggers for various drag and scroll events
        setupEventTrigger(EventTriggerType.BeginDrag, (data) => { scrollView.OnBeginDrag((PointerEventData)data); });
        setupEventTrigger(EventTriggerType.Drag, (data) => { scrollView.OnDrag((PointerEventData)data); });
        setupEventTrigger(EventTriggerType.EndDrag, (data) => { scrollView.OnEndDrag((PointerEventData)data); });
        setupEventTrigger(EventTriggerType.InitializePotentialDrag, (data) => { scrollView.OnInitializePotentialDrag((PointerEventData)data); });
        setupEventTrigger(EventTriggerType.Scroll, (data) => { scrollView.OnScroll((PointerEventData)data); });
    }

    /// <summary>
    /// Updates the damage bar UI with the latest report.
    /// </summary>
    public void UpdateDamageBar(PlayerDamageReport report, int index, float fillPercentage)
    {
        damageDisplayText.text = DamageFormat.FormatDamageMetrics(report.TotalDamageStats, fillPercentage, uiManager.DisplaySettings);
        transform.SetSiblingIndex(index); // Update position in list
        percentBarFill.fillAmount = fillPercentage; // Update fill percentage
        percentBarFill.color = report.DamageSource.Color; // Set color based on damage source
    }

    /// <summary>
    /// Helper method to setup individual event triggers.
    /// </summary>
    private void setupEventTrigger(EventTriggerType type, UnityEngine.Events.UnityAction<BaseEventData> action)
    {
        EventTrigger.Entry entry = new EventTrigger.Entry { eventID = type };
        entry.callback.AddListener(action);
        trigger.triggers.Add(entry);
    }

}

