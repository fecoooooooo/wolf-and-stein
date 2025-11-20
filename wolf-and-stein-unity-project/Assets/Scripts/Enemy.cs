using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float MaxHP = 100f;
    public float OffsetForShooting = .3f;

    public float MoveSpeed = .01f;
    public float MinDistanceToOtherColliders = .5f;
    public float IdleTime = 5f;
    public float IdleChance = 0.002f;

    float currentHp;

    const float TARGET_REACHED_DISTANCE = .01f;
    bool hasTarget = false;
    Vector3 currentTarget;
    Vector3? directionAfterReachedTarget;

    public float idleTimeLeft = 0;

    Transform targetDebug;
    Transform visuals;
    BoxCollider myCollider;
    
    Animator animator;

    EnemyState state;

    float timeToLeaveIdle;

    void Start()
    {
        currentHp = MaxHP;

        visuals = transform.Find("Visuals");
        myCollider = GetComponent<BoxCollider>();

        animator = GetComponentInChildren<Animator>();
        timeToLeaveIdle = animator.runtimeAnimatorController.animationClips.FirstOrDefault(a => a.name == "EnemyIdleLeave").length * 2;

        targetDebug = GameObject.CreatePrimitive(PrimitiveType.Sphere).transform;
        targetDebug.GetComponent<SphereCollider>().enabled = false;
        targetDebug.localScale = new Vector3(.2f, .2f, .2f);
        targetDebug.GetComponent<MeshRenderer>().material = Resources.Load<Material>("DebugMaterial");
    }

    void FixedUpdate()
    {
		HandleIdle();

		switch (state)
        {
            case EnemyState.Idle:
                Idle();
                break;
            case EnemyState.IdleLeave:
                break;
            case EnemyState.Dead:
                return;
            case EnemyState.Attack:
                break;
            case EnemyState.Walk:
				Move();
                break;
            case EnemyState.WalkBack:
				Move();
                break;
            case EnemyState.WalkLeft:
				Move();
                break;
            case EnemyState.WalkRight:
				Move();
				break;
            default:
                break;
        }
        if (state == EnemyState.Dead)
            return;

        Attack();


    }

	private void Attack()
	{
		animator.SetTrigger("Attack");
	}

	private void HandleIdle()
    {
        if (state != EnemyState.Idle && !hasTarget && Random.Range(0f, 1f) < IdleChance)
		{
            state = EnemyState.Idle;
            idleTimeLeft = IdleTime;
            animator.SetTrigger("IdleEnter");
        }
    }

    private void Idle()
    {
		idleTimeLeft -= Time.deltaTime;
		if (state != EnemyState.IdleLeave && idleTimeLeft <= timeToLeaveIdle)
		{
			state = EnemyState.IdleLeave;
			animator.SetTrigger("IdleLeave");
		}
		if (idleTimeLeft <= 0)
			state = EnemyState.Walk;
	}

    private void Move()
    {
        SetRequiredWalkAnimation();

        RaycastHit hitInfo;
        if (!hasTarget && Physics.Raycast(new Ray(transform.position, transform.forward), out hitInfo, MinDistanceToOtherColliders))
        {
            Door door = hitInfo.transform.GetComponent<Door>();

            if (door != null)
            {
                door.Open();
                hasTarget = true;
                currentTarget = door.GetFartherPoint(transform.position);
                directionAfterReachedTarget = door.GetRotationAfterPass(transform.position);
            }
            else
            {
                int direction = Random.Range(0, 4);
                transform.rotation = Quaternion.Euler(0, direction * 90, 0);
            }
        }

        if (hasTarget)
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

	private void SetRequiredWalkAnimation()
	{
        EnemyState requiredState = CalculateRequiredStateViaWalking();
        if (state != requiredState)
        {
            animator.ResetTrigger("IdleLeave");
			switch (requiredState)
			{
				case EnemyState.Walk:
                    animator.SetTrigger("Walk");
					break;
				case EnemyState.WalkBack:
                    animator.SetTrigger("WalkBack");
                    break;
				case EnemyState.WalkLeft:
                    animator.SetTrigger("WalkLeft");
                    break;
				case EnemyState.WalkRight:
                    animator.SetTrigger("WalkRight");
                    break;
			}
            state = requiredState;
        }
    }

	private EnemyState CalculateRequiredStateViaWalking()
	{
        Vector3 characterToEnemy = (transform.position - Character.instance.transform.position).normalized;
        float angle = Vector3.SignedAngle(characterToEnemy, transform.forward, Vector3.up);
        //Debug.Log(angle);
        if (-45 <= angle && angle < 45)
            return EnemyState.WalkBack;
        else if (45 <= angle && angle < 135)
            return EnemyState.WalkRight;
        else if (-135 <= angle && angle < -45)
            return EnemyState.WalkLeft;
        else
            return EnemyState.Walk;
    }

	internal Vector3[] GetTargetPositions()
	{
        Vector3 offset = visuals.right * OffsetForShooting;

        Vector3[] targetPositions = { visuals.position - offset, visuals.position, visuals.position + offset };

        return targetPositions;
    }

	internal void TakeDamage(float damage)
	{
        if (state == EnemyState.Dead)
            return;

        currentHp -= damage;

        Debug.Log($"{name} hp: {currentHp}");

        if (currentHp <= 0)
            Die();
        else
            animator.SetTrigger("TakeDamage");
    }

	private void Die()
	{
        state = EnemyState.Dead;
        animator.SetTrigger("Die");
        myCollider.enabled = false;
        Map.instance.SpawnLootFromCorpse(transform.position);
	}

	enum EnemyState
	{
        Idle,
        IdleLeave,
        Dead,
        Attack,
        Walk,
        WalkBack,
        WalkLeft,
        WalkRight,
	}
}
