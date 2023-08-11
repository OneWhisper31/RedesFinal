using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using System;
using System.Linq;

public class Plate : NetworkBehaviour
{
    public Tuple<Ingredient, NetworkBool>[] listOrder;//itemsRequired

    [Networked(OnChanged = nameof(OnChangeStay))]
    NetworkBool isInside { get; set; }

    static void OnChangeStay(Changed<Plate> changed)
    {
        var beh = changed.Behaviour;
        changed.LoadOld();
        var oldBeh = changed.Behaviour;
        beh.isInside = !oldBeh.isInside;
    }


    //[Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void SetPlateIngredients(List<Tuple<Ingredient, NetworkBool>> list)
    {
        //si todos se vuelven true, se genera el plato
        listOrder = list.Where(x => x.Item2).Select(x=>Tuple.Create(x.Item1,(NetworkBool)false)).ToArray();

        foreach (var item in listOrder)
        {
            Debug.Log(item.Item1.name);
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_OnAddIngredient(Ingredient ingredient)
    {
        if (ingredient==null)
            return;

        if (listOrder.Any(x => ingredient.type == x.Item1.type))
        {
            listOrder= listOrder.Where(x => ingredient.type != x.Item1.type)
                                .Append(Tuple.Create(ingredient, (NetworkBool)false)).ToArray();

            Debug.Log("Añado");

            if (listOrder.All(x => x.Item2))
                Debug.Log("Plato Terminado");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        var player = other.GetComponent<NetworkPlayer>();
        if (player != null)
        {
            isInside = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        var player = other.GetComponent<NetworkPlayer>();
        if (player != null)
        {
            isInside = false;
        }
    }
    void OnTriggerStay(Collider other)
    {
        var player = other.GetComponent<NetworkPlayer>();
        if (player != null)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                if(Runner.IsServer)
                    RPC_OnAddIngredient(player.myGrabbedIngredient);

               // player.myGrabbedIngredient = null;
            }

        }
    }
}
