﻿//Main Author: Emil Dahl

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;



[RequireComponent(typeof(CapsuleCollider))]
public class HostileBaseState : State
{
    // Attributes

    protected enum Behaviors { STAGGER, KNOCKBACK }
    [SerializeField] protected Behaviors controlBehaviors = Behaviors.STAGGER;
    [SerializeField] protected Material material;
    [SerializeField] protected float moveSpeed;
    [SerializeField] protected Vector3 scale;
    [SerializeField] protected float staggerCD = 0.5f;
    [SerializeField] protected GameObject bloodParticle;
    #region components
    protected CapsuleCollider capsuleCollider;
    protected Unit owner;
    protected UnitDeath death;
    protected GameObject timer;
    #endregion
    protected Rigidbody rigidbody;
    protected Vector3 heading;
    protected const float rotationalSpeed = 0.035f;
    protected float actualDamage;
    protected float distanceToPlayer;
    protected bool damaged = false;
    protected bool alive = true;
    protected bool timerRunning = false;
    protected bool attacking = false;



    // Methods
    public override void EnterState()
    {
        base.EnterState();
        owner.renderer.material = material;
        owner.agent.speed = moveSpeed;
        owner.transform.localScale = scale;
    }


    public override void InitializeState(StateMachine owner)
    {
        this.owner = (Unit)owner;
    }

    public override void ToDo(){ }

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
        GameObject splatter = Instantiate(bloodParticle, owner.transform.position, Quaternion.identity);
        splatter.AddComponent<Timer>().RunCountDown(4, PlaceboMethod);
    }

    private void PlaceboMethod() { }

}
#region EnemyBaseLegacy
// lightTreshold = owner.LightThreshold;
//     spreadAngle = Quaternion.AngleAxis(lightField.spotAngle, owner.agent.velocity);
//// protected float lightAngle;
// //private Quaternion spreadAngle;
    //protected float DotMethod()
    //{
    //    heading = (owner.player.transform.position - owner.transform.position).normalized;
    //    dotProduct = Vector3.Dot(owner.agent.velocity.normalized, heading);
    //    return dotProduct;
    //}
    //private float dotProduct;
#endregion