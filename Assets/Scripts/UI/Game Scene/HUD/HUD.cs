using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour {
    
    public Slider turnTimer;
    public Text phaseText;
    public Image HierarchyButtonImage;
    public Sprite HierarchyViewSprite;
    public Sprite WorldViewSprite;
    public SelectedEmployeePanel SelectedEmployeePanel;

    [HideInInspector]
    public bool countingDown;

    private Image TurnTimerFillImage;    
    private Vector3 textTimerPos;
    private Vector3 textNoTimerPos;

    private void Awake()
    {
        TurnTimerFillImage = turnTimer.fillRect.GetComponent<Image>();
        textTimerPos = phaseText.transform.position;
        textNoTimerPos = textTimerPos - new Vector3(0, 7.5f, 0);
    }

    void Update () {
        if (countingDown)
        {
            turnTimer.value -= Time.deltaTime;
            if(turnTimer.value <= 0f)
            {
                GameController.PlayerOutOfTime();
                countingDown = false;
                return;
            }
            TurnTimerFillImage.color = Color.HSVToRGB(Mathf.Lerp(0f, 0.4f, turnTimer.value),0.9f,0.9f);
        }
	}

    #region Event Handlers

    public void NextPhaseButton_Click()
    {
        switch (GameController.phase)
        {
            case GameController.Phase.Restructuring:
                GameUI.instance.dialog.Show("Confirm", "Are you sure you want to lock in this company structure?", "Yes", GameController.TransitionFromRestructuringToOperations, "No", null);
                break;

            case GameController.Phase.Operations:
                if (!NetworkPlayer.localPlayer.ready)
                    GameController.ReadyUp();
                else
                    GameController.UndoReadyUp();
                break;

            case GameController.Phase.Dinner:
                break;
        }
    }

    public void EmployeeViewButton_Click()
    {
        GameController.ToggleBetweenHierarchyAndWorld();
    }

    public void ToggleHireablesButton_Click()
    {
        GameController.ToggleShowHires();
    }

    #endregion

    #region Update the HUD

    internal void SelectItem(object p)
    {
        RemoveSelection();

        if (p != null && p is Employee)
        {
            ShowEmployeeSelection(p as Employee);
        }
    }

    private void RemoveSelection()
    {
        if (SelectedEmployeePanel.RenameField.isActiveAndEnabled) SelectedEmployeePanel.RenameComplete();
        SelectedEmployeePanel.gameObject.SetActive(false);
    }

    private void ShowEmployeeSelection(Employee employee)
    {
        SelectedEmployeePanel.gameObject.SetActive(true);
        SelectedEmployeePanel.LoadEmployee(employee);
    }

    public void SetOperationsText(bool ready)
    {
        phaseText.text = ready ? "Waiting for others" : "Operating Phase";
    }

    public void SetRestructuringText()
    {
        phaseText.text = "Restructuring Phase";
    }

    internal void SetDinnerText()
    {
        phaseText.text = "Dinner Phase";
    }

    public void StartTimer(int seconds)
    {
        turnTimer.maxValue = seconds;
        turnTimer.value = seconds;
        countingDown = true;
    }
    
    public void EndTimer()
    {
        turnTimer.value = 0;
        countingDown = false;
    }

    internal void SetHierarchyButtonImage(bool worldView)
    {
        if (worldView)
        {
            HierarchyButtonImage.sprite = HierarchyViewSprite;
        }
        else
        {
            HierarchyButtonImage.sprite = WorldViewSprite;
        }
    }

    #endregion
}
