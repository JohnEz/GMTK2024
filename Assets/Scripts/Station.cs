using UnityEngine;

[RequireComponent(typeof (TrackPieceController))]
public class Station : MonoBehaviour {
    void Awake() {
        TrackPiecePlacementButtons trackPiecePlacementButtons = GetComponentInChildren<TrackPiecePlacementButtons>();
        if (trackPiecePlacementButtons != null) {
            trackPiecePlacementButtons.OnClick.AddListener(OnClickPlacement);
        }
    }

    void OnClickPlacement(Compass direction) {
        RouteManager.Instance.StartEditing(this, direction);
    }
}
