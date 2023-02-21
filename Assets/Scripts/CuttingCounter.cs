using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuttingCounter : BaseCounter
{
    [SerializeField] private CuttingRecipeSO[] cuttingRecipeSOArray;
    public override void Interact(Player player)
    {
        if(!HasKitchenObject())
        {
            //There is no KitchenObject here
            if(player.HasKitchenObject())
            {
                //Player is carrying something
                if(HasRecipeWithInput(player.GetKitchenObject().GetKitchenObjectSO())){
                    //Player carrying something that we can cut
                    player.GetKitchenObject().SetKitchenObjectParent(this);
                }
            }
        }
        else
        {
            //There is a KitchenObject here
            if(!player.HasKitchenObject())
            {
                //Player is not carrying anything
                GetKitchenObject().SetKitchenObjectParent(player);
            }
        }
    }
    public override void InteractAlternate(Player player)
    {
        if(HasKitchenObject() && HasRecipeWithInput(GetKitchenObject().GetKitchenObjectSO()))
        {
            //We are only cutting if there is something on the counter AND we can cut it
            KitchenObjectSO outputKitchenObjectSO = GetOutputForInput(GetKitchenObject().GetKitchenObjectSO());
            GetKitchenObject().DestroySelf();
            KitchenObject.SpawnKitchenObject(outputKitchenObjectSO, this);
        }
    }
    private bool HasRecipeWithInput(KitchenObjectSO inputKitchenObjectSO)
    {
        foreach (CuttingRecipeSO cuttingRecipeSO in cuttingRecipeSOArray)
        {
            if(cuttingRecipeSO.input == inputKitchenObjectSO)
            {
                return true;
            }
        }
        return false;
    }
    private KitchenObjectSO GetOutputForInput(KitchenObjectSO inputKitchenObjectSO){
        foreach (CuttingRecipeSO cuttingRecipeSO in cuttingRecipeSOArray)
        {
            if(cuttingRecipeSO.input == inputKitchenObjectSO)
            {
                return cuttingRecipeSO.output;
            }
        }
        return null;
    }
}
