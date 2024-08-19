using System;
using UnityEngine;
using UnityEngine.UI;

public class BuilderTrackPieceButtonController : MonoBehaviour
{
    [SerializeField]
    private Image _icon;

    [SerializeField]
    private GameObject _lockedOverlay;

    public TrackPieceOption Option {
        get;
        private set;
    }

    public event Action<TrackTemplate> OnClick;

    public event Action<TrackTemplate> OnHover;

    public void SetOption(TrackPieceOption option)
    {
        Option = option;

        _icon.sprite = option.sprite;
        _lockedOverlay.SetActive(option.isLocked);
    }

    public void HandleClick() {
        if (!Option.isLocked) {
            OnClick?.Invoke(Option.template);
        }
    }

    public void HandlePointerEnter() {
        if (!Option.isLocked) {
            OnHover?.Invoke(Option.template);
        }
    }
}
