using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public partial class Player
{

    #region Experimental

    private static event Action<Player> OnLocalPlayerStarted;
    private static List<Action<Player>> pendingSubscriptions = new List<Action<Player>>();
    private static bool isInitialized = false;

    public static void SubscribeToOnLocalPlayerStarted(Action<Player> callback)
    {
        if (isInitialized)
        {
            OnLocalPlayerStarted += callback;
        }
        else
        {
            pendingSubscriptions.Add(callback);
        }
    }

    #endregion


    // would rather call this in OnStartLocalPlayer,
    // but it's already used and try to avoice core modification.
    public override void OnStartClient()
    {
        base.OnStartClient();

        if (isLocalPlayer)
        {
            #region Experimental

            // Add all pending subscriptions to the event
            foreach (var pending in pendingSubscriptions)
            {
                Debug.Log(pending.GetMethodInfo().Name);
                OnLocalPlayerStarted += pending;
            }

            pendingSubscriptions.Clear();

            // Set the flag and then invoke the event
            isInitialized = true;
            OnLocalPlayerStarted?.Invoke(this);

            #endregion
        }
    }
}

public partial class NetworkManagerMMO 
{
    // casting.
    public static new NetworkManagerMMO singleton => singleton;
}