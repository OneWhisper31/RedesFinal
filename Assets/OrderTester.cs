using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using System.Linq;
public class OrderTester : NetworkBehaviour
{
    [Networked(OnChanged = nameof(OnChangeStay))]
    NetworkBool isInside { get; set; }


    [Networked, Capacity(5)]
    public NetworkArray<NetworkBool> plateIngredients { get; }

    Plate grabbedPlate;
    public CarBehaviour carBehaviour;

    [Networked]
    NetworkBool tested { get; set; }

    [Networked]
    NetworkBool wrongPlate { get; set; }
    static void OnChangeStay(Changed<OrderTester> changed)
    {
        var beh = changed.Behaviour;
        changed.LoadOld();
        var oldBeh = changed.Behaviour;
        beh.isInside = !oldBeh.isInside;
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
            if (player.CanGrabItem && player.GrabbedObject != null)
            {
                if (player.GrabbedObject.GetBehaviour<Plate>())
                {
                    grabbedPlate = player.GrabbedObject.GetBehaviour<Plate>();

                    if(tested == false)
                    {
                        TestOrder(grabbedPlate, carBehaviour, player);
                        tested = true;
                    }

                    if (Runner.IsServer)
                        RPC_SetPlatePosition(player);

                }
            }
        }
    }

    public void TestOrder(Plate food, CarBehaviour order, NetworkPlayer player)
    {

        for (int i = 0; i < order.ingredientsOrder.Count; i++)
        {
            if (food.myIngredients[i] != order.ingredientsOrder[i].Item2)
            {
                wrongPlate = true;
            }
        }
        if (wrongPlate)
        {
            //print("Wrong Order");
            order.animator.SetBool("hasMeal", true);
            StartCoroutine(GameManager.Instance.NextCar());

        }
        else
        {
            print(GameManager.Instance);
            GameManager.Instance.RPC_AddPoints(player.Object.InputAuthority, 1);

            StartCoroutine(GameManager.Instance.NextCar());
            order.animator.SetBool("hasMeal", true);

        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_SetPlatePosition(NetworkPlayer player)
    {
        Runner.Despawn(player.GrabbedObject);
        player.GrabbedObject = null;
    }
}
