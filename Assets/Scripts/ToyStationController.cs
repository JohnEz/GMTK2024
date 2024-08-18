using UnityEngine;

[RequireComponent(typeof (TrackPieceController))]
public class ToyStationController : MonoBehaviour {
    private TrackPieceController _trackPieceController;

    void Awake() {
        _trackPieceController = GetComponent<TrackPieceController>();

        TrackPiecePlacementButtons trackPiecePlacementButtons = GetComponentInChildren<TrackPiecePlacementButtons>();
        if (trackPiecePlacementButtons != null) {
            trackPiecePlacementButtons.OnClick.AddListener(OnClickPlacement);
        }
    }

    void OnClickPlacement(Compass direction) {
        RouteManager.Instance.StartEditing(direction, _trackPieceController.TrackPiece);
    }
}
