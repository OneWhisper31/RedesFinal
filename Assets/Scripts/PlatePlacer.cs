using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using System;

public class PlatePlacer : NetworkBehaviour
{
    public Transform placePoint;

    [Networked(OnChanged = nameof(OnChangeStay))]
    NetworkBool isInside { get; set; }

    static void OnChangeStay(Changed<PlatePlacer> changed)
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
                if(player.GrabbedObject.GetBehaviour<Plate>())
                {
                    if(Runner.IsServer)
                        RPC_SetPlatePosition(player);
                }
            }
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_SetPlatePosition(NetworkPlayer player)
    {
        player.GrabbedObject.transform.position = placePoint.position;
        player.GrabbedObject.transform.parent = null;
        player.GrabbedObject = null;
        Object.gameObject.SetActive(false);
    }
}
