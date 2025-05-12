using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DisplayLevel_OpVsInfo : DisplayLevel
{
    public static DisplayLevel_OpVsInfo Instance;

    private void Awake() {
        Instance = this;
    }

    bool canPress = false;

    public Image[] images;

    public int correctImage = 0;

    bool canInteract = false;
    public List<BiasButton> buttons = new List<BiasButton>();

    public override void StartLevel() {
        base.StartLevel();

        for (int i = 0; i < buttons.Count; i++)
        {
            var button = buttons[i];
            button.uiText.text = MissionIntroDisplay.Instance.hpTitles[i];
            button.Hide();
        }
        OnMediaDownloaded();

    }

    public override void OnMediaDownloaded() {
        base.OnMediaDownloaded();

        foreach (var button in buttons) {
            button.FadeIn();
        }

        canPress = true;
    }


    public void PressButton(int index) {

        if ( !canPress)
        {
            return;   
        }
        canPress = false;
        foreach (var button in buttons) {
            button.FadeOut();
        }
        string correctAnswer = GetCurrentDocument().correctStatement.ToLower();
        string chosenAnswer = buttons[index].GetComponentInChildren<TextMeshProUGUI>().text.ToLower();
        Debug.Log($"correct answer : {correctAnswer}");
        Debug.Log($"chosen answer : {chosenAnswer}");

        if (chosenAnswer == correctAnswer) {
            ++correctAnswers;
            MissionDisplay.instance.Document_Sucess();
        } else {
            MissionDisplay.instance.Document_Fail();
        }

    }


}
