using UnityEngine;

[RequireComponent(typeof (TrackPieceController))]
public class ToyStationController : MonoBehaviour {
    private TrackPieceController _trackPieceController;

    private TrackPiecePlacementButtons _trackPiecePlacementButtons;

    void Awake() {
        _trackPieceController = GetComponent<TrackPieceController>();

        _trackPiecePlacementButtons = GetComponentInChildren<TrackPiecePlacementButtons>();
        if (_trackPiecePlacementButtons != null) {
            _trackPiecePlacementButtons.OnClick.AddListener(OnClickPlacement);

            GameStateManager.Instance.OnStateChange.AddListener(OnGameStateChange);
            OnGameStateChange(GameStateManager.Instance.State);
        }
    }

    void OnClickPlacement(Compass direction) {
        RouteManager.Instance.StartEditing(direction, _trackPieceController.TrackPiece);
    }

    void OnGameStateChange(GameState state) {
        _trackPiecePlacementButtons.gameObject.SetActive(state != GameState.KidEditing);
    }
}
