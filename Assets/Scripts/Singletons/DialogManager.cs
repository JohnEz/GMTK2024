using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum Speaker {
    Lenny,
    Terrence
}

public class DialogManager : Singleton<DialogManager> {

    [SerializeField]
    private HideableUIComponent lenny;

    [SerializeField]
    private HideableUIComponent terrence;

    [SerializeField]
    private CanvasGroup dialogBox;

    [SerializeField]
    private Image dialogBackground;

    [SerializeField]
    private TMP_Text dialogText;

    [SerializeField]
    private Color lennyColor = Color.black;

    [SerializeField]
    private TMP_FontAsset lennyFont;

    [SerializeField]
    private Color terrenceColor = Color.black;

    [SerializeField]
    private TMP_FontAsset terranceFont;

    private void Start() {
        DisplayDialog(Speaker.Lenny, "Welcome to Train Set Go!");
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.R)) {
            DisplayDialog(Speaker.Terrence, "This is some debug dialog i hope we dont leave in!!");
        }
    }

    public void DisplayDialog(Speaker speaker, string dialog) {
        if (speaker == Speaker.Lenny) {
            terrence.Hide();
            lenny.Show();
            dialogText.font = lennyFont;
            dialogBackground.color = lennyColor;
        } else {
            lenny.Hide();
            terrence.Show();
            dialogText.font = terranceFont;
            dialogBackground.color = terrenceColor;
        }

        dialogText.text = dialog;

        dialogBox.DOFade(1f, .2f).SetDelay(lenny.Duration);
    }

    public void HideDialog() {
        dialogBox.DOFade(0f, .2f);

        lenny.Hide();
        terrence.Hide();
    }
}