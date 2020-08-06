using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float MaxHP = 100f;
    public float MoveSpeed = .01f;
    public float MinDistanceToOtherColliders = .5f;
    public float IdleTime = 5f;
    public float IdleChance = 0.002f;

    bool chasing = false;
    public float idleTimeLeft = 0;


    void Start()
    {
        
    }

    void FixedUpdate()
    {
        if (idleTimeLeft > 0) 
        {
            idleTimeLeft -= Time.deltaTime;
            return;
        }


        TryToIdle();
        Move();
        
    }

	private void TryToIdle()
	{
        if (Random.Range(0f, 1f) < IdleChance)
		{
            idleTimeLeft = IdleTime;
            Debug.Log("Idling");
		}
	}

	private void Move()
	{
        if(!chasing && Physics.Raycast(new Ray(transform.position, transform.forward), MinDistanceToOtherColliders))
        {
            int direction = UnityEngine.Random.Range(0, 4);
            transform.rotation = Quaternion.Euler(0, direction * 90, 0);
		}

        transform.position += transform.forward * MoveSpeed;
	}
}
