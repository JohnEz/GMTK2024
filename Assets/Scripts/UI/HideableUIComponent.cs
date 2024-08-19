using DG.Tweening;
using UnityEngine;

public class HideableUIComponent : MonoBehaviour {

    [SerializeField]
    private Vector2 hiddenPosition;

    [SerializeField]
    private bool startHidden;

    private Vector2 shownPosition;

    [SerializeField]
    private float duration = .75f;

    private bool _isHidden;
    public bool IsHidden { get => _isHidden; }

    private RectTransform rectTransform;

    [SerializeField]
    private AudioClip _enterSound;

    [SerializeField]
    private AudioClip _exitSound;

    public void Awake() {
        rectTransform = GetComponent<RectTransform>();
        shownPosition = rectTransform.anchoredPosition;

        _isHidden = startHidden;
        if (startHidden) {
            rectTransform.anchoredPosition = hiddenPosition;
        }
    }

    public void Toggle() {
        if (_isHidden) {
            Show();
        } else {
            Hide();
        }
    }

    public void Show() {
        if (!_isHidden) {
            return;
        }

        _isHidden = false;

        rectTransform.DOAnchorPos(shownPosition, duration).SetEase(Ease.OutSine);

        // TODO why do i get an error without this check, the underlying method checks for null
        if (_enterSound) {
            AudioManager.Instance.PlaySound(_enterSound);
        }
    }

    public void Hide() {
        if (_isHidden) {
            return;
        }

        _isHidden = true;

        rectTransform.DOAnchorPos(hiddenPosition, duration).SetEase(Ease.InSine);

        if (_exitSound) {
            AudioManager.Instance.PlaySound(_exitSound);
        }
    }
}