using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : Pickable
{
    public override void OnPickUp()
    {
        Character.instance.AddKey(1);
    }

}
