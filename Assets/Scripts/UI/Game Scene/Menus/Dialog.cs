using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Dialog : MonoBehaviour
{
    public delegate void Callback();

    public Text Title;
    public Text Message;

    public Button LeftButton;
    internal Text LeftButtonText;

    public Button RightButton;
    internal Text RightButtonText;

    internal Callback left;
    internal Callback right;
    
    public void Show(string title, string message, string leftButton, Callback leftCallback, string rightButton, Callback rightCallback)
    {
        gameObject.SetActive(true);

        if (LeftButtonText == null)
        {
            LeftButtonText = LeftButton.GetComponentInChildren<Text>();
            RightButtonText = RightButton.GetComponentInChildren<Text>();
        }

        Title.text = title;
        Message.text = message;

        LeftButtonText.text = leftButton;
        left = leftCallback;

        RightButtonText.text = rightButton;
        right = rightCallback;
    }

    public void LeftClicked()
    {
        gameObject.SetActive(false);
        left?.Invoke();
    }

    public void RightClicked()
    {
        gameObject.SetActive(false);
        right?.Invoke();
    }
}
