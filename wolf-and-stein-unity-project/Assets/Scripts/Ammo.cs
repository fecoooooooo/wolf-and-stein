using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ammo : Pickable
{
    public int ammoToAdd = 5;

    public override void OnPickUp()
    {
        Character.instance.AddAmmo(ammoToAdd);
    }

}
