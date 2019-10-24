﻿//Main Author: Emil Dahl

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;



[RequireComponent(typeof(CapsuleCollider))]
public class ChaseBase : HostileBaseState
{
    public override void EnterState()
    {
        base.EnterState();
    }


    public override void InitializeState(StateMachine owner)
    {
        this.owner = (Unit)owner;
    }

    public override void ToDo()
    {
        base.ToDo();
        CheckLife();
        CheckForDamage();
    }

    protected void CheckLife()
    {
        if (owner.Health <= 0)
        {
            if (alive)
            {
                death = new UnitDeath();
                death.eventDescription = "Unit Died";
                death.enemyObject = owner.gameObject;
                EventSystem.Current.FireEvent(death);
            }

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
                owner.getGenericTimer.SetTimer(owner.AttackSpeed);
                attacking = !attacking;
                DamagePlayer();
            }
        }
    }

    protected virtual void DamagePlayer()
    {
        actualDamage = Random.Range(owner.Attack, owner.Attack * 3);
        owner.player.GetComponent<Player>().HealthProp = -actualDamage;
    }

    public override void TakeDamage(float damage)
    {
        owner.Health -= damage;
        if (controlBehaviors == Behaviors.STAGGER)
        {
            ControlEffects();
        }
    }

    /// <summary>
    /// Used to call different death-states from different enemies 
    /// </summary>
    protected virtual void Die() { }

    protected virtual void ControlEffects()
    {
        if (owner.getGenericTimer.timeTask && !damaged)
        {
            damaged = true;
            owner.getGenericTimer.SetTimer(staggerCD);
            damaged = false;

            if (owner.agent.enabled)
                owner.agent.SetDestination(owner.transform.position);
        }
    }

    protected virtual void Chase()
    {
        distanceToPlayer = Vector3.Distance(owner.transform.position, owner.player.transform.position);
        owner.agent.SetDestination(owner.player.transform.position);
    }
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
#endregion