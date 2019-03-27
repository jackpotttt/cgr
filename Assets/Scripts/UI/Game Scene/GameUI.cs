using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class GameUI : MonoBehaviour {

    public static GameUI instance;

    public StartLobbyPlayerEntry startLobbyPlayerPrefab;
    public GameObject messagePrefab;

    public GameObject staticMessage;
    public RestaurantCustomizationMenu restaurantCustomizationMenu;
    public StartLobby startLobby;
    public GameObject centerMessage;
    public HUD hud;
    public Dialog dialog;

    public int maxMessages;
    public float messageDurationSeconds;

    private List<string> _messageQueue = new List<string>();
    private List<Message> _currentlyActiveMessages = new List<Message>();
    private Text _staticMessageText;

    private class Message
    {
        public GameObject gameObject;
        public float duration;

        public Message(GameObject gameObject, float duration)
        {
            this.gameObject = gameObject;
            this.duration = duration;
        }
    }

    public void Awake()
    {
        instance = this;
    }

    public void Start()
    {
        _staticMessageText = staticMessage.GetComponentInChildren<Text>();
    }

    public void Update()
    {
        while(_messageQueue.Count > 0 && _currentlyActiveMessages.Count < maxMessages)
        {
            var newMessage = Instantiate(messagePrefab, transform);
            newMessage.GetComponentInChildren<Text>().text = _messageQueue[0];
            _messageQueue.RemoveAt(0);

            var newTransform = newMessage.GetComponent<RectTransform>();
            float y;
            if (_currentlyActiveMessages.Count > 0)
                y = _currentlyActiveMessages.Last().gameObject.transform.localPosition.y - (0.55f * newTransform.rect.height);                            
            else
                y = staticMessage.transform.localPosition.y;
            newTransform.localPosition = new Vector3(newTransform.localPosition.x, y, newTransform.localPosition.x);

            _currentlyActiveMessages.Add(new Message(newMessage, messageDurationSeconds));
        }

        if(_currentlyActiveMessages.Count > 0)
        {
            List<Message> toRemove = new List<Message>();
            foreach(var message in _currentlyActiveMessages)
            {
                message.duration -= Time.deltaTime;
                if (message.duration <= 0f)
                    toRemove.Add(message);

                else if(message.duration <= message.gameObject.GetComponent<CanvasGroup>().alpha)
                {
                    message.gameObject.GetComponent<CanvasGroup>().alpha = message.duration;
                }
            }

            foreach(var message in toRemove)
            {
                _currentlyActiveMessages.Remove(message);
                Destroy(message.gameObject);
            }

            var y = staticMessage.transform.localPosition.y; 
            foreach (var message in _currentlyActiveMessages)
            {
                var t = message.gameObject.transform;
                t.localPosition = new Vector3(t.localPosition.x, y, t.localPosition.x);
                y -= 0.55f * message.gameObject.GetComponent<RectTransform>().rect.height;
            }
        }
        if (_currentlyActiveMessages.Count == 0 && _staticMessageText.text != "")        
            staticMessage.SetActive(true);        
        else        
            staticMessage.SetActive(false);
    }

    public void SetStaticMessage(string text)
    {
        _staticMessageText.text = text;
    }

    public void StopStaticMessage()
    {
        _staticMessageText.text = "";
    }

    public void ShowStaticCenterMessage(string text)
    {
        instance.centerMessage.SetActive(true);
        instance.centerMessage.GetComponentInChildren<Text>().text = text;
    }

    public void StopStaticCenterMessage()
    {
        instance.centerMessage.SetActive(false);
    }

    public void AddTempMessage(string s)
    {
        _messageQueue.Add(s);
    }
}
