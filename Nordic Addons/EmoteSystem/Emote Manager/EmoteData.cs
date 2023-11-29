using System.Collections.Generic;
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
    /// Loopable emote or one shot?
    /// </summary>
    public EmoteType emoteType;

    /// <summary>
    /// Every possible animation, they will be picked at random and played directly by the animator, IE Dance might have two different versions.
    /// </summary>
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

    // Method to get a random AnimationClip
    public AnimationClip GetRandomAnimationClip()
    {
        if (animationClips != null && animationClips.Length > 0)
        {
            return animationClips[UnityEngine.Random.Range(0, animationClips.Length)];
        }
        return null;
    }

}
