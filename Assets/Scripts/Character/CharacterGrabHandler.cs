using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using System;
public class CharacterGrabHandler : NetworkBehaviour
{
    GameObject myGrabbedItem;
    // Update is called once per frame
    public override void FixedUpdateNetwork()
    {
        if(GetInput(out NetworkInputData networkInput))
        {
            if(networkInput.isGrabPressed)
            {
                OnGrabItem();
            }
        }
    }

    public void OnGrabItem()
    {
       
    }
}
