using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverMenu : MonoBehaviour {

    [SerializeField]
    private CanvasGroup _canvasGroup;

    [SerializeField]
    private TMP_Text successText;

    [SerializeField]
    private TMP_Text failureText;

    [SerializeField]
    private TMP_Text statusText;

    [SerializeField]
    private TMP_Text statsText;

    [SerializeField]
    private TMP_Text scoreText;

    [SerializeField]
    private Button playButton;

    [SerializeField]
    private Button exitButton;

    [SerializeField]
    private Button continueButton;

    [SerializeField]
    private Image Lenny;

    [SerializeField]
    private Image Terrence;

    public void Awake() {
        _canvasGroup = GetComponent<CanvasGroup>();
        _canvasGroup.alpha = 0f;
        _canvasGroup.blocksRaycasts = false;
    }

    public void Show(bool won, string station, decimal cash, int passengers, int coolpoints) {
        failureText.gameObject.SetActive(!won);
        successText.gameObject.SetActive(won);

        playButton.gameObject.SetActive(!won);
        exitButton.gameObject.SetActive(!won);
        continueButton.gameObject.SetActive(won);

        Lenny.gameObject.SetActive(won);
        Terrence.gameObject.SetActive(!won);

        statusText.text = won
            ? $"By creating a route to {station}, you finalised rail infrastructure connecting all the key regions of the Midlands' coal and steel industries."
            : $"{station} got overcrowded and Terrence was fired and broken down for parts.";
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
        SettingsManager.Instance.tutorialEnabled = false;
        SceneChanger.Instance.ChangeScene("Core");
    }

    public void Exit() {
        SceneChanger.Instance.ChangeScene("MainMenu");
    }
}