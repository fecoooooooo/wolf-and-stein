using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food: Pickable
{
    public int amountToAdd = 10;

    public override void OnPickUp()
    {
        Character.instance.AddHP(amountToAdd);
    }

	public override bool ExtraCondition()
	{
		return base.ExtraCondition() && Character.instance.HP < Character.MAX_HP;
	}
}
