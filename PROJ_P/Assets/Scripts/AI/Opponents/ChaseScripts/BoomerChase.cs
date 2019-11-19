﻿//Main Author: Emil Dahl


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Hostile/Boomer/BoomerChase")]
public class BoomerChase : ChaseBase
{
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
            //JumpDistance();
        }
    }

    protected override void Die()
    {
        base.Die();
        owner.ChangeState<BoomerDeath>();
    }

    public override void TakeDamage(float damage, float magnitude)
    {
        base.TakeDamage(damage, magnitude);

        float oldHealth = owner.Health;
        owner.Health -= damage;
        owner.ui.ChangeHealth(owner.InitialHealth, owner.Health);
    }

    private void JumpDistance()
    {
        if(Vector3.Distance(owner.agent.transform.position, owner.target.transform.position) > 10 && Vector3.Distance(owner.agent.transform.position, owner.target.transform.position) < 20)
            owner.ChangeState<JumpImpact>();
    }

}

#region ChaseLegacy
        //Stagger(magnitude);
    //protected override void OperateHesitation()
    //{
    //    base.OperateHesitation();
    //    if (Vector3.Distance(owner.gameObject.transform.position, owner.target.gameObject.transform.position) <= hesitationDistance)
    //    {
    //        owner.ChangeState<FanaticHesitate>();
    //    }
    //}
// lightAngle = lightField.spotAngle;
//ChaseEvent chaseEvent = new ChaseEvent();
//chaseEvent.gameObject = owner.gameObject;
//chaseEvent.eventDescription = "Chasing Enemy";
//chaseEvent.audioSpeaker = audioSpeaker;

//EventSystem.Current.FireEvent(chaseEvent);
#endregion