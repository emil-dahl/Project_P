﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneHandler : MonoBehaviour
{
    public string mainMenu;
    public static SceneHandler sceneHandler;
    public Settings settings;
    [SerializeField] private GameObject shopWindow, optionsWindow;
    [SerializeField] private GameObject mainMenuPrompt;
    [SerializeField] private GameObject[] promptToggles;
    [SerializeField] private GameObject[] adaptionToggles;
    [SerializeField] private Slider paceSlider;
    [SerializeField] private AudioSource eliasSource;

    public static SceneHandler instance;

    private Coroutine pauseCoroutine;

    public void SetupPromptToggles()
    {
        promptToggles[0].SetActive(settings.UseWarnings);
        promptToggles[1].SetActive(settings.UseInfo);
        promptToggles[2].SetActive(settings.UseBonus);
    }
    public void SetupAdaptions()
    {
        adaptionToggles[0].SetActive(settings.UseAutoRefill);
        adaptionToggles[1].SetActive(settings.UseExtraShopTime);
        adaptionToggles[2].SetActive(settings.UseMusic);
        adaptionToggles[3].SetActive(settings.UseSFX);
        adaptionToggles[4].SetActive(settings.UseAimAssist);
        paceSlider.value = settings.enemyPace;

        if(eliasSource)
            eliasSource.mute = !settings.UseMusic;
    }
    


    public void SetPace()
    {
        settings.enemyPace = paceSlider.value;
    }


    public void Awake()
    {
        instance = this;
    }

    public void Start()
    {
        if(promptToggles != null && promptToggles.Length > 0)
        {
            SetupPromptToggles();
        }

        if(adaptionToggles != null && adaptionToggles.Length > 0)
        {
            SetupAdaptions();

            if(eliasSource)
                eliasSource.mute = !settings.UseMusic;
        }
    }

    public void ToggleExtraShopTime() { settings.UseExtraShopTime = !settings.UseExtraShopTime; }

    public void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (shopWindow && shopWindow.activeSelf)
            {
                shopWindow.SetActive(false);
            }
            else if (mainMenuPrompt && !mainMenuPrompt.activeSelf)
            {
                pauseCoroutine = StartCoroutine(CheckRestore());
                Time.timeScale = 0;
                mainMenuPrompt.SetActive(true);
            } else if(SceneManager.GetActiveScene() == SceneManager.GetSceneByName("OptionsMenu") || SceneManager.GetActiveScene() == SceneManager.GetSceneByName("ClassChooserScene") || SceneManager.GetActiveScene() == SceneManager.GetSceneByName("ScoreboardScene") || SceneManager.GetActiveScene() == SceneManager.GetSceneByName("CreditsMenu"))
            {
                GoToScene("MainMenu");
            }

        }
    }

    public IEnumerator CheckRestore()
    {

        bool paused = true;

        while (paused)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (optionsWindow && optionsWindow.activeSelf)
                {
                    optionsWindow.SetActive(false);
                }
                else if (mainMenuPrompt && mainMenuPrompt.activeSelf)
                {

                    mainMenuPrompt.SetActive(false);
                    Player.instance.SetHover(false);
                    paused = false;
                    RestoreTimeScale();
                }

            }

        yield return null;
        }
    }

    public void RestoreTimeScale()
    {
        StopCoroutine(pauseCoroutine);
        Time.timeScale = 1;
    }

    public void Toggle(GameObject go) {

        go.SetActive(!go.activeSelf);
    }

    public void ResetAllSettings()
    {
        settings.UseWarnings = true;
        settings.UseBonus = true;
        settings.UseInfo = true;

        settings.UseMusic = true;
        settings.UseSFX = true;

        settings.UseAutoRefill = false;
        settings.UseExtraShopTime = false;
        settings.UseAimAssist = false;

        settings.enemyPace = 1;

        SetupPromptToggles();
        SetupAdaptions();

    }
    public void ToggleWarnings() {
        settings.UseWarnings = !settings.UseWarnings;
    }
    public void ToggleInfo()
    {
        settings.UseInfo = !settings.UseInfo;
    }
    public void ToggleBonus()
    {
        settings.UseBonus = !settings.UseBonus;
    }
    public void ToggleAutoRefill()
    {
        settings.UseAutoRefill = !settings.UseAutoRefill;
    }
    public void ToggleMusic()
    {
        settings.UseMusic = !settings.UseMusic;

        if(eliasSource)
            eliasSource.mute = !settings.UseMusic;
    }
    public void ToggleSFX()
    {
        settings.UseSFX = !settings.UseSFX;

        if (Player.instance != null)
            Player.instance.PlayHeartBeat();
    }

    public void ToggleAimAssist()
    {
        settings.UseAimAssist = !settings.UseAimAssist;
    }
    public void GoToScene(string scene)
    {
        StartCoroutine(DelaySceneChange(scene));
    }

    public IEnumerator DelaySceneChange(string scene)
    {
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene(scene);
    }

    public IEnumerator DelaySceneChange(string scene, float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        SceneManager.LoadScene(scene);
    }

    public IEnumerator DelayQuit()
    {
        yield return new WaitForSeconds(0.5f);
        Application.Quit();
    }

    public void QuitGame()
    {
        StartCoroutine(DelayQuit());
    }


}
