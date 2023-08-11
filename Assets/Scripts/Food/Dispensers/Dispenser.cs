using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using UnityEngine.UI;
public class Dispenser : NetworkBehaviour
{
    public GameObject ingredientPrefab;
    public Ingredient myIngredient;
    public GameObject image;
    public GameObject canvas;
    Hitbox hitbox;

    Sprite ingredientImage;

    [Networked(OnChanged = nameof(OnChangeStay))]
    NetworkBool isInside { get; set; }


    public override void Spawned()
    {
        ingredientImage = myIngredient.image;
        image.GetComponent<Image>().sprite = ingredientImage;
    }
    static void OnChangeStay(Changed<Dispenser> changed)
    {
        var beh = changed.Behaviour;
        changed.LoadOld();
        var oldBeh = changed.Behaviour;
        beh.isInside = !oldBeh.isInside;
        beh.RPC_Display();
    }
    void RPC_Display()
    {
        canvas.SetActive(isInside);
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
        if(player != null)
        {
            if (player.CanGrabItem)
            {
                player.GrabItem(myIngredient);
            }
            
        }
    }
}

