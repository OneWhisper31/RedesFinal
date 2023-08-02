using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fusion;
using System;
public class DisplayOrder : NetworkBehaviour
{
    public NetworkObject IngredientDisplay;
    public Transform displayPlace;

    //[Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void DisplayOrders(List<Tuple<Ingredient, NetworkBool>> ingredients)
    {
        foreach (var ingredient in ingredients)
        {
            if(ingredient.Item2)
            {
                print(ingredient.Item1);
                var ing = FindObjectOfType<NetworkRunner>().Spawn(IngredientDisplay);
                ing.transform.parent = displayPlace;
                IngredientDisplay.GetComponent<Image>().sprite = ingredient.Item1.image;
            }
        }
    }
}
