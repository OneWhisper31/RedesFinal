using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using Fusion;
public class CarBehaviour : NetworkBehaviour
{
    public NetworkBool hasOrdered;
    public Animator animator;

    [SerializeField] NetworkObject DeliverHitbox;
    [SerializeField] DisplayOrder myDisplayOrder;
    [SerializeField] GameObject Canvas;
    [SerializeField] NetworkObject Order;
    [SerializeField] List<Ingredient> myIngredients;
    [SerializeField] public List<Tuple<Ingredient, NetworkBool>> ingredientsOrder;

    [Networked, Capacity(5)]
    public NetworkArray<NetworkBool> decidedIngredients { get; }

    public override void Spawned()
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

        if (Runner.IsServer)
        {
            myDisplayOrder = Runner.Spawn(Order).GetBehaviour<DisplayOrder>();
            RPC_OrderFood(myDisplayOrder);
        }
        StartCoroutine(FoodOrdered());
        //GameManager.Instance.SetPlatesRecipe(ingredientsOrder);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_OrderFood(DisplayOrder order)
    {
        hasOrdered = true;
        RPC_GenerateRandomIngredients();

        order.Initialize(this);

        order.transform.SetParent(Canvas.transform, false);
        order.gameObject.SetActive(true);
        ingredientsOrder = GenerateOrder(myIngredients, decidedIngredients);
        

        order.gameObject.SetActive(true);
        order.RPC_DisplayOrders(this);

    }
    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();

        if (myDisplayOrder != null)
            myDisplayOrder.transform.LookAt(Camera.main.transform);
    }

    public List<Tuple<Ingredient, NetworkBool>> GenerateOrder(List<Ingredient> ingredients, NetworkArray<NetworkBool> decidedIngredients)
    {

        var order = new List<Tuple<Ingredient, NetworkBool>>();

        for (int i = 0; i < ingredients.Count; i++)
        {
            order.Add(Tuple.Create(ingredients[i], decidedIngredients[i]));
        }
        return order;
    }

    IEnumerator FoodOrdered()
    {
        yield return new WaitForSeconds(5f);
        animator.SetBool("isMealOrdered", true);
    }
    public void OnWindow()
    {
        DeliverHitbox.gameObject.SetActive(true);
    }
    public void OnReset()
    {
        animator.SetBool("isMealOrdered", false);
        animator.SetBool("hasMeal", false);
        Object.gameObject.SetActive(false);
        //Initialize();
    }
}
