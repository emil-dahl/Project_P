﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Attacks/Melee Hack")]
public class MeleeHack : PlayerAttack
{

    public AnimationClip slash;
    private Animation animation;

    public override void Execute()
    {
        base.Execute();
        animation.AddClip(slash, "Slash");
        animation.Play("Slash");
    }

    public override void OnEquip()
    {
        base.OnEquip();
        Player player = Player.instance;
        player.weapon.GetComponent<Sword>().SetDamage(damage);
        animation = player.weapon.GetComponent<Animation>();

    }




}
