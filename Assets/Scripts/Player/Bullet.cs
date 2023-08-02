using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class Bullet : NetworkBehaviour
{
    [SerializeField] NetworkRigidbody _rgbd;

    [SerializeField] float _speed=25f,_dmg = 25f;

    void Start()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 3.7f;

        if (Object.HasStateAuthority)
            _rgbd.Rigidbody.AddForce(transform.forward * _speed, ForceMode.VelocityChange);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!Object || !Object.HasStateAuthority) return;

        if (other.TryGetComponent(out PlayerModel otherPlayer))
        {
            otherPlayer.TakeDamage(_dmg);
        }

        Runner.Despawn(Object);
    }
}
