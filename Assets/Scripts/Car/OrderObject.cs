using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using Fusion;
public class OrderObject : NetworkBehaviour
{                                   

    public List<Tuple<Ingredient, NetworkBool>> GenerateOrder(List<Ingredient> ingredients, NetworkArray<NetworkBool> decidedIngredients)
    {

        var order = new List<Tuple<Ingredient, NetworkBool>>();

        for(int i = 0; i < ingredients.Count; i++)
        {
            order.Add(Tuple.Create(ingredients[i], decidedIngredients[i]));
        }
        return order;
    }
}


