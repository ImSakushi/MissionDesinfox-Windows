using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class DisplayDataLoading : Displayable
{
    public static DisplayDataLoading Instance;

    public RectTransform fillImage;
    public RectTransform background;
    bool loading = false;

    public bool debug_local = false;

    private void Awake() {
        Instance = this;
    }

    public override void Start() {
        base.Start();
        Debug.Log($"[DisplayDataLoading] Start() – debug_local = {debug_local}");
        if (debug_local) {
            Debug.Log("[DisplayDataLoading] Mode LOCAL → j’appelle DB_Loader.Instance.Load()");
            DB_Loader.Instance.Load();
            Debug.Log("[DisplayDataLoading] Après Load() → j’appelle Exit2()");
            Exit2();
            return;
        }
        loading = true;
        fillImage.sizeDelta = new Vector2(0f, background.sizeDelta.y);
        DB_Loader.Instance.DownloadCSVs();
        DB_Loader.Instance.onDownloadFinish += LoadData;
    }

    void LoadData() {
        Tween.Bounce(background.transform);
        fillImage.sizeDelta = new Vector2(background.sizeDelta.x * 0.89f, background.sizeDelta.y);
        DB_Loader.Instance.Load();
        loading = false;
        Invoke("Exit", 1f);
    }

    float timer = 0f;
    private void Update() {
        if (loading) {
            timer += Time.deltaTime;
            float w = background.sizeDelta.x * (timer/8f);
            w = Mathf.Clamp(w, 0f, background.sizeDelta.x);
            fillImage.sizeDelta = new Vector2(w, fillImage.sizeDelta.y);
        }
    }

    void Exit() {
        fillImage.sizeDelta = background.sizeDelta;
        Invoke("Exit2", 1f);
    }

    void Exit2() {
        Debug.Log("oui");
        FadeOut();
        SelectionMenu.Instance.FadeIn();
        DisplayMessage.Instance.Display(MissionIntroDisplay.Instance.gameIntroduction);
    }
}
