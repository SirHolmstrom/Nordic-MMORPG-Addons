using UnityEngine;
using UnityEngine.UI;

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

public partial class UIChat
{
    public void AddEmoteMessage(ChatMessage message)
    {
        // delete old messages so the UI doesn't eat too much performance.
        // => every Destroy call causes a lag because of a UI rebuild
        // => it's best to destroy a lot of messages at once so we don't
        //    experience that lag after every new chat message
        if (content.childCount >= keepHistory)
        {
            for (int i = 0; i < content.childCount / 2; ++i)
                Destroy(content.GetChild(i).gameObject);
        }

        // instantiate and initialize text prefab with parent
        GameObject go = Instantiate(message.textPrefab, content.transform, false);
        go.GetComponent<Text>().text = message.message;
        go.GetComponent<UIChatEntry>().message = message;

        AutoScroll();
    }
}