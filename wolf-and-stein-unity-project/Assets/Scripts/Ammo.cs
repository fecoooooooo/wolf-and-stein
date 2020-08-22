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

    public override bool ExtraCondition()
    {
        return base.ExtraCondition() && Character.instance.Ammo < Character.MAX_AMMO;
    }
}
