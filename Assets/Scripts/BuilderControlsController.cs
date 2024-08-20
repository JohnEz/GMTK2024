using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TrackPieceOption {
    public Sprite sprite;

    public TrackTemplate template;

    public bool isLocked;

    public float unlockPrice;
}

public class BuilderControlsController : MonoBehaviour
{
    [SerializeField]
    private List<TrackPieceOption> _initialOptions = new();

    private List<TrackPieceOption> _options = new();

    [SerializeField]
    private GameObject _buttonContainer;

    [SerializeField]
    private BuilderTrackPieceButtonController _buttonPrefab;

    public event Action<TrackTemplate> OnHoverPiece;

    public event Action<TrackTemplate> OnConfirmPiece;

    public event Action OnUndoPiece;

    public event Action OnClearRoute;

    private bool _enabled = false;

    void Awake() {
        GameStateManager.Instance.OnStateChange += OnGameStateChanged;
        OnGameStateChanged(GameStateManager.Instance.State);

        _options = new List<TrackPieceOption>(_initialOptions);
        _options.ForEach(AddOption);
    }

    private void AddOption(TrackPieceOption option) {
        BuilderTrackPieceButtonController button = Instantiate(_buttonPrefab, _buttonContainer.transform);
        button.Option = option;

        button.OnClick += (option) => OnConfirmPiece?.Invoke(option.template);
        button.OnHover += (option) => OnHoverPiece?.Invoke(option.template);
    }

    public void OnGameStateChanged(GameState state) {
        _enabled = state == GameState.KidEditing;
    }

    public void HandleUndoPiece() {
        if (_enabled) {
            OnUndoPiece?.Invoke();
        }
    }

    public void HandleClearRoute() {
        if (_enabled) {
            OnClearRoute?.Invoke();
        }
    }
}
