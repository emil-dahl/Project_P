﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "Attacks/Warrior/LifeLeech")]
public class LifeLeech : PlayerAttack
{
    [Header("Ability Specific")]
    [SerializeField] private AnimationClip forwardSlash;
    [SerializeField] private AnimationClip backSlash;
    [SerializeField] private int iterations = 20;
    [SerializeField] private float iterationTime = 0.3f;
    private float regenerationValue = 1;
    private Animation animation;
    private Sword sword;
    [SerializeField] private List<float> regenerationPerLevel = new List<float>();

    public override void OnEnable()
    {
        base.OnEnable();
    }
    public override void Execute()
    {
        base.Execute();
    }
    public override void UpgradeAttack()
    {
        base.UpgradeAttack();
        regenerationValue = regenerationPerLevel[CurrentLevel];
    }

    public override void RunAttack()
    {
        sword.GetComponent<Collider>().enabled = true;
        animation = sword.GetComponent<Animation>();

        if (animation.IsPlaying(forwardSlash.name))
        {
            animation.AddClip(backSlash, backSlash.name);
            animation.Play(backSlash.name);
        }
        else
        {
            animation.AddClip(forwardSlash, forwardSlash.name);
            animation.Play(forwardSlash.name);
        }

        Timer timer = BowoniaPool.instance.GetFromPool(PoolObject.TIMER).GetComponent<Timer>();
        timer.RunCountDown(forwardSlash.length, ResetSword, Timer.TimerType.DELAY);
    }

    public override void OnEquip()
    {
        base.OnEquip();
        sword = player.weapon.GetComponent<Sword>();
        sword.CacheComponents(damage, magnitude, this, null, StealLife());
    }

    public IEnumerator StealLife()
    {
        Particle instantiatedParticle = Instantiate(particles, player.transform.position, Quaternion.identity, player.transform).GetComponent<Particle>();
        instantiatedParticle.disableTime = iterationTime * iterations;
        
        for (int i = 0; i < iterations; i++)
        {
            yield return new WaitForSeconds(iterationTime);
            player.HealthProp = regenerationValue;
        }
    }

    public void ResetSword()
    {
        sword.GetComponent<Collider>().enabled = false;
    }


}
