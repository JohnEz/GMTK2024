using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct TrackPieceOption {
    public Sprite sprite;

    public TrackTemplate template;

    public bool isLocked;
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

    void Awake() {
        _options = new List<TrackPieceOption>(_initialOptions);
        _options.ForEach(AddOption);
    }

    private void AddOption(TrackPieceOption option) {
        BuilderTrackPieceButtonController button = Instantiate(_buttonPrefab, _buttonContainer.transform);
        button.SetOption(option);

        button.OnClick += (template) => OnConfirmPiece?.Invoke(template);
        button.OnHover += (template) => OnHoverPiece?.Invoke(template);
    }
}
