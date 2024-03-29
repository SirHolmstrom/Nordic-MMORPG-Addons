using UnityEngine;
using System.Text;
using Mirror;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor.Animations;
using UnityEditor.Events;
#endif

public class PlayerEmote : NetworkBehaviour
{
    // all Fields.
    #region Fields

    [Header("Component")]
    // the emote data reference.
    public ScriptableEmoteList EmoteData;

    // our wonderful player reference, assigned automatically.
    public Player player;

    /// this value is the index of our current emote,
    /// player will find the emote from the local dict that the emote manager loads.
    [SyncVar(hook = nameof(OnEmoteChanged))] private int currentEmoteIndex = -1;

    /// cheap and reliable way to help late joiners or networked culled players seeing us for the first time.
    [SyncVar] private byte animationVariant;

    /// we don't need to sync this because it's Server > Client sync direction,
    /// only server needs to know when to stop.
    // server-only timer for one-shot emotes.
    private double NextEmoteEndTime = 0;

    // local random instance for audio selection.
    private System.Random localRandom = new System.Random();

    // layer index for emotes, fetched on client start.
    private int _emoteLayerIndex = 0;

    #region Checks

    // readonly property to check if a one-shot emote is playing.
    private bool _isPlayingOneShot => NextEmoteEndTime != -1;

    // readonly property to check if we have a ongoing emote playing.
    private bool _isPlayingEmote => currentEmoteIndex != -1;

    // I don't like using strings for this but it is what it is.
    private bool _allowedMovementState => player.state == "IDLE";

    // checks if we have a oneshot and is currently "emoting".
    public bool isPlayingOneShotEmote => _isPlayingEmote && _isPlayingOneShot;

    // simple way of checking if we should stop emoting.
    public bool EventEmoteTimeElapsed => isPlayingOneShotEmote && NordicUtils.ElapsedNetworkTime(NextEmoteEndTime);

    #endregion

    #region Helpers

    // helper property for audio source, player's is used by default.
    private AudioSource _audioSource => player.audioSource;

    #endregion

    // just a editor inspector.
    #region editor only
#   if UNITY_EDITOR
    public List<AnimatorController> animatorControllers = new List<AnimatorController>();
    #endif
    #endregion

    #endregion

    // Unity Callbacks.
    #region Unity Callbacks

    private void Update()
    {
        if (isServer)
        {
            // we don't care if no emote is playing.
            if (!_isPlayingEmote) return;

            // if the oneshot animation is finished or we left IDLE.
            if (EventEmoteTimeElapsed || !_allowedMovementState)
            {
                // reset emote.
                ResetEmote();
            }
        }
    }

    #endregion

    // Mirror Callbacks.
    #region Mirror Callback

    // this is just for example purposes:
    public override void OnStartLocalPlayer()
    {
        UIEmotes ui = FindObjectOfType<UIEmotes>();

        if(ui != null)
        {
            ui.Init(EmoteData);
        }
    }

    public override void OnStartClient()
    {
        // get the animator layer for the emotes.
        if (EmoteData != null)
            _emoteLayerIndex = player.animator.GetLayerIndex(EmoteData.EmoteAnimatorLayer);

        // this is for late joiners.
        if (!isLocalPlayer)
        {
            // check if something should be emoted.
            if (currentEmoteIndex != -1)
            {
                // emote it.
                StartEmote(currentEmoteIndex);
            }
        }
    }

    #endregion

    // logic for emote system.
    #region Emote Logic

    [Client]
    public void TryEmote(string identifier)
    {
        // Validate identifier and local player
        if (string.IsNullOrEmpty(identifier) || !isLocalPlayer) return;

        // Validate movement state
        if (!_allowedMovementState) return;

        // Get emote ID and validate
        int emoteID = EmoteData.GetEmoteID(identifier);
        if (emoteID == -1) return;

        // Get emote data and validate
        EmoteData data = EmoteData.GetEmoteData(emoteID);
        if (data == null) return;

        // Select a random animation index
        byte selectedClipIndex = (byte)Random.Range(0, data.animationClips.Length);

        // just need the lenght to actually roll correctly,
        // but need to clamp just to be sure (Count vs Lenght).
        selectedClipIndex = (byte)Mathf.Clamp(selectedClipIndex, 0, data.animationClips.Length - 1);

        // Send command to set the current emote
        CmdSetCurrentEmote(emoteID, selectedClipIndex);

    }

    [Command]
    private void CmdSetCurrentEmote(int emoteIndex, byte clipIndex)
    {
        // set the emote & variant.
        animationVariant = clipIndex;
        currentEmoteIndex = emoteIndex;

        // get the data so we can validate the emote type and if we are using a one-shot we need timestamp.
        // if it's loopable, we set -1 as it means it don't have an end-time, so it loops. (in this context).
        EmoteData data = EmoteData.GetEmoteData(emoteIndex);
        NextEmoteEndTime = data.emoteType == EmoteType.OneShot
                           ? NetworkTime.time + data.animationClips[clipIndex].length
                           : -1;
    }


