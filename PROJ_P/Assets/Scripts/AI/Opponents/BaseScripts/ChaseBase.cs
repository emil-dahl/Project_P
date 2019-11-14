﻿//Main Author: Emil Dahl

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;



[RequireComponent(typeof(CapsuleCollider))]
public class ChaseBase : AliveBase
{
    [SerializeField] protected float hesitationChance = 1f;
    [SerializeField] protected float hesitationDistance;
    [SerializeField] protected float playerStoppingDistance = 1.5f;
    [SerializeField] protected float buildingStoppingDistance = 2.8f;
    public override void EnterState()
    {
        base.EnterState();
        Mathf.Clamp(hesitationChance, 0, 1);
    }



    public override void ToDo()
    {
        base.ToDo();
        OperateHesitation();
    }

    protected virtual void Chase()
    {
        owner.agent.avoidancePriority = 99;
        if (owner.target.CompareTag("Player"))
        {
            owner.agent.stoppingDistance = playerStoppingDistance;
            Player.instance.GetComponent<NavMeshAgent>().avoidancePriority = 99;
        }
        else
        {
            owner.agent.stoppingDistance = buildingStoppingDistance;
        }

        distanceToTarget = Vector3.Distance(owner.transform.position, owner.target.transform.position);
        owner.agent.SetDestination(owner.target.transform.position);
        if(owner.target.transform.parent != null)
        owner.transform.LookAt(owner.target.transform.parent.transform.position + new Vector3(0, owner.capsuleCollider.radius, 0));
        else
            owner.transform.LookAt(owner.target.transform.position + new Vector3(0, owner.capsuleCollider.radius, 0));
    }

    protected virtual void OperateHesitation() { }

    protected bool DiceRoll()
    {
        float diceRoll = Random.Range(0, 1);
        if (diceRoll <= hesitationChance)
            return true;
        else
            return false;
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