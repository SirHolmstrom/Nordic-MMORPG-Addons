using Mirror;
using System.Collections;
using System.Collections.Generic;
using Unity.Plastic.Newtonsoft.Json.Bson;
using UnityEngine;

public partial class Player
{
    [Header("Emote")]
    public PlayerEmote emote;

#if UNITY_EDITOR

    private void OnValidate()
    {
        if (emote == null) 
        {
            if(TryGetComponent(out PlayerEmote _emote))
            {
                emote = _emote;
            }
        }
    }

#endif

}
