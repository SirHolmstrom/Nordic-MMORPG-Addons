using UnityEngine;

public static class NordicExtension
{
    /// <summary>
    /// Extension method to remove all children from a Transform or GameObject
    /// </summary>
    public static void RemoveAllChildren(this Transform parent)
    {
        foreach (Transform child in parent)
        {
            GameObject.Destroy(child.gameObject);
        }
    }
}
