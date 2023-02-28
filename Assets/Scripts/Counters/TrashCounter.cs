using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashCounter : BaseCounter
{
    public static event EventHandler OnAnyObjectsTrashed;
    new public static void ResetStaticData()
    {
        OnAnyObjectsTrashed = null;
    }
    public override void Interact(Player player)
    {
        if (player.HasKitchenObject())
        {
            player.GetKitchenObject().DestroySelf();
            OnAnyObjectsTrashed?.Invoke(this, EventArgs.Empty);
        }
    }
}