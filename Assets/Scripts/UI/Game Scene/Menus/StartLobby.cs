using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartLobby : MonoBehaviour {

    public StartLobbyPlayerEntry startLobbyPlayerPrefab;
    public RectTransform startLobbyPanel;

    Color lightGrey = Color.HSVToRGB(0, 0, 0.75f);
    Color darkGrey = Color.HSVToRGB(0, 0, 0.25f);

    void Update()
    {
        for (int i = 0; i < startLobbyPanel.childCount; i++)
        {
            Destroy(startLobbyPanel.GetChild(i).gameObject);
        }

        int j = 0;
        float bottom = 0;
        foreach (var player in FindObjectsOfType<NetworkPlayer>())
        {
            var entry = Instantiate<StartLobbyPlayerEntry>(startLobbyPlayerPrefab, startLobbyPanel);
            var rect = entry.GetComponent<RectTransform>();
            rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, rect.anchoredPosition.y + j);
            j -= 60;
            bottom = entry.transform.localPosition.y - 50;

            entry.nameField.text = player.buisnessName;
            if (player.RestaurantStyle != null) entry.splash.sprite = player.RestaurantStyle.splash;
            else entry.splash.sprite = null;
            entry.splash.color = player.playerColor;
            entry.readyText.text = player.ready ? "READY" : "NOT READY";
            entry.readyImage.color = player.ready ? darkGrey : lightGrey;
            entry.readyText.color = player.ready ? lightGrey : darkGrey;
        }

        startLobbyPanel.sizeDelta = new Vector2(startLobbyPanel.sizeDelta.x, -bottom);
    }

    public void StartGameVote()
    {
        NetworkPlayer.localPlayer.ready = !NetworkPlayer.localPlayer.ready;
        NetworkPlayer.localPlayer.Cmd_SendReadyStartVote(NetworkPlayer.localPlayer.ready);
    }
}
