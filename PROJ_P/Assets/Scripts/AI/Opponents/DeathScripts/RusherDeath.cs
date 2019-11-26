﻿//Main Author: Emil Dahl
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Hostile/Rusher/RusherDeath")]
public class RusherDeath : DeathBase
{
    public override void EnterState()
    {
        base.EnterState();
        Debug.Log("die");
        animator.SetTrigger("Death");
    }

    public override void ToDo()
    {
            base.ToDo();
    }
    protected override void RemoveObject()
    {
        BowoniaPool.instance.AddToPool(PoolObject.ZOOMER, owner.gameObject);
    }
}

#region ChaseLegacy
// lightAngle = lightField.spotAngle;
//ChaseEvent chaseEvent = new ChaseEvent();
//chaseEvent.gameObject = owner.gameObject;
//chaseEvent.eventDescription = "Chasing Enemy";
//chaseEvent.audioSpeaker = audioSpeaker;

//EventSystem.Current.FireEvent(chaseEvent);
#endregion