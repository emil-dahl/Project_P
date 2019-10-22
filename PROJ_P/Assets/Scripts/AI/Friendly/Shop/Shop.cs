﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : StateMachine
{
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject text;
    [SerializeField] private GameObject shopWindow;
    [SerializeField] private float shopTime = 40;
    [SerializeField] private int costOfPotion;
    private GameObject shopTimer;
    private ToggleArrowEvent toggleArrow = new ToggleArrowEvent();
    [SerializeField] private GameObject timerText;
    public Vector3 spawnPoint { get; private set; }

    private void Start()
    {
        spawnPoint = transform.position;
    }
    private void OnEnable()
    {
        spawnPoint = transform.position;
        ChangeState<ShopBaseState>();
        shopTimer = new GameObject("Timer");
        shopTimer.AddComponent<Timer>().RunCountDown(shopTime, RemoveShop);
        timerText.gameObject.SetActive(true);
        timerText.GetComponent<ShopTimer>().SetTimer(shopTimer.GetComponent<Timer>());
        toggleArrow.goal = gameObject;
        toggleArrow.toggle = true;
        EventSystem.Current.FireEvent(toggleArrow);
    }
    private void OnDisable()
    {
        toggleArrow.toggle = false;
        EventSystem.Current.FireEvent(toggleArrow);
    }
    private void RemoveShop()
    {
        ChangeState<ShopTimeFinishedState>();
    }

    public GameObject GetShopWindow()
    {
        return shopWindow;
    }
    public GameObject GetPlayer()
    {
        return player;
    }
    public GameObject GetText()
    {
        return text;
    }
    public float GetShopTime()
    {
        return shopTime;
    }

    public void RefillPotions()
    {
        if (Player.instance.GoldProp >= costOfPotion)
        {
            Player.instance.Resource.IncreaseResource(1f);
            Player.instance.HealthProp += 100;
            Player.instance.GoldProp -= 10;
        }

    }


}