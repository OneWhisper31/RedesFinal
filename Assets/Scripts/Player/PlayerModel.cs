using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class PlayerModel : NetworkBehaviour
{
    [SerializeField] NetworkRigidbody _rgbd;
    [SerializeField] Animator _animator;
    [SerializeField] Bullet _bulletPrefab;
    [SerializeField] ParticleSystem _shootParticle;
    [SerializeField] GameObject _shield;
    [SerializeField] Transform _firePosition;

    [Networked(OnChanged = nameof(LifeChangedCallback))]
    [SerializeField] float _life { get; set; }
    [SerializeField] float _speed;
    [SerializeField] float _jumpForce, _protectCooldown;
    [SerializeField] float _maxlife { get; set; }

    float _lastFireTime,_lastProtectTime;
    
    [Networked(OnChanged = nameof(ShootChangedCallback))]
    bool IsFiring { get; set; }


    [Networked(OnChanged = nameof(ProtectChangedCallback))]
    bool IsProtecting { get; set; }

    int _previousSign, _currentSign;

    //Action de muerte que vamos a usar para que el Handler/Manager de vidas flotantes se registre.
    public event Action OnPlayerDestroyed = delegate { };

    //Action de actuaizacion de vida que vamos a usar para que el Handler/Manager de vidas flotantes se registre.
    public event Action<float> OnLifeUpdate = delegate { };

    void Start()
    {
        transform.forward = Vector3.right;
        _maxlife = _life;
    }
    public void ResetLife()
    {
        _life = _maxlife;
    }

    public override void Spawned()
    {
        _life = 100;

        var lifeManager = FindObjectOfType<LifebarManager>();

        lifeManager.SpawnBar(this);

    }

    public override void FixedUpdateNetwork()
    {
        if (Time.timeScale == 0)
            return;
        if (GetInput(out NetworkInputData networkInputData))
        {
            Movement(networkInputData.movementInput);
        }

        if (networkInputData.isFirePressed)
        {
            Shoot();
        }

        if (networkInputData.isJumpPressed)
        {
            Jump();
        }

        if (networkInputData.isProtectPressed)
        {
            Protect();
        }
    }

    private void Movement(float xAxi)
    {
        if (xAxi != 0)
        {
            _rgbd.Rigidbody.MovePosition(transform.position + Vector3.right * xAxi * _speed * Time.fixedDeltaTime);

            _currentSign = (int)Mathf.Sign(xAxi);

            if (_previousSign != _currentSign)
            {
                _previousSign = _currentSign;

                transform.rotation = Quaternion.Euler(Vector3.up * 90 * _currentSign);
            }

            _animator.SetFloat("MovementValue", Mathf.Abs(xAxi));
        }
        else if (_currentSign != 0)
        {
            _currentSign = 0;
            _animator.SetFloat("MovementValue", 0);
        }
    }

    private void Jump()
    {
        _rgbd.Rigidbody.AddForce(Vector3.up * _jumpForce, ForceMode.VelocityChange);
    }

    private void Shoot()
    {
        if (Time.time - _lastFireTime < 0.15f) return;

        _lastFireTime = Time.time;

        StartCoroutine(Shoot_Cooldown());

        Runner.Spawn(_bulletPrefab, _firePosition.position, transform.rotation);
        
    }

    IEnumerator Shoot_Cooldown()
    {
        //WaitForSeconds variable = new WaitForSeconds(0.15f);

        IsFiring = true;

        yield return new WaitForSeconds(0.15f);

        IsFiring = false;
    }

    private void Protect()
    {
        if (Time.time - _lastProtectTime < _protectCooldown) return;

        _lastProtectTime = Time.time;

        StartCoroutine(Protect_Cooldown());
    }

    IEnumerator Protect_Cooldown()
    {
        IsProtecting = true;

        yield return new WaitForSeconds(_protectCooldown);

        IsProtecting = false;
    }
    
    static void ShootChangedCallback(Changed<PlayerModel> changed)
    {
        //Guardo el valor de IsFiring del frame actual
        bool updatedShoot = changed.Behaviour.IsFiring;

        //Cargo los datos del frame anterior
        changed.LoadOld();

        //Guardo el valor de IsFiring del frame anterior
        bool previousShoot = changed.Behaviour.IsFiring;

        if (!previousShoot && updatedShoot)
        {
            changed.Behaviour._shootParticle.Play();
        }
    }
    static void ProtectChangedCallback(Changed<PlayerModel> changed)
    {
        //actualizco la visibilidad en base al nuevo valor
        changed.Behaviour._shield.SetActive(changed.Behaviour.IsProtecting);
    }

    public void TakeDamage(float dmg)
    {
        RPC_GetHit(dmg);
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    void RPC_GetHit(float dmg)
    {
        _life -= dmg;

        if (_life <= 0)
        {
            Dead();
        }
    }

    static void LifeChangedCallback(Changed<PlayerModel> changed)
    {
        changed.Behaviour.OnLifeUpdate(changed.Behaviour._life / 100f);
    }

    public void OnDisconected()
    {
        Runner.Despawn(this.Object);
        //Runner.Shutdown();
        Debug.LogWarning("Player Desconected");
    }
    void Dead()
    {
        GameManager.GM.RPC_OnEnd(Runner.GetPlayerUserId());
    }

    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        OnPlayerDestroyed();
    }
}
