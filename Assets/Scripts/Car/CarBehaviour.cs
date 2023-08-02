using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using Fusion;
public class CarBehaviour : NetworkBehaviour
{
    public NetworkBool hasOrdered;
    Animator animator;
    OrderObject myOrder;
    DisplayOrder myDisplayOrder;


    [SerializeField] GameObject Canvas;
    [SerializeField] GameObject Order;
    [SerializeField] List<Ingredient> myIngredients;
    [SerializeField] List<Tuple<Ingredient, NetworkBool>> ingredientsOrder;

    [Networked, Capacity(5)]
    public NetworkArray<NetworkBool> decidedIngredients { get; }

    void Start()
    {
        animator = GetComponent<Animator>();
        Initialize();
    }

    public CarBehaviour Initialize()
    {
        if(hasOrdered)
        {
            myDisplayOrder.gameObject.SetActive(true);
            RPC_GenerateRandomIngredients();
        }
        return this;
    }
    [Rpc(RpcSources.StateAuthority, RpcTargets.StateAuthority)]
    public void RPC_GenerateRandomIngredients()
    {
        decidedIngredients.Set(0, true);

        for (int i = 1; i < myIngredients.Count; i++)
        {
            decidedIngredients.Set(i , UnityEngine.Random.Range(0, 2) != 0);
        }
    }

    public void OrderFood()
    {
        if(NetworkPlayer.Local.HasStateAuthority)
        {
            RPC_OrderFood();
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_OrderFood()
    {
        hasOrdered = true;
        RPC_GenerateRandomIngredients();
        myOrder = Instantiate(Order).GetComponent<OrderObject>();
        myDisplayOrder = myOrder.GetComponentInChildren<DisplayOrder>();


        myOrder.transform.SetParent(Canvas.transform, false);
        Order.SetActive(true);
        ingredientsOrder = myOrder.GenerateOrder(myIngredients, decidedIngredients);

        myDisplayOrder.gameObject.SetActive(true);
        myDisplayOrder.DisplayOrders(ingredientsOrder);

    }

    void FoodOrdered()
    {
        animator.SetBool("isMealOrdered", true);
    }

}
