﻿using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class Player : MonoBehaviour
{

    public static Player instance;
    [SerializeField] private bool debugMode;

    [Header("Attacks")]
    public ClassAttackList attackSets = new ClassAttackList();
    public AttackSet attackSet { get; private set; }
    private PlayerAttack activeAttack;
    private int selectedAttack;
    public GameObject weapon;
    [SerializeField] private KeybindSet keybindSet;

    [Header("UI elements")]
    [SerializeField] private Image[] attackUISpot;
    [SerializeField] private Color fullHealth;
    [SerializeField] private Color emptyHealth;
    [SerializeField] [Range(0, 1)] float colorTransitionPoint = 0.5f;
    private Coroutine[] cooldowns;

    [Header("Attributes")]
    [SerializeField] private Image health;
    [SerializeField] private Image resourceImage;
    [SerializeField] private int gold;
    [SerializeField] private Transform spawnPoint;
    private AttackSet activeAttacks;
    public AudioSource Audio { get; private set; }
    [SerializeField] private AudioClip[] hurtClip;
    [SerializeField] private AudioClip lackResourceClip;

    [Header("Resources")]
    private int healthPotions;
    private int resourcePotions;
    [SerializeField] private float healthPotionIncrease = 30;
    [SerializeField] [Range(0, 3)] private int healthPotionsStart = 3;
    [SerializeField] private Text healthPotionsText;
    [SerializeField] private Text resourcePotionsText;
    [SerializeField] [Range(0, 3)] private int resourcePotionsStart = 3;
    
    public Resource Resource { get; private set; }
    public PlayerClass playerClass;
    private float tempHP = 100f;
    [SerializeField] private bool hover = false;


    [SerializeField] private PlayerStats originalStats;
    [HideInInspector] public PlayerStats activeStats;
    [SerializeField] private Text durabilityTextObject;





    [Serializable]
    public struct PlayerStats
    {
        public float movementSpeed;
        public float resistanceMultiplier;
        public float attackSpeed;
        public float attackDamage;
    }

    public KeybindSet GetKeybindSet() { return keybindSet; }

    public void PlayAudio(AudioClip clip)
    {
        Audio.clip = clip;
        Audio.Play();
    }

    public void ResetStats()
    {
        activeStats.movementSpeed = originalStats.movementSpeed;
        activeStats.resistanceMultiplier = originalStats.resistanceMultiplier;
        activeStats.attackSpeed = originalStats.attackSpeed;
        activeStats.attackDamage = originalStats.attackDamage;
    }

    public int HealthPotions { 
        get { return healthPotions; }
        set
        {
            healthPotions = value;
            healthPotionsText.text = healthPotions.ToString();
        }
    }

    public int ResourcePotionsProp
    {
        get
        {
            return resourcePotions;
        }
        set
        {
            resourcePotions = value;
            if (resourcePotionsText != null)
            {
                resourcePotionsText.text = resourcePotions.ToString();
            }
        }
    }

    public float HealthProp {
        get { return tempHP; }
        set {
            tempHP += value * activeStats.resistanceMultiplier;
            health.fillAmount = tempHP * 0.01f;

            if (health.fillAmount < colorTransitionPoint)
                health.color = Color.Lerp(emptyHealth, fullHealth, health.fillAmount /colorTransitionPoint);

            if (value < 0 && hurtClip != null)
            {
                Audio.clip = hurtClip[Random.Range(0, hurtClip.Length)];
                Audio.Play();
            }
        }
    }
    public int GoldProp
    {
        get { return gold; }
        set
        {
            gold = value;
        }
    }


    
    public delegate void Attack();
    public  event Attack AttackEvent;

    public Transform GetSpawnPoint() { return spawnPoint; }


    public Text GetDurabilityText()
    {
        return durabilityTextObject;
    }
   public void RunAttackCooldown()
    {
        for (int i = 0; i < activeAttacks.list.Length; i++)
        {
            if (activeAttacks.list[i] == activeAttack)
            {
                cooldowns[i] = StartCoroutine(ShowCooldown(i));
            }
        }
    }

    public void Start()
    {
        playerClass = keybindSet.playerClass;
        CacheComponents();
        EventSystem.Current.RegisterListener<GiveResource>(Refill);
    }

    public void SetupClass() {

        switch (playerClass) {

            case PlayerClass.WIZARD:
                Resource = ScriptableObject.CreateInstance<Mana>();
                attackSet = attackSets.Get(PlayerClass.WIZARD);
                break;

            case PlayerClass.WARRIOR:
                Resource = ScriptableObject.CreateInstance<Rage>();
                attackSet = attackSets.Get(PlayerClass.WARRIOR);
                StartCoroutine(TapRage());
                break;

            default:
                throw new Exception("Error when setting resource");
                

        }
    }
  
    public void Refill(GiveResource res)
    {
        Resource.IncreaseResource(res.fillAmount);
    }

    public IEnumerator TapRage()
    {
        yield return null;
        while (true)
        {
            Resource.DrainResource(2 * Time.deltaTime);
            yield return null;
        }
    }


    public AttackSet CloneAttackSet()
    {
        AttackSet clone = Instantiate(attackSet);
        for (int i = 0; i < clone.list.Length; i++)
        {
            clone.list[i] = Instantiate(attackSet.list[i]);

        }

        return clone;
    }


    public void CacheComponents() {

        instance = this;
        SetupClass();
        attackSet = CloneAttackSet();
        activeAttacks = ScriptableObject.CreateInstance<AttackSet>();
        activeAttacks.list[0] = attackSet.list[0];
        if (debugMode)
        {
            for (int i = 0; i < 4; i++)
            {
                activeAttacks.list[i] = attackSet.list[i];
            }

        }
        for (int i = 0; i < attackUISpot.Length; i++)
        {
            if (activeAttacks.list[i] != null)
            {

                attackUISpot[i].sprite = activeAttacks.list[i].GetImage();
            }
        }
        SelectAttack(0);
        Resource.CacheComponents(resourceImage);

        for (int i = 0; i < activeAttacks.list.Length; i++)
        {
            if (activeAttacks.list[i] != null)
            {
                activeAttacks.list[i].ResetCooldown();
            }
        }

        cooldowns = new Coroutine[attackUISpot.Length];
        ResetStats();
        health.color = fullHealth;

        if (playerClass == PlayerClass.WARRIOR)
        {
            ((MeleeHack)activeAttacks.list[0]).SetDurabilityTextObject(durabilityTextObject);
        }
        else
        {
            durabilityTextObject.gameObject.SetActive(false);
        }
        Audio = GetComponent<AudioSource>();
        HealthPotions = healthPotionsStart;
        ResourcePotionsProp = resourcePotionsStart;
    }

    public void Update() {

        if (Input.GetKey(keybindSet.GetBind(KeyFeature.EXECUTE))) {
            if (!ClickOnFriendly() && !hover)
            {
                ExecuteAttack();
            }

        }

        if (Input.GetKeyDown(keybindSet.GetBind(KeyFeature.BASE_ATTACK)))
        {
            if (activeAttacks.list[0] != null)
            {
                SelectAttack(0);
            }
        }
        else if (Input.GetKeyDown(keybindSet.GetBind(KeyFeature.ABILITY_1)))
        {
            if (activeAttacks.list[1] != null)
            {
                SelectAttack(1);
            }
        }
        else if (Input.GetKeyDown(keybindSet.GetBind(KeyFeature.ABILITY_2)))
        {
            if (activeAttacks.list[2] != null)
            {
                SelectAttack(2);
            }
        }
        else if (Input.GetKeyDown(keybindSet.GetBind(KeyFeature.ABILITY_3))) {

            if (activeAttacks.list[3] != null)
            {
                SelectAttack(3);
            }
        }
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            if (activeAttacks.list[(selectedAttack + 1) % activeAttacks.list.Length] != null)
            {
                SelectAttack((selectedAttack + 1) % activeAttacks.list.Length);
            }
            else
            {
                int temp = selectedAttack +1;
                while (activeAttacks.list[(temp) % activeAttacks.list.Length] == null)
                {
                    temp++;
                }
                SelectAttack(temp % activeAttacks.list.Length);

            }
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {


            int temp = (selectedAttack - 1 < 0 ? selectedAttack + activeAttacks.list.Length - 1 : selectedAttack - 1);
                while (activeAttacks.list[temp] == null)
                {
                    temp--;
                }
                SelectAttack(temp);
        }

        if (Input.GetKeyDown(keybindSet.GetBind(KeyFeature.REFILL)))
        {
            UseHealthPotion();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            UseResourcePotion();
        }

        if(tempHP <= 0)
        {
            PlayerDied();   
        }

    }

    private void UseHealthPotion()
    {
        if (HealthPotions>0)
        {
            HealthProp = healthPotionIncrease;
            HealthPotions--;
        }
    }
    private void UseResourcePotion()
    {
        if (ResourcePotionsProp > 0)
        {
            ResourcePotionsProp--;
            if (playerClass == PlayerClass.WIZARD)
            {
                Resource.IncreaseResource(healthPotionIncrease / 100);
            }
            else
            {
                ((MeleeHack)activeAttacks.list[0]).IncreaseDurability(5 /100.0f);
            }
        }

    }

    private void PlayerDied()
    {
        SceneHandler.instance.GoToScene(SceneManager.GetActiveScene().name);
    }

    Image[] attack = new Image[4];
    private IEnumerator ShowCooldown(int position)
    {
        float animationTime;
        float cooldownTime;
        attack[position] = attackUISpot[position];
        attack[position].fillAmount = 0;
        animationTime = 0;
        cooldownTime = activeAttack.GetCooldown() / activeStats.attackSpeed;
        while (animationTime < cooldownTime)
        {
            animationTime += Time.deltaTime;
            attack[position].fillAmount = animationTime / cooldownTime;
            yield return null;

        }
    }

    RaycastHit hit;
    Ray ray;
    private bool ClickOnFriendly()
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform.gameObject.tag == "Friendly")
            {
                return true;
            }
        }
        return false;
    }

    public void SetHover(bool hover)
    {
        this.hover = hover;
    }


    public void SetAbility(int position, PlayerAttack ability)
    {

        if (ability == null)
        {

            attackUISpot[position].color = new Color32(0, 0, 0, 0);
        }
        else
        {

            attackUISpot[position].color = new Color32(0, 0, 0, 100);
            SelectAttack(selectedAttack);
        }
        if (position == selectedAttack)
        {
            SelectAttack(0);
        }
        activeAttacks.list[position] = ability;
    }

    public void ExecuteAttack() {
        if (Resource.Value >= activeAttack.GetCastCost() / 100) 
        {
            AttackEvent();

        }
        else
        {
            Audio.clip = lackResourceClip;
            Audio.Play();
        }
    }


    public void SubscribeToAttackEvent() {

        for (int i = 0; i < activeAttacks.list.Length; i++)
        {
            if (activeAttacks.list[i] != null)
            {
                AttackEvent -= activeAttacks.list[i].Execute;
            }
        }

        AttackEvent += activeAttack.Execute;

    }

    private void UpdateIcons()
    {

        for (int i = 0; i < attackUISpot.Length; i++)
        {
            if (activeAttacks.list[i] != null)
            {
                attackUISpot[i].color = new Color32(0, 0, 0, 138);
            }
            else
            {
                attackUISpot[i].color = new Color32(0, 0, 0, 0);
            }
        }
    }
    

    public void SelectAttack(int selectedAttack)
    {

        UpdateIcons();

        if (activeAttacks.list[selectedAttack]) {

            activeAttack = activeAttacks.list[selectedAttack];
            attackUISpot[selectedAttack].color = new Color32(255,255,255,255);
            this.selectedAttack = selectedAttack;
            activeAttack.OnEquip();
        }

        SubscribeToAttackEvent();

    }



}


public enum PlayerClass {

    WIZARD, WARRIOR

}