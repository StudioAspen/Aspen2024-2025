using KBCore.Refs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody rigidBody;
    [SerializeField, Self] private CapsuleCollider capsuleCollider;
    [SerializeField] private HitNumbers hitNumberPrefab;
    /*Temp*/
    [SerializeField] public BaseEnemy enemyStats;
    [SerializeField] public EnemyAgent agent;

    [SerializeField] private LayerMask groundLayer;
    [HideInInspector] public bool IsGrounded = true;

    private void OnValidate()
    {
        this.ValidateRefs();
    }

    private void Awake()
    {
        agent = GameObject.FindObjectOfType<EnemyAgent>();
        enemyStats = GameObject.FindObjectOfType<BaseEnemy>();
        IgnoreCollisionsWithSelf();
    }

    private void Update()
    {
        CheckGrounded();

        HandleAnimations();

        if (Input.GetKeyDown(KeyCode.F)) LaunchUpwards(10f);
    }

    private void CheckGrounded()
    {
        IsGrounded = Physics.CheckSphere(transform.position, capsuleCollider.radius, groundLayer);
    }

    private void HandleAnimations()
    {
        //animator.SetBool("IsGrounded", IsGrounded);
    }

    private void IgnoreCollisionsWithSelf()
    {
        Collider[] colliders = GetComponents<Collider>();

        foreach(Collider c1 in colliders)
        {
            foreach(Collider c2 in colliders)
            {
                Physics.IgnoreCollision(c1, c2);
            }
        }
    }

    public void TakeDamage(int damage, Vector3 hitPoint)
    {
        //animator.CrossFadeInFixedTime("Hit", 0.1f);

        HitNumbers hitNumber = Instantiate(hitNumberPrefab, hitPoint, Quaternion.identity);
        hitNumber.ActivateHitNumberText(damage);

        //Temp
        EnemyHitState enemyGotHit = agent.movementStateMachine.GetState(EnemyMovementStateId.Hit) as EnemyHitState;
        //agent.movementStateMachine.ChangeState(EnemyMovementStateId.Hit);
        enemyStats.enemyCurrentHP -= damage;
        LaunchUpwards(0);
    }

    public void LaunchUpwards(float magnitude)
    {
        rigidBody.AddForce(magnitude * Vector3.up, ForceMode.Impulse);
    }
}
