using UnityEngine;

[RequireComponent(typeof (TrackPieceController))]
public class Station : MonoBehaviour {
    private TrackPieceController _trackPieceController;

    public TrackTemplate stationTemplate;

    void Awake() {
        _trackPieceController = GetComponent<TrackPieceController>();
        _trackPieceController.TrackPiece = new TrackPiece() { X = 0, Y = 0, Template = stationTemplate };

        TrackPiecePlacementButtons trackPiecePlacementButtons = GetComponentInChildren<TrackPiecePlacementButtons>();
        if (trackPiecePlacementButtons != null) {
            trackPiecePlacementButtons.OnClick.AddListener(OnClickPlacement);
        }
    }

    void OnClickPlacement(Compass direction) {
        RouteManager.Instance.StartEditing(direction, _trackPieceController.TrackPiece);
    }
}
