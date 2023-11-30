using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditor.Animations;

[CreateAssetMenu(fileName = "EmoteManager", menuName = "Emote System/Emote Manager", order = 1)]
public class EmoteManager : ScriptableObject
{
    [SerializeField]
    private List<EmoteData> emotes = new List<EmoteData>();
    private Dictionary<int, EmoteData> emoteDictionaryById;
    private Dictionary<string, EmoteData> emoteDictionaryByIdentifier;

    private static EmoteManager _instance;

    //public static EmoteManager Settings => _instance ?? (_instance = Resources.Load<EmoteManager>("EmoteManager") );


    public static EmoteManager Instance
    {
        get
        {
            
            if (!_instance)
            {
                _instance = Resources.Load<EmoteManager>("EmoteManager");

                if (_instance != null)
                {
                    _instance.Initialize();
                }

                else
                {
                    Debug.LogError("Failed to load EmoteManager from Resources.");
                }
            }

            return _instance;
        }
    }

    // feel free to change both of this, you can even expose it but I want to prevent misstakes!
    [HideInInspector] public string Emote_Animator_Layer = "Emote Fullbody";
    [HideInInspector] public string Empty_Animator_State = "Empty";

    public List<EmoteData> Emotes => emotes;

    // EDITOR ONLY
    public List<AnimatorController> animatorControllers;

    public void Initialize()
    {
        Debug.Log("INIT");
        emoteDictionaryById = new Dictionary<int, EmoteData>();
        emoteDictionaryByIdentifier = new Dictionary<string, EmoteData>();

        for (int i = 0; i < emotes.Count; i++)
        {
            var emote = emotes[i];
            emoteDictionaryById[i] = emote; // Using the index as the ID

            if (!string.IsNullOrEmpty(emote.identifier))
            {
                emoteDictionaryByIdentifier[emote.identifier] = emote;
            }
        }
    }

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

    public int GetEmoteDataID(string identifier)
    {
        if (emoteDictionaryByIdentifier.TryGetValue(identifier, out EmoteData emoteData))
        {
            for (int i = 0; i < emotes.Count; i++)
            {
                int ind = i;

                if (emoteData == emotes[i]) 
                {
                    Debug.Log(ind);
                    return ind;
                }
            }
        }

        return -1;
    }
}
