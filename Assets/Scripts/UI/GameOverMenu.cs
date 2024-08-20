using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverMenu : MonoBehaviour {

    [SerializeField]
    private CanvasGroup _canvasGroup;

    [SerializeField]
    private TMP_Text failureText;

    [SerializeField]
    private TMP_Text statsText;

    [SerializeField]
    private TMP_Text scoreText;

    public void Awake() {
        _canvasGroup = GetComponent<CanvasGroup>();
        _canvasGroup.alpha = 0f;
        _canvasGroup.blocksRaycasts = false;
    }

    public void Show(string station, decimal cash, int passengers, int coolpoints) {
        failureText.text = $"{station} got overcrowded and Terrence was fired and broken down for parts.";
        statsText.text = $"You earned £{cash} and delivered {passengers} passengers";
        scoreText.text = $"but who cares because you got {coolpoints} cool points!!";

        _canvasGroup.DOFade(1f, .75f);
        _canvasGroup.blocksRaycasts = true;
    }

    public void Hide() {
        _canvasGroup.DOFade(0f, .75f);
        _canvasGroup.blocksRaycasts = false;
    }

    public void PlayAgain() {
        SceneChanger.Instance.ChangeScene("Core");
    }

    public void Exit() {
        SceneChanger.Instance.ChangeScene("MainMenu");
    }
}