﻿//Main Author: Emil Dahl

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;



[RequireComponent(typeof(CapsuleCollider))]
public class DeathBase : HostileBaseState
{
    // Attributes
    [SerializeField] protected float corpseTimer = 2f;

    // Methods
    public override void EnterState()
    {
        base.EnterState();
        DisableUnit();
    }


    public override void InitializeState(StateMachine owner)
    {
        this.owner = (Unit)owner;
    }

    public override void ToDo()
    {
        base.ToDo();
        DeathRotation();
    }

    protected void DisableUnit()
    {
        owner.gameObject.transform.position = new Vector3(owner.gameObject.transform.position.x,
            owner.gameObject.transform.position.y - owner.capsuleCollider.radius / 2, owner.transform.position.z);

        owner.rigidbody.isKinematic = true;
        owner.agent.isStopped = true;
        owner.agent.enabled = false;
        owner.capsuleCollider.enabled = false;
        Destroy(owner.gameObject, corpseTimer);
    }

    protected virtual void DeathRotation()
    {
        Quaternion rotation = Quaternion.Euler(-90, 0, 0);
        owner.transform.localRotation = rotation;
        alive = false;
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