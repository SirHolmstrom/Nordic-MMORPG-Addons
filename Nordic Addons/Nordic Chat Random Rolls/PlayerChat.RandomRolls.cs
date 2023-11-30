using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Text;
using Codice.Client.Common.GameUI;
using UnityEditor.Events;

public partial class PlayerChat
{
    public const string rollText = "{NAME} " + "rolled " + "{VALUE}";

    // message we check
    #region message check for logic

    // the logic that will look for /roll command
    public void CheckForRandomRollMessage(string message)
    {
        if (message.ToLower().StartsWith("/roll"))
        {
            TryRoll(message);
        }

        else if (message.ToLower().Equals("/coinflip") || message.ToLower().StartsWith("/side") || message.ToLower().StartsWith("/flip"))
        {
            TryCoinFlip(message);
        }
    }

    #endregion

    // roll logic.
    #region roll

    private void TryRoll(string text)
    {
        string[] to = text.Split(" ");

        // this means the user tries to do more than one tosses, capped to 5 by default.
        if (to.Length == 4)
        {
            int min = 0;
            int max = 0;
            int loops = 0;

            bool acceptedInput = int.TryParse(to[1], out min) && int.TryParse(to[2], out max) && int.TryParse(to[3], out loops);

            if (acceptedInput)
            {
                if (max <= 1)
                {
                    CmdMsgRandomRoll(1, 100);
                }

                else if (loops <= 5)
                {
                    for (int i = 0; i < loops; i++)
                    {
                        CmdMsgRandomRoll(min, max);
                    }
                }

                else
                {
                    UIChat.singleton.AddMessage(new ChatMessage("", infoChannel.identifierIn, "Use /roll <MIN> <MAX> <TIMES(5max)> ", "", infoChannel.textPrefab));
                }
            }
            else
            {
                UIChat.singleton.AddMessage(new ChatMessage("", infoChannel.identifierIn, "Use /roll <MIN> <MAX> <TIMES(5max)> ", "", infoChannel.textPrefab));
            }

        }

        else if (to.Length == 3)
        {
            int min = 0;
            int max = 0;

            bool acceptedInput = int.TryParse(to[1], out min) && int.TryParse(to[2], out max);

            if (acceptedInput)
            {
                if (max <= 1)
                {
                    CmdMsgRandomRoll(1, 100);
                }

                CmdMsgRandomRoll(min, max);
            }

            else
            {
                UIChat.singleton.AddMessage(new ChatMessage("", infoChannel.identifierIn, "Use /roll <MIN> <MAX> <TIMES(5max)>  ", "", infoChannel.textPrefab));
            }
        }

        else if (to.Length == 2)
        {
            int max = 0;

            bool acceptedInput = int.TryParse(to[1], out max);

            if (acceptedInput)
            {
                CmdMsgRandomRoll(1, max);
            }

            else
            {
                UIChat.singleton.AddMessage(new ChatMessage("", infoChannel.identifierIn, "Use /roll <MIN> <MAX> <TIMES(5max)>  ", "", infoChannel.textPrefab));
            }
        }

        else if (to.Length == 1)
        {
            CmdMsgRandomRoll(1, 100);
        }

        else
        {
            UIChat.singleton.AddMessage(new ChatMessage("", infoChannel.identifierIn, "Use /roll <MIN> <MAX> <TIMES(5max)>  ", "", infoChannel.textPrefab));
        }

    }

    // send roll request to server, server then rolls.
    [Command]
    private void CmdMsgRandomRoll(int min, int max)
    {
        if (max <= 1) max = 100;

        else if (max > int.MaxValue) max = int.MaxValue;

        RpcMsgRoll(RandomRoll(min, max), min, max);
    }

    // Server tells clients results and our min max.
    [ClientRpc]
    private void RpcMsgRoll(int value, int min, int max)
    {
        RollMsgOnClients(value, min, max);
    }

    // the clients get's to deal with the message overhead,
    // sending string over network is not optimal.
    [Client]
    private void RollMsgOnClients(int value, int min, int max)
    {
        StringBuilder tip = new StringBuilder(rollText);
        tip.Replace("{NAME}", name);
        tip.Replace("{VALUE}", value.ToString());

        string message = tip + " (" + min + "-" + max + ")";

        UIChat.singleton.AddMessage(new ChatMessage("Server", infoChannel.identifierIn, message, "", infoChannel.textPrefab));
    }

    #endregion

    // side flip logic
    #region coinflip
    private void TryCoinFlip(string text)
    {
        string[] to = text.Split(" ");

        if (to.Length == 2)
        {
            int loops = 0;

            bool acceptedInput = int.TryParse(to[1], out loops);

            if (acceptedInput)
            {
                if (loops > 10)
                {
                    UIChat.singleton.AddMessage(new ChatMessage("", infoChannel.identifierIn, "Use /coinflip <AMOUNT(10max)>  ", "", infoChannel.textPrefab));
                    return;
                }

                else
                {
                    CmdMsgGambaFlip(loops);
                }
            }
        }

        else
        {
            CmdMsgGambaFlip(1);
        }
    }

    [Command]
    private void CmdMsgGambaFlip(int loops)
    {
        if (loops > 1)
        {
            int headCount = 0;
            int tailsCount = 0;

            for (int i = 0; i < loops; i++)
            {
                // do a coinflip
                COINSIDERESULT side = CoinFlip;

                // id result was head, increment headCount and start over loop.
                if (side == COINSIDERESULT.HEAD)
                {
                    headCount++;
                    continue;
                }

                // otherwise we increment tails.
                tailsCount++;
            }

            RpcMsgFlip(headCount, tailsCount, loops);
        }

        else
        {
            RpcMsgFlip(CoinFlip);
        }
    }


    [ClientRpc]
    private void RpcMsgFlip(int headCount, int tailsCount, int loops)
    {
        string message = name + " toss (<b>" + loops + "</b>) coins up in the air landing on the ground, showing: (" + headCount + " heads) and (" + tailsCount + " tails)";

        UIChat.singleton.AddMessage(new ChatMessage("Server", infoChannel.identifierIn, message, "", infoChannel.textPrefab));

    }

    [ClientRpc]
    void RpcMsgFlip(COINSIDERESULT side)
    {
        string message = name + " flips a coin and it shows " + (side == COINSIDERESULT.HEAD ? "Heads" : "Tails");

        UIChat.singleton.AddMessage(new ChatMessage("Server", infoChannel.identifierIn, message, "", infoChannel.textPrefab));

    }


    #endregion

    // the methods the server use to roll. (Random.Range & Random.value)
    #region simple random rolls

    [ServerCallback]
    public int RandomRoll(int min, int max)
    {
        return Random.Range(min, max + 1);
    }

    public COINSIDERESULT CoinFlip => Random.value > 0.5f ? COINSIDERESULT.HEAD : COINSIDERESULT.TAIL;

    #endregion

#if UNITY_EDITOR
    private void OnValidate()
    {
        // adds it automatically.
        if (!onSubmit.ContainsPersistentListener("CheckForRandomRollMessage"))
        {
            UnityEventTools.AddPersistentListener(onSubmit, CheckForRandomRollMessage);
        }
    }
#endif
}

public enum COINSIDERESULT : byte { HEAD, TAIL }
