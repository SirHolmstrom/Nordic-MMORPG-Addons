using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NordicUtils
{
    /// <summary>Check if x Seconds have elapsed since the Started Time </summary>
    public static bool ElapsedTime(float StartTime, float intervalTime) => (Time.time - StartTime) >= intervalTime;

    /// <summary>
    /// Check if NetworkTime is greater or equals the referenced StoredNetworkTime.
    /// Example (nextAttack = NetworkTime.Time + attackCooldown) so we check ElapsedNetworkTime(nextAttack).
    /// </summary>
    /// <returns>True if the time has passed and false if not.</returns>
    public static bool ElapsedNetworkTime(double StoredNetworkTime) => Mirror.NetworkTime.time >= StoredNetworkTime;

    public static bool IsPrefab(GameObject go) => !go.scene.IsValid();

    public static bool IsParent(Transform childObject, Transform parent)
    {
        Transform t = childObject.transform;

        while (t.parent != null)
        {
            if (t.parent == parent)
            {
                return true;
            }
            t = t.parent.transform;
        }
        return false; // Could not its parent
    }

}
