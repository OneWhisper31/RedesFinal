using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fusion;
public abstract class Ingredient : NetworkBehaviour
{
    public new string name;
    public int id;
    public Sprite image;
}
