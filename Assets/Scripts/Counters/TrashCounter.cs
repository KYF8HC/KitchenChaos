using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashCounter : BaseCounter
{
    public static event EventHandler OnAnyObjectsTrashed; 
    public override void Interact(Player player)
    {
        if(player.HasKitchenObject())
        {
            player.GetKitchenObject().DestroySelf();
            OnAnyObjectsTrashed?.Invoke(this, EventArgs.Empty);
        }
    }
}
