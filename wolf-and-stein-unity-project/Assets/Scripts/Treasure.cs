using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Treasure : Pickable
{
    public int amountToAdd = 100;

    public override void OnPickUp()
    {
        Character.instance.AddScore(amountToAdd);
    }

}
