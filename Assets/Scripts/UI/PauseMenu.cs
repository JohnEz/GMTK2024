using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : Singleton<PauseMenu> {
    private CanvasGroup _canvasGroup;

    private void Awake() {
        _canvasGroup = GetComponent<CanvasGroup>();
        _canvasGroup.alpha = 0;
        _canvasGroup.blocksRaycasts = false;
    }

    public void Pause() {
        Show();
        Time.timeScale = 0;
    }

    public void ContinueGame() {
        Hide();
        Time.timeScale = 1;
    }

    public void Restart() {
        Time.timeScale = 1;
        SceneChanger.Instance.ChangeScene("Core");
    }

    public void Exit() {
        Time.timeScale = 1;
        SceneChanger.Instance.ChangeScene("MainMenu");
    }

    private void Show() {
        _canvasGroup.DOFade(1f, .75f).SetUpdate(true);
        _canvasGroup.blocksRaycasts = true;
    }

    private void Hide() {
        _canvasGroup.DOFade(0f, .75f).SetUpdate(true);
        _canvasGroup.blocksRaycasts = false;
    }
}