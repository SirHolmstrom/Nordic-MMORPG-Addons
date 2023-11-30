using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public enum EmoteType
{
    Loopable,
    OneShot
}

[System.Serializable]
public class EmoteData
{
    /// <summary>
    /// Easier way to grab one of the Emote Datas by GetEmote("Dance"), it's just easier and only done on clients.
    /// </summary>
    public string identifier;

    /// <summary>
    /// Allow you to type out a text in chat as the emote plays. "starts dancing alone." (Optional).
    /// </summary>
    public EmoteTextGroup EmoteText;

    /// <summary>
    /// Loopable emote or one shot?
    /// </summary>
    public EmoteType emoteType;


    /// <summary>
    /// Every possible animation, they will be picked at random and played directly by the animator, IE Dance might have two different versions.
    /// </summary>
    [Space]
    public AnimationClip[] animationClips;

    /// <summary>
    /// If we wish to play an audio as the emote starts playing.
    /// </summary>
    public AudioClip[] audioClips;

    // Constructor
    public EmoteData(string identifier, AnimationClip[] animationClips, AudioClip[] audioClips, EmoteType emoteType)
    {
        this.identifier = identifier;
        this.animationClips = animationClips;
        this.audioClips = audioClips;
        this.emoteType = emoteType;
    }

}

[System.Serializable]
public class EmoteTextGroup
{
    /// <summary>
    /// Allow you to type out a text in chat as the emote plays. "starts dancing alone." (Optional).
    /// </summary>
    public string emoteQuote;

    /// <summary>
    /// Allow you to type out a text in chat as the emote plays."starts dancing with *target name inserted later*" (Optional).
    /// </summary>
    public string emoteQuoteTarget;
}
