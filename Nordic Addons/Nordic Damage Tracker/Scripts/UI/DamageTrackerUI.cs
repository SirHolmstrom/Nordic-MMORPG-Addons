using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public partial class DamageTrackerUI : MonoBehaviour
{
    public DamageTrackerManager damageManager;

    public UIDamageTrackerBar damageBarPrefab;

    private Dictionary<Player, UIDamageTrackerBar> damageBarDictionary = new Dictionary<Player, UIDamageTrackerBar>();

    public KeyCode[] toggleUIHotkeys;

    public DamageFormat.DamageDisplaySettings DisplaySettings => displaySettings;

    [SerializeField]
    private DamageFormat.DamageDisplaySettings displaySettings =
    new DamageFormat.DamageDisplaySettings(10000, "K", "M", "n1", "DPS | TOTAL (PERCENT)");

    [SerializeField] private ScrollRect scrollView;
    [SerializeField] private Transform content;
    [SerializeField] private CanvasGroup canvasGroup;

    private void Update()
    {
        // toggling UI visibility with hotkeys.
        if (Utils.AnyKeyDown(toggleUIHotkeys))
        {
            ToggleUIVisibility();
        }
    }

    // event subscriptions.
    private void OnEnable()
    {
        damageManager.OnNewDamageSourceDetected += AddNewDamageBar;
        damageManager.OnDamageUpdated += RefreshDamageBars;
    }

    private void OnDisable()
    {
        damageManager.OnNewDamageSourceDetected -= AddNewDamageBar;
        damageManager.OnDamageUpdated -= RefreshDamageBars;
    }

    // handle new damage sources.
    private void AddNewDamageBar(DamageSource skillData)
    {
        var newBar = Instantiate(damageBarPrefab, content);
        newBar.name = $"DamageBar - {skillData.Player}";
        damageBarDictionary.Add(skillData.Player, newBar);

        newBar.Initialize(skillData.Player, this, scrollView);
    }

    // refresh UI with updated damage data.
    private void RefreshDamageBars(List<PlayerDamageReport> newValues)
    {
        var accumulatedDamage = newValues.Sum(wrapper => wrapper.TotalDamageStats.AccumulatedDamage);

        for (var index = 0; index < newValues.Count; index++)
        {
            var wrapper = newValues[index];
            var damageBarUI = damageBarDictionary[wrapper.DamageSource.Player];
            damageBarUI.UpdateDamageBar(wrapper, index, (float)(wrapper.TotalDamageStats.AccumulatedDamage / accumulatedDamage));
        }
    }

    // Toggle damage tracking.
    public void ToggleDamageTracking()
    {
        damageManager.ToggleTracking();
    }


    // Reset and clear damage data.
    public void ClearDamageTracker()
    {
        damageManager.ResetTracking();
        foreach (var bar in damageBarDictionary.Values)
        {
            Destroy(bar.gameObject);
        }
        damageBarDictionary.Clear();
    }

    // toggles UI.
    public void ToggleUIVisibility()
    {
        bool isVisible = canvasGroup.alpha == 0;
        canvasGroup.alpha = isVisible ? 1 : 0;
        canvasGroup.blocksRaycasts = isVisible;
    }

}