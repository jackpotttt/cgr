using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {

    public const int DEFAULT_PORT = 57575;

    public CanvasGroup mainGroup;
    public CanvasGroup hostGroup;
    public CanvasGroup joinGroup;
    public CanvasGroup newGameGroup;

    public Slider sizeSlider;
    public Slider waterSlider;
    public Slider roadSlider;
    public Slider buildingSlider;

    public InputField clientPort;
    public InputField clientAddress;
    public InputField hostPort;

    public CustomNetworkManager networkManager;

    private bool isOnline = true;

    private void Start()
    {
        Debug.Log("loaded");
        networkManager = FindObjectOfType<CustomNetworkManager>();

        mainGroup.gameObject.SetActive(true);
        mainGroup.alpha = 1;

        hostGroup.gameObject.SetActive(false);
        hostGroup.alpha = 0;
        joinGroup.gameObject.SetActive(false);
        joinGroup.alpha = 0;
        newGameGroup.gameObject.SetActive(false);
        newGameGroup.alpha = 0;
    }

    private void BeginHost()
    {        
        try
        {
            networkManager.Host(int.Parse(hostPort.text));
        }
        catch(Exception e)
        {
            Debug.Log("Could not start host");
            Debug.LogError(e);
        }
    }

    private void BeginClient()
    {
        try
        {
            networkManager.Join(clientAddress.text, int.Parse(clientPort.text));
        }
        catch (Exception e)
        {
            Debug.Log("Could not start client");
            Debug.LogError(e);
        }
    }

    public IEnumerator FadeTo(CanvasGroup start, CanvasGroup finish, int frames, int deadzone)
    {
        finish.alpha = 0;
        finish.gameObject.SetActive(true);

        for (int i = 0; i < frames + deadzone; i++)
        {
            yield return null;
            finish.alpha = Mathf.Clamp01(((float)i - deadzone) / ((float)frames));
            start.alpha = Mathf.Clamp01(((float)(frames - i)) / ((float)frames));
        }
        start.gameObject.SetActive(false);
    }

    #region UI Events

    public void ButtonEntered(Animator anim)
    {
        anim.SetBool("entered", true);        
    }
    public void ButtonLeft(Animator anim)
    {
        anim.SetBool("entered", false);
    }

    public void JoinButtonClick(Animator anim)
    {
        StartCoroutine(FadeTo(mainGroup, joinGroup, 20, 15));
        ButtonLeft(anim);
    }

    public void HostButtonClick(Animator anim)
    {
        StartCoroutine(FadeTo(mainGroup, hostGroup, 20, 15));
        ButtonLeft(anim);
    }

    public void HostBackButtonClick(Animator anim)
    {
        StartCoroutine(FadeTo(hostGroup, mainGroup, 20, 15));
        ButtonLeft(anim);
    }

    public void JoinBackButtonClick(Animator anim)
    {
        StartCoroutine(FadeTo(joinGroup, mainGroup, 20, 15));
        ButtonLeft(anim);
    }

    public void NewGameButtonClick(Animator anim)
    {
        StartCoroutine(FadeTo(hostGroup, newGameGroup, 20, 15));
        ButtonLeft(anim);
    }

    public void NewBackButtonClick(Animator anim)
    {
        StartCoroutine(FadeTo(newGameGroup, hostGroup, 20, 15));
        ButtonLeft(anim);
    }

    public void ConnectToGameClick(Animator anim)
    {
        ButtonLeft(anim);
        BeginClient();

        WorldGenData.generate = false;
        SceneManager.LoadScene(1, LoadSceneMode.Single);
    }

    public void StartNewGameClick(Animator anim)
    {
        ButtonLeft(anim);
        if (isOnline) BeginHost();
        
        WorldGenData.Size = (sizeSlider.value * 0.5f) + 0.2f;
        WorldGenData.Water = waterSlider.value;
        WorldGenData.Roads = (roadSlider.value * 0.8f) + 0.2f;
        WorldGenData.Buildings = (buildingSlider.value * 0.8f) + 0.2f;
        WorldGenData.generate = true;

        SceneManager.LoadScene(1, LoadSceneMode.Single);
    }

    public void IsOnlineButtonClick(Text text)
    {
        isOnline = !isOnline;
        text.text = isOnline ? "True" : "False";
    }

    #endregion

}
