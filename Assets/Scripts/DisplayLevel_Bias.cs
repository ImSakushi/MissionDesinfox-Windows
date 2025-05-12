using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DisplayLevel_Bias : DisplayLevel
{
    public static DisplayLevel_Bias Instance;

    public List<BiasButton> buttons = new List<BiasButton>();
    bool canPress = false;

    private void Awake() {
        Instance = this;
    }

    public override void StartLevel() {
        base.StartLevel();
        for (int i = 0; i < buttons.Count; i++) {
            buttons[i].uiText.text = MissionIntroDisplay.Instance.biaisTitles[i];
            buttons[i].FadeIn();
        }
        canPress = true;
    }

    public override void OnMediaDownloaded() {
        base.OnMediaDownloaded();

        foreach (var button in buttons) {
            button.FadeIn();
        }

        canPress = true;
    }

    public void PressButton(int index) {
        if (!canPress) {
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
