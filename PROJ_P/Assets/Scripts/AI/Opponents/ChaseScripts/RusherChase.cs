﻿//Main Author: Emil Dahl


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Hostile/Rusher/RusherChase")]
public class RusherChase : ChaseBase
{

    private GameObject rushCooldown;
    private float cooldown = 10f, rushMinDistance = 5f, rushMaxDistance = 10f;
    private bool canRush = true;
    public override void EnterState()
    {
        base.EnterState();
    }

    public override void ToDo()
    {
        base.ToDo();
        if (owner.target != null && owner.agent.enabled)
        {
            Chase();
            CheckForDamage();
            EngageRush();

        }
    }

    private void EngageRush()
    {
        if (Vector3.Distance(owner.agent.transform.position, owner.target.transform.position) < rushMaxDistance && owner.target.CompareTag("Player"))
        {
            if (!rushCooldown && canRush)
            {
                rushCooldown = Instantiate(new GameObject("RushCDTimer"));
                rushCooldown.AddComponent<Timer>().RunCountDown(cooldown, EnableAbility, Timer.TimerType.DELAY);

                Debug.Log("In Rusher Chase!");
                canRush = false;
                owner.ChangeState<RusherRush>();
            }
        }
    }

    private void EnableAbility()
    {
        canRush = true;
    }

    protected override void Die()
    {
        base.Die();
        owner.ChangeState<RusherDeath>();
    }

    public override void TakeDamage(float damage, float magnitude)
    {
        base.TakeDamage(damage, magnitude);
        float oldHealth = owner.Health;
        owner.Health -= damage;
        owner.ui.ChangeHealth(owner.InitialHealth, owner.Health);

        SetCrowdControl(magnitude);

    }


    protected override void SetCrowdControl(float magnitude)
    {
        switch (weight.Compare(magnitude))
        {
            case 1:
                owner.ChangeState<RusherStagger>();
                break;
            case 2:
                owner.ChangeState<RusherKnockback>();
                break;
            case 3:
                owner.ChangeState<RusherSuperKnockback>();
                break;
            default:
                break;
        }
    }

    protected override void CheckForDamage()
    {
        owner.agent.avoidancePriority = 99;

        if (distanceToTarget < owner.GetAttackRange && CapsuleCast() && owner.AliveProp && !attacking)
        {
            owner.agent.obstacleAvoidanceType = UnityEngine.AI.ObstacleAvoidanceType.NoObstacleAvoidance;
            if (owner.getGenericTimer.TimeTask)
            {
                attacking = true;
                owner.PlayDamageAudio(owner.attackSound);
                owner.getGenericTimer.SetTimer(owner.AttackSpeed);
                attacking = !attacking;
                owner.agent.obstacleAvoidanceType = UnityEngine.AI.ObstacleAvoidanceType.LowQualityObstacleAvoidance;
                DamageTarget();
                animator.SetTrigger("Melee");
            }
        }
    }
}

#region ChaseLegacy
// lightAngle = lightField.spotAngle;
//ChaseEvent chaseEvent = new ChaseEvent();
//chaseEvent.gameObject = owner.gameObject;
//chaseEvent.eventDescription = "Chasing Enemy";
//chaseEvent.audioSpeaker = audioSpeaker;

//EventSystem.Current.FireEvent(chaseEvent);
//protected override void OperateHesitation()
//{
//    base.OperateHesitation();
//    if (Vector3.Distance(owner.gameObject.transform.position, owner.target.gameObject.transform.position) <= hesitationDistance)
//    {
//        owner.ChangeState<FanaticHesitate>();
//    }
//}
#endregion