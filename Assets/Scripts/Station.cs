using UnityEngine;

[RequireComponent(typeof (TrackPieceController))]
public class Station : MonoBehaviour {
    public TrackTemplate stationTemplate;

    void Awake() {
        TrackPieceController trackPieceController = GetComponent<TrackPieceController>();
        trackPieceController.TrackPiece = new TrackPiece() { X = 0, Y = 0, Template = stationTemplate };

        TrackPiecePlacementButtons trackPiecePlacementButtons = GetComponentInChildren<TrackPiecePlacementButtons>();
        if (trackPiecePlacementButtons != null) {
            trackPiecePlacementButtons.OnClick.AddListener(OnClickPlacement);
        }
    }

    void OnClickPlacement(Compass direction) {
        RouteManager.Instance.StartEditing(this, direction);
    }
}
