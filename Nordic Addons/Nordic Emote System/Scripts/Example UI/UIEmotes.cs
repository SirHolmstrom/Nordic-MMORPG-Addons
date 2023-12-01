using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIEmotes : MonoBehaviour
{
    public GameObject panel;

    public Transform content;
    public Button EmoteButton;

    public KeyCode key = KeyCode.E;

    public void Init(ScriptableEmoteList emotes)
    {
        foreach(var emote in emotes.Emotes)
        {
            Button btn = Instantiate(EmoteButton, content);
            btn.onClick.AddListener(() => Player.localPlayer.emote.TryEmote(emote.identifier));

            if(btn.transform.GetChild(0).TryGetComponent(out TextMeshProUGUI text))
            {
                text.text = emote.identifier;
            }

            btn.gameObject.SetActive(true);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(key) && !UIUtils.AnyInputActive())
        {
            panel.SetActive(!panel.activeSelf);
        }
    }
}
