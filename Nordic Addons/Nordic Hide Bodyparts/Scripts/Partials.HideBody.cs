using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

// Enum for body parts
public enum BodyPart
{
    HEAD = 0,
    CHEST = 1,
    ARMS = 2,
    HANDS = 3,
    LEGS = 4,
    FEET = 5
    // ... other body parts
}

// Mapping of body parts to their GameObjects
[System.Serializable]
public class BodyPartMapping
{
    // easier to track in editor.
    public string name = "";

    // the enum we match against.
    public BodyPart bodyPart;

    // gameobjects that will be affected.
    public GameObject[] partObjects;

    // more friendly way to disable/enable the go's.
    public void SetActive(bool active)
    {
        foreach (var go in partObjects)
        {
            if (go != null)
                go.SetActive(active);
        }
    }
}

// make this is partial, not by default.
public partial class EquipmentItem
{
    /// <summary>
    /// Bodyparts we want to hide when equipped.
    /// </summary>
    public BodyPart[] affectedBodyParts;
}

// Player equipment class
public partial class PlayerEquipment
{
    private void Start()
    {
        if (!isServerOnly)
        {
            // before the hook is called we need to do this somewhere.
            for (int i = 0; i < slots.Count; ++i)
            {
                StartClientBodyPartVisibility(i);
            }
        }
    }

    [Header("Hide Body Part Mapping: ")]
    [Space]
    // body mapping.
    public BodyPartMapping[] bodyPartMappings;

    // will be used to hook future addons too.
    public UnityEvent<EquipmentItem, EquipmentItem> OnEquipmentChangedEvent;

    public void StartClientBodyPartVisibility(int index)
    {
        ItemSlot slot = slots[index];

        if (slot.amount > 0)
        {
            EquipmentItem item = slot.item.data as EquipmentItem;

            if (item != null)
            {
                foreach (var part in item.affectedBodyParts)
                {
                    // try to find matching parts.
                    foreach (var mapping in bodyPartMappings)
                    {
                        // match? Turn them hide them.
                        if (mapping.bodyPart == part)
                            mapping.SetActive(false);
                    }
                }
            }
        }
    }

    public void UpdateBodyPartVisibility(EquipmentItem oldItem, EquipmentItem newItem)
    {
        // Show the old items objects again we previously hidden.
        // Useful so that if we remove the armor completely it restores initial state.
        if (oldItem != null)
        {
            foreach (var part in oldItem.affectedBodyParts)
            {
                // try to find matching parts.
                foreach (var mapping in bodyPartMappings)
                {
                    // match? Turn them back on.
                    if (mapping.bodyPart == part)
                        mapping.SetActive(true);
                }
            }
        }

        // Hide the new items objects.
        if (newItem != null)
        {
            foreach (var part in newItem.affectedBodyParts)
            {
                // try to find matching parts.
                foreach (var mapping in bodyPartMappings)
                {
                    // match? Turn them hide them.
                    if (mapping.bodyPart == part)
                        mapping.SetActive(false);
                }
            }
        }

    }

}