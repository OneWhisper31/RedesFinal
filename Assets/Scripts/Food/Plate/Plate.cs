using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using System;
using System.Linq;

public class Plate : NetworkBehaviour
{


    /*1-bread
      2-cheese
      3-drink
      4-Lettuce
      5-Tomatoe
     */

    [Networked, Capacity(5)]
    public NetworkArray<NetworkBool> myIngredients { get; }
    
    public NetworkObject myPlatePlacer;


    public void InteractWithPlate(NetworkPlayer player)
    {
        if (player.CanGrabItem && player.GrabbedObject != null)
        {
            if (player.GrabbedObject.GetBehaviour<Ingredient>() != null)
            {
                CheckFood(player.GrabbedObject.GetBehaviour<Ingredient>().type);
                player.LeaveItem();
            }
        }
        else if (player.CanGrabItem && player.GrabbedObject == null)
        {
            myPlatePlacer.gameObject.SetActive(true);
            player.GrabItem(Object);
        }
    }

    void CheckFood(IngredientType ingType)
    {
        switch (ingType)
        {
            case IngredientType.Bread:
                myIngredients.Set(0, true);
                break;
            case IngredientType.Cheese:
                myIngredients.Set(1, true);
                break;
            case IngredientType.Drink:
                myIngredients.Set(2, true);
                break;
            case IngredientType.Lettuce:
                myIngredients.Set(3, true);
                break;
            case IngredientType.Tomatoe:
                myIngredients.Set(4, true);
                break;
        }
    }
}
