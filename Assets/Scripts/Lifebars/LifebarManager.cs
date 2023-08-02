using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class LifebarManager : MonoBehaviour
{
    [SerializeField] Lifebar _lifebarPrefab;

    public event Action UpdateBars = delegate { };

    public void SpawnBar(PlayerModel target)
    {
        Lifebar lifebar = Instantiate(_lifebarPrefab, target.transform.position, Quaternion.identity, transform)
                          .SetTarget(target);

        UpdateBars += lifebar.UpdatePosition;

        target.OnPlayerDestroyed += () =>
        {
            UpdateBars -= lifebar.UpdatePosition;

            Destroy(lifebar.gameObject);
        };
    }

    void LateUpdate()
    {
        UpdateBars();
    }
}
