﻿//Main Author: Emil Dahl

using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[RequireComponent(typeof(CapsuleCollider))]
public class HostileBaseState : State
{
    // Attributes
    [SerializeField] protected Behaviors controlBehaviors = Behaviors.STAGGER;
    [SerializeField] protected Material material;
    [SerializeField] protected float moveSpeed;
    [SerializeField] protected float EnemyHealth { get { return health; } set { health = value; } }
    private float health;
    [SerializeField] private Vector3 scale;
    [SerializeField] private bool specialDeath;
    private CapsuleCollider capsuleCollider;
    private Vector3 heading;
    private const float rotationalSpeed = 0.035f;
    private float dotProduct;
    [SerializeField] private float staggerCD = 0.5f;
    private bool damaged = false;

    [SerializeField] protected float enemyBaseDamage = 5f;
    [SerializeField] private float maxCritical = 10f;
    [SerializeField] private float attackSpeed = 1f;
    protected enum Behaviors { STAGGER, KNOCKBACK }
    protected float deathTimer;
    protected float actualDamage;
    protected float distanceToPlayer;
    protected const float damageDistance = 2.5f;
    protected Unit owner;
    private UnitDeath death;
    protected GameObject timer;
    private float startTime;
    protected bool alive = true;
    private bool timerRunning = false;
    protected bool attacking = false;



    // Methods
    public override void EnterState()
    {
        base.EnterState();
        health = 10f;
        owner.renderer.material = material;
        owner.agent.speed = moveSpeed;
        owner.transform.localScale = scale;
        capsuleCollider = owner.GetComponent<CapsuleCollider>();
        EventSystem.Current.RegisterListener<WaveCompletionEvent>(NewWave);
    }


    public override void InitializeState(StateMachine owner)
    {
        this.owner = (Unit)owner;
    }

    public override void ToDo()
    {
        
        if (health <= 0)
        {
            if (alive)
            {
                death = new UnitDeath();
                death.eventDescription = "Unit Died";
                death.enemyObject = owner.gameObject;
                EventSystem.Current.FireEvent(death);
            }
            deathTimer = 2f;
            Die();
        }
    }
    protected void CheckForDamage()
    {

        if (distanceToPlayer < damageDistance && LineOfSight() && alive && !attacking)
        {
            if (owner.getGenericTimer.timeTask)
            {
                attacking = true;
                owner.getGenericTimer.SetTimer(attackSpeed);
                attacking = !attacking;
                DamagePlayer();
            }
        }
    }

    public override void TakeDamage(float damage)
    {
        EnemyHealth -= damage;

        if (controlBehaviors == Behaviors.STAGGER)
        {
            ControlEffects();
        }
    }

    public void NewWave(WaveCompletionEvent waveCompletion)
    {
        health += waveCompletion.EnemyHealth;
        Debug.Log("wave health: " + health);
    }

    protected bool LineOfSight()
    {
        bool lineCast = Physics.Linecast(owner.agent.transform.position, owner.player.transform.position, owner.visionMask);
        if (lineCast)
            return false;
        return true;
    }

    protected void DamagePlayer()
    {
        actualDamage = Random.Range(enemyBaseDamage, maxCritical);
        owner.player.GetComponent<Player>().HealthProp -= actualDamage;
    }



    protected void Chase()
    {
        distanceToPlayer = Vector3.Distance(owner.transform.position, owner.player.transform.position);
        owner.agent.SetDestination(owner.player.transform.position);
    }


    protected float DotMethod()
    {
        heading = (owner.player.transform.position - owner.transform.position).normalized;
        dotProduct = Vector3.Dot(owner.agent.velocity.normalized, heading);
        return dotProduct;
    }

    protected virtual void Die()
    {
        DeathAnimation();
        owner.agent.isStopped = true;
        capsuleCollider.enabled = false;
        Destroy(owner.gameObject, deathTimer);

    }
    protected void DeathAnimation()
    {
        Quaternion rotation = Quaternion.Euler(-90, 0, 0);
        owner.transform.localRotation = rotation;
        alive = false;
        float startTIme = 2;

        if (specialDeath)
        {
            while (startTIme > 0)
            {
                owner.transform.localScale = (owner.transform.localScale * 1.00003f);
                startTIme -= Time.deltaTime;
            }
        }
    }

    protected virtual void ControlEffects()
    {
        if (owner.getGenericTimer.timeTask && !damaged)
        {
            Debug.Log("Please work, visual studio!");
            damaged = true;
            owner.getGenericTimer.SetTimer(staggerCD);
            damaged = false;
            owner.agent.SetDestination(owner.transform.position);
            
        }
    }
}
#region EnemyBaseLegacy
// lightTreshold = owner.LightThreshold;
//     spreadAngle = Quaternion.AngleAxis(lightField.spotAngle, owner.agent.velocity);
//// protected float lightAngle;
// //private Quaternion spreadAngle;
#endregion