using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TrackPiecePlacementButtons : MonoBehaviour
{
    private TrackPieceController _trackPieceController;

    private Button[] _buttons;

    private readonly Compass[] _dirtyAssumptionButtonOrder = {
        Compass.North,
        Compass.East,
        Compass.South,
        Compass.West
    };

    public UnityEvent<Compass> OnClick = new();

    void Awake() {
        _trackPieceController = GetComponentInParent<TrackPieceController>();
        if (_trackPieceController == null) {
            throw new Exception("No parent TrackPieceController found");
        }

        _trackPieceController.OnTrackPieceSet.AddListener(OnTrackPieceSet);
        InitialiseButtons();
    }

    private void InitialiseButtons() {
        _buttons = GetComponentsInChildren<Button>();

        for (int i = 0; i < _buttons.Length; i++) {
            _buttons[i].onClick.AddListener(() => HandleButtonClick(_dirtyAssumptionButtonOrder[i]));
        }
    }

    private void HandleButtonClick(Compass direction) {
        OnClick.Invoke(direction);
    }

    private void OnTrackPieceSet(TrackPiece trackPiece)
    {
        if (trackPiece == null) {
            foreach (Button button in _buttons) {
                button.gameObject.SetActive(false);
            }
            return;
        }

        Compass[] connections = trackPiece.Template.ConnectionPoints;

        for (int i = 0; i < _buttons.Length; i++) {
            _buttons[i].gameObject.SetActive(connections.Contains(_dirtyAssumptionButtonOrder[i]));
        }
    }
}
