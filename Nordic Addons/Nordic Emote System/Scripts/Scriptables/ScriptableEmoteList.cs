using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New ScriptableEmoteList", menuName = "Emote System/Scriptable Emote List")]
public class ScriptableEmoteList : ScriptableObject
{
    [Header("Emote List: ")]
    public List<EmoteData> Emotes;

    [Header("Emote Settings: ")]
    // feel free to change both of this, you can even expose it but I want to prevent misstakes!
    public string EmoteAnimatorLayer = "Emote Fullbody";
    public string EmptyAnimatorState = "Empty";
    public float AnimationCrossfadeTime = 0.15f;

    private Dictionary<int, EmoteData> emoteDictionaryById;
    private Dictionary<string, EmoteData> emoteDictionaryByIdentifier;

    private void OnEnable()
    {
        Initialize();
    }

    public void Initialize()
    {
        emoteDictionaryById = new Dictionary<int, EmoteData>();
        emoteDictionaryByIdentifier = new Dictionary<string, EmoteData>();

        for (int i = 0; i < Emotes.Count; i++)
        {
            var emote = Emotes[i];
            emoteDictionaryById[i] = emote; // Using the index as the ID

            if (!string.IsNullOrEmpty(emote.identifier))
            {
                emoteDictionaryByIdentifier[emote.identifier] = emote;
            }
        }
    }

    #region Experimental
    static Dictionary<ScriptableEmoteList, List<EmoteData>> cache;
    public static Dictionary<ScriptableEmoteList, List<EmoteData>> All
    {
        get
        {
            // Not loaded yet?
            if (cache == null)
            {
                // Get all ScriptableEmoteList in resources
                ScriptableEmoteList[] scriptableEmoteLists = Resources.LoadAll<ScriptableEmoteList>("");

                // Check for duplicates, then add to cache
                List<string> duplicates = scriptableEmoteLists.ToList().FindDuplicates(item => item.name);
                if (duplicates.Count == 0)
                {
                    cache = scriptableEmoteLists.ToDictionary(item => item, item => item.Emotes);

                    foreach (var item in scriptableEmoteLists)
                    {
                        item.Initialize();
                    }
                }

                else
                {
                    foreach (string duplicate in duplicates)
                        Debug.LogError("Resources folder contains multiple EmoteLists with the name " + duplicate + ".");
                }
            }
            return cache;
        }
    }
    #endregion


    public EmoteData GetEmoteData(int id)
    {
        if (emoteDictionaryById.TryGetValue(id, out EmoteData emoteData))
        {
            return emoteData;
        }
        else
        {
            Debug.LogWarning($"Emote with ID {id} not found.");
            return null;
        }
    }

    public EmoteData GetEmoteData(string identifier)
    {
        if (emoteDictionaryByIdentifier.TryGetValue(identifier, out EmoteData emoteData))
        {
            return emoteData;
        }
        else
        {
            Debug.LogWarning($"Emote with Identifier {identifier} not found.");
            return null;
        }
    }

    public int GetEmoteID(string identifier)
    {
        if (emoteDictionaryByIdentifier.TryGetValue(identifier, out EmoteData emoteData))
        {
            return Emotes.IndexOf(emoteData);
        }
        else
        {
            Debug.LogWarning($"Emote with Identifier {identifier} not found.");
            return -1;
        }
    }

}
