using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public Animator animator;

    public NavMeshAgent agent;

    private Player player;

    public LayerMask whatIsGround, whatIsPlayer;

    public float health;

    // patroling
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    // attacking
    public float timeBetweenAttack;
    bool alreadyAttacked;

    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;

    public AudioClip shotgunSound;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }

    // Update is called once per frame
    private void Update()
    {
        Vector3 forwardInSight = transform.localRotation * Vector3.forward * sightRange;
        Vector3 forwardInAttack = transform.localRotation * Vector3.forward * attackRange;

        playerInSightRange = Physics.CheckBox(transform.position + forwardInSight / 2, new Vector3(sightRange / 2, sightRange / 2, sightRange / 2), transform.rotation, whatIsPlayer);
        playerInAttackRange = Physics.CheckBox(transform.position + forwardInAttack / 2, new Vector3(attackRange / 2, attackRange / 2, attackRange / 2), transform.rotation, whatIsPlayer);

        if (!playerInSightRange && !playerInAttackRange) Patrolloing();
        if (playerInSightRange && !playerInAttackRange) ChasePlayer();
        if (playerInAttackRange && playerInSightRange) AttackPlayer();

        animator.SetFloat("Speed", agent.velocity.magnitude);
    }

    private void AttackPlayer()
    {
        if (player != null)
        {
            // 안움직이게
            agent.SetDestination(transform.position);

            transform.LookAt(player.transform);

            if (!alreadyAttacked)
            {
                // 공격 애니메이션
                animator.SetBool("Shot", true);
                animator.SetBool("Reload", false);
                player.hurt(30.0f);
                playshotgunSound();

                alreadyAttacked = true;
                Invoke(nameof(ResetAttack), timeBetweenAttack);
            }
        }
    }

    public void Reload()
    {
        // 공격 애니메이션
        animator.SetBool("Shot", false);
        animator.SetBool("Reload", true);
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    public void TakeDamage(int damage)
    {
        health -= damage;

        if (health <= 0)
            Invoke(nameof(DestroyEnemy), 0.5f);
    }

    private void DestroyEnemy()
    {
        Destroy(gameObject);
    }

    private void SearchWalkPoint()
    {
        float randomZ = UnityEngine.Random.Range(-walkPointRange, walkPointRange);
        float randomX = UnityEngine.Random.Range(-walkPointRange, walkPointRange);
        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
            walkPointSet = true;
    }

    private void ChasePlayer()
    {
        if (player != null)
        {
            agent.SetDestination(player.transform.position);
        }
    }

    private void Patrolloing()
    {
        if (!walkPointSet) SearchWalkPoint();

        if (walkPointSet)
            agent.SetDestination(walkPoint);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        if (distanceToWalkPoint.magnitude < 1f)
            walkPointSet = false;
    }

    public void playshotgunSound()
    {
        AudioSource.PlayClipAtPoint(shotgunSound, player.transform.position);
    }

}
