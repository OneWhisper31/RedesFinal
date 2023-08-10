using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fusion;
using System;
public class DisplayOrder : NetworkBehaviour
{
    public GameObject IngredientDisplay;
    public Transform DisplayPlace;
    public CarBehaviour MyDaddyBehaviour;

    public DisplayOrder Initialize(CarBehaviour car)
    {
        MyDaddyBehaviour = car;
        return this;
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_DisplayOrders(CarBehaviour car)
    {
        foreach (var ingredient in car.ingredientsOrder)
        {
            if (ingredient.Item2)
            {
                var ing = Instantiate(IngredientDisplay);
                ing.transform.SetParent(DisplayPlace, false);
                ing.GetComponent<Image>().sprite = ingredient.Item1.image;
            }
        }           
    }


}
