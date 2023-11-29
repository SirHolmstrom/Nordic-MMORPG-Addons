using System;
using UnityEngine;
using Random = System.Random;

public static class NordicExtension
{
    private static Random random = new Random();

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

    /// <summary>
    /// Extension method for Array to get a random element    
    /// </summary>
    public static T GetRandom<T>(this T[] array)
    {
        if (array == null || array.Length == 0)
        {
            return default(T);
        }

        int randomIndex = random.Next(array.Length);
        return array[randomIndex];
    }
}
