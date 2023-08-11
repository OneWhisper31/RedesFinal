using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using System;
public class PlateInteractor : NetworkBehaviour
{
    [Networked(OnChanged = nameof(OnChangeStay))]
    public NetworkBool isInside { get; set; }

    public Plate MyPlate;

    static void OnChangeStay(Changed<PlateInteractor> changed)
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
            MyPlate.InteractWithPlate(player);
        }
    }
}
