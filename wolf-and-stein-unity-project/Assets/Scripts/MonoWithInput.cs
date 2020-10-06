using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class MonoWithInput : MonoBehaviour
{
    protected bool inputActive;

    public virtual void Update()
    {
        if (inputActive)
            HandleInput();
    }

    public abstract void HandleInput();

    public void EnableInput()
	{
        StartCoroutine(SetInput(true));
	}

    public void DisableInput()
    {
        StartCoroutine(SetInput(false));
    }

    IEnumerator SetInput(bool inputActive)
	{
        yield return null;
        this.inputActive = inputActive;
	}
}