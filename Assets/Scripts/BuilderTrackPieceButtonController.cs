using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuilderTrackPieceButtonController : MonoBehaviour
{
    [SerializeField]
    private CanvasGroup _button;

    [SerializeField]
    private Image _icon;

    [SerializeField]
    private CanvasGroup _lockedOverlay;

    [SerializeField]
    private TMP_Text _price;

    private TrackPieceOption _option;

    public TrackPieceOption Option {
        get => _option;
        set {
            _option = value;

            _icon.sprite = value.sprite;
            _price.text = value.price.ToString("N2");
            UpdateLocked();
        }
    }

    public event Action<TrackPieceOption> OnClick;

    public event Action<TrackPieceOption> OnHover;

    void Update() {
        UpdateLocked();
    }

    public void HandleClick() {
        OnClick?.Invoke(Option);
    }

    public void HandlePointerEnter() {
        if (Option.isLocked) {
            _lockedOverlay.alpha = 0.7f;
        } else {
            OnHover?.Invoke(Option);
        }
    }

    public void HandlePointerExit() {
        _lockedOverlay.alpha = 1;
    }

    private void UpdateLocked() {
        _lockedOverlay.gameObject.SetActive(Option.isLocked);
    }
}
