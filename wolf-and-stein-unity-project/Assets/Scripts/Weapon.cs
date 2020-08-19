using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    Animator animator;
    float Speed = 0;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
		if (Input.GetKeyDown(KeyCode.Space))
		{
            Speed = 1;
            animator.SetFloat("Speed", Speed);
		}   
    }

    public void OnReachAnimEnd()
	{
        Speed = 0;
        animator.SetFloat("Speed", Speed);
    }
}
