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
    
    const float TARGET_REACHED_DISTANCE = .01f;
    bool hasTarget = false;
    Vector3 currentTarget;
    Vector3? directionAfterReachedTarget;
    
    public float idleTimeLeft = 0;

    Transform targetDebug;


    void Start()
    {
        targetDebug = GameObject.CreatePrimitive(PrimitiveType.Sphere).transform;
        targetDebug.localScale = new Vector3(.2f, .2f, .2f);
        targetDebug.GetComponent<MeshRenderer>().material = Resources.Load<Material>("DebugMaterial");
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
        if (!hasTarget && Random.Range(0f, 1f) < IdleChance)
            idleTimeLeft = IdleTime;
	}

	private void Move()
	{
        RaycastHit hitInfo;
        if(!hasTarget && !chasing && Physics.Raycast(new Ray(transform.position, transform.forward), out hitInfo, MinDistanceToOtherColliders))
        {
            Door door = hitInfo.transform.GetComponent<Door>();
            
            if(door != null)
			{
                door.Open();
                hasTarget = true;
                currentTarget =  door.GetFartherPoint(transform.position);
                directionAfterReachedTarget = door.GetRotationAfterPass(transform.position);
            }
			else
			{
                int direction = Random.Range(0, 4);
                transform.rotation = Quaternion.Euler(0, direction * 90, 0);
			}
		}

        if(hasTarget)
		{
            transform.LookAt(currentTarget);
            if (Vector3.Distance(transform.position, currentTarget) < TARGET_REACHED_DISTANCE)
			{
                if (directionAfterReachedTarget.HasValue)
                    transform.rotation = Quaternion.Euler(directionAfterReachedTarget.Value);

                hasTarget = false;
                directionAfterReachedTarget = null;
            }
		}

        targetDebug.position = hasTarget ? currentTarget : new Vector3(0, -100, 0);

        transform.position += transform.forward * MoveSpeed;
	}
}