    [ClientCallback]
    private void OnEmoteChanged(int oldEmoteID, int newEmoteID)
    {
        // do nothing but stop playing.
        if (newEmoteID == -1)
        {
            // set animators layer weight to 0 and force the Empty State.
            StopEmoteAnimation();
            return;
        }

        // play the emote.
        StartEmote(currentEmoteIndex);
    }

    [ClientCallback]
    private void StartEmote(int id)
    {
        EmoteData data = EmoteData.GetEmoteData(id);

        if (data != null)
        {
            AnimationClip clip = data.animationClips[animationVariant];

            // audio.
            PlayEmoteAudio(data);

            // emote message.
            ShowEmoteMessage(data);

            // animate.
            PlayEmoteAnimation(clip);
        }
        else
        {
            Debug.LogWarning($"Emote with id'{id}' not found.");
        }
    }

    [ClientCallback]
    private void PlayEmoteAnimation(AnimationClip clip)
    {
        if (clip != null)
        {
            // just the defaul way of doing this is uMMORPG.
            foreach (Animator anim in GetComponentsInChildren<Animator>())
            {
                anim.CrossFade(clip.name, EmoteData.AnimationCrossfadeTime, _emoteLayerIndex);
            }
        }
    }

    // *** will try later to eleminate and base the seed on "oldEmoteID" inside the hook ***
    [ClientCallback]
    private void PlayEmoteAudio(EmoteData data)
    {
        if (data.audioClips.Length > 0)
        {
            // make sure everyone plays the same "random" audio clip.
            localRandom = new System.Random(animationVariant);
            int audioIndex = localRandom.Next(data.audioClips.Length);
            AudioClip audio = data.audioClips[audioIndex];
            player.audioSource.PlayOneShot(audio);
        }
    }

    [ClientCallback]
    public void StopEmoteAnimation()
    {
        // just the defaul way of doing this is uMMORPG.
        foreach (Animator anim in GetComponentsInChildren<Animator>())
        {
            anim.CrossFade(EmoteData.EmptyAnimatorState, EmoteData.AnimationCrossfadeTime, _emoteLayerIndex);
        }
    }

    [ServerCallback]
    public void ResetEmote()
    {
        currentEmoteIndex = -1;
    }

    #endregion

    // logic for /emote commands.
    #region Chat Logic

    public void HandleMessageEmote(string message)
    {
        // should already be converted to lowercase but not taking any chances.
        string checkEmoteText = message.ToLower();

        if (EmoteData != null)
        {
            foreach (var emote in EmoteData.Emotes)
            {
                string check = emote.identifier.ToLower();

                if (checkEmoteText == "/" + check)
                {
                    if (isLocalPlayer)
                    {
                        Player.localPlayer.emote.TryEmote(emote.identifier);
                    }
                }
            }
        }
    }

    [Client]
    public void ShowEmoteMessage(EmoteData data)
    {
        bool isEmoter = player == Player.localPlayer;

        // see if player has a target.
        bool hasTarget = player.target != null;

        // string builder to replace the names.
        StringBuilder sb = new StringBuilder(hasTarget && !string.IsNullOrWhiteSpace(data.EmoteText.emoteQuoteTarget) ? data.EmoteText.emoteQuoteTarget : data.EmoteText.emoteQuote);

        // if our stringbuilder is null return.
        if (sb.Length == 0) return;

        // always change {EMOTER} to players name.
        sb.Replace("{EMOTER}", isEmoter ? "You" : player.name);

        // if the player that does the emote has a target and there is a emoteQuote for target:
        if (hasTarget && !string.IsNullOrWhiteSpace(data.EmoteText.emoteQuoteTarget))
        {
            // change {TARGET} with it's name.
            sb.Replace("{TARGET}", player.target == Player.localPlayer ? "You" : player.target.name);
        }

        UIChat.singleton.AddEmoteMessage(new ChatMessage("", "", "" + sb.ToString().Colorize("#FF9900"), "", player.chat.localChannel.textPrefab));
    }

    #endregion

    // helper to assigning player.
    #region Editor Only
#if UNITY_EDITOR

    private void OnValidate()
    {
        if (player == null)
            player = GetComponent<Player>();

        if (player)
        {
            // adds it automatically.
            if (!player.chat.onSubmit.ContainsPersistentListener("HandleMessageEmote"))
            {
                // add the messages handling to player chat (IE /dance /wave etc..).
                UnityEventTools.AddPersistentListener(player.chat.onSubmit, HandleMessageEmote);
            }
        }
    }

#endif

    #endregion
}
