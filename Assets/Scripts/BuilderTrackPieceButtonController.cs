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
            _price.text = value.unlockPrice.ToString("N2");
            UpdateLocked();
        }
    }

    public event Action<TrackPieceOption> OnClick;

    public event Action<TrackPieceOption> OnHover;

    private bool _enabled = false;

    void Awake() {
        GameStateManager.Instance.OnStateChange += OnGameStateChanged;
        OnGameStateChanged(GameStateManager.Instance.State);
    }

    public void OnGameStateChanged(GameState state) {
        _enabled = state == GameState.KidEditing;
    }

    public void HandleClick() {
        if (!_enabled) {
            return;
        }

        if (!Option.isLocked) {
            if (BankManager.Instance.Spend(Option.placePrice)) {
                OnClick?.Invoke(Option);
            } else {
                HandleBegging();
            }

            return;
        }

        Debug.Log($"Trying to spend £{Option.unlockPrice} on {Option.template.TrackPieceType}");
        if (BankManager.Instance.Spend(Option.unlockPrice)) {
            Option.isLocked = false;
            UpdateLocked();
        } else {
            HandleBegging();
        }
    }

    public void HandlePointerEnter() {
        if (Option.isLocked) {
            _lockedOverlay.alpha = 0.7f;
        } else if (_enabled) {
            OnHover?.Invoke(Option);
        }
    }

    public void HandlePointerExit() {
        _lockedOverlay.alpha = 1;
    }

    private void UpdateLocked() {
        _lockedOverlay.gameObject.SetActive(Option.isLocked);
    }

    private void HandleBegging() {
        UIFloatingTextManager.Instance.Show("Not enough\ncash!", gameObject);
    }
}
