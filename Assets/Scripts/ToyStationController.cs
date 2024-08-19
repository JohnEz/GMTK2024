using UnityEngine;

[RequireComponent(typeof (TrackPieceController))]
public class ToyStationController : MonoBehaviour {
    private TrackPieceController _trackPieceController;

    private RouteBuildStartButtonsController _routeBuildStartButtons;

    void Awake() {
        _trackPieceController = GetComponent<TrackPieceController>();

        _routeBuildStartButtons = GetComponentInChildren<RouteBuildStartButtonsController>();
        if (_routeBuildStartButtons != null) {
            _routeBuildStartButtons.OnClick.AddListener(OnClickPlacement);

            GameStateManager.Instance.OnStateChange += OnGameStateChange;
            OnGameStateChange(GameStateManager.Instance.State);
        }
    }

    void OnClickPlacement(Compass direction) {
        RouteBuilderManager.Instance.StartEditing(direction, _trackPieceController.TrackPiece);
    }

    void OnGameStateChange(GameState state) {
        _routeBuildStartButtons.gameObject.SetActive(state != GameState.KidEditing);
    }
}
