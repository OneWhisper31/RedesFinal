using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
public class NetworkPlayer : NetworkBehaviour
{
    public static NetworkPlayer Local { get; private set; }

    public event System.Action OnLeft = delegate { };

    public Transform GrabPos;
    public NetworkBool CanGrabItem = true;

    public NetworkObject GrabbedObject;

    NetworkMecanimAnimator Animator;

    public void GrabItem(NetworkObject Item)
    {
        if(GrabbedObject == null)
        {
            Animator.SetTrigger("Grab");
            if (Item.GetBehaviour<Ingredient>() != null)
                GrabbedObject = Runner.Spawn(Item);
            else GrabbedObject = Item;
            if(Runner.IsServer)
                RPC_SetIngPosition(GrabbedObject);

            // = Item;
        }
    }

    public void LeaveItem()
    {
        if (GrabbedObject != null)
        {
            if (GrabbedObject.GetBehaviour<Ingredient>() != null)
                Runner.Despawn(GrabbedObject);
            GrabbedObject = null;
            //RPC_LeaveItem(myGrabbedIngredient);
        }

    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_SetIngPosition(NetworkObject grab)
    {
        GrabbedObject = grab;
        GrabbedObject.transform.position = GrabPos.position;
        GrabbedObject.transform.parent = GrabPos;
    }

    public override void FixedUpdateNetwork()
    {
        if(GetInput(out NetworkInputData input))
        {
            if(input.isGrabPressed)
            {
                CanGrabItem = true;
            }
            else CanGrabItem = false;
        }


    }
    public override void Spawned()
    {
        CanGrabItem = true;
        Animator = GetBehaviour<NetworkMecanimAnimator>();
        //nickname = NicknameHandler.Instance.AddNickname(this);
        if (Object.HasInputAuthority)
        {
            Local = this;

            GetComponentInChildren<Renderer>().material.color = Color.blue;

            //nickname.UpdateText("Asd");

            print("[Custom Msg] Local Player Spawned");

            //RPC_SetNickname("PEPE" + Random.Range(0, 1000));
        }
        else
        {
            GetComponentInChildren<Renderer>().material.color = Color.red;

            print("[Custom Msg] Remote Player Spawned");
        }

    }

    /*[Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    void RPC_SetNickname(string nick, RpcInfo info = default)
    {
        NickName = nick;
    }

    static void OnNicknameChanged(Changed<NetworkPlayer> changed)
    {
        changed.Behaviour.UpdateNickName(changed.Behaviour.NickName.ToString());

    }

    void UpdateNickName(string name)
    {
        nickname.UpdateText(name);
    }*/

    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        OnLeft();
    }
}
