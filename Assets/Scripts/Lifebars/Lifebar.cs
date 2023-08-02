using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Lifebar : MonoBehaviour
{
    Transform _target;

    [SerializeField] float _yOffset = 3;

    [SerializeField] Image _myImage;

    public Lifebar SetTarget(PlayerModel target)
    {
        _target = target.transform;

        target.OnLifeUpdate += UpdateLifeBar;
        //target.OnPlayerDestroyed += () => Destroy(gameObject);

        return this;
    }

    void UpdateLifeBar(float amount)
    {
        _myImage.fillAmount = amount;
    }

    public void UpdatePosition()
    {
        transform.position = _target.position + Vector3.up * _yOffset;
    }
}
