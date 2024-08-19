using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RouteBuilderManager : Singleton<RouteBuilderManager> {
    [SerializeField]
    private BuilderControlsController _builderControlsController;

    [SerializeField]
    private TrackPieceController _trackPreviewPrefab;

    private TrackPiece OriginStation;

    private TrackPiece TerminatingStation;

    private TrackPiece EditFromTrackPiece;

    private readonly List<(TrackPieceController controller, Compass direction)> PreviewTrackPieces = new();

    [SerializeField]
    private GhostTrackPiece GhostTrackPiece;

    void Awake() {
        _builderControlsController.OnHoverPiece += (template) => GhostTrackPiece.TrackPieceType = template.TrackPieceType;
        _builderControlsController.OnConfirmPiece += (template) => PlacePiece();
        _builderControlsController.OnUndoPiece += () => RemovePiece();
        _builderControlsController.OnClearRoute += () => StopEditing();
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            StopEditing();
        }
    }

    public void StartEditing(Compass direction, TrackPiece fromTrackPiece) {
        if (!GameStateManager.Instance.TrySetState(GameState.KidEditing)) {
            return;
        }

        OriginStation = fromTrackPiece;
        EditFromTrackPiece = OriginStation;
        GhostTrackPiece.gameObject.SetActive(true);
        GhostTrackPiece.SetPosition(direction, EditFromTrackPiece);
    }

    private void StopEditing() {
        if (!GameStateManager.Instance.TrySetState(GameState.Kid)) {
            return;
        }

        PreviewTrackPieces.ForEach(preview => Destroy(preview.controller.gameObject));
        PreviewTrackPieces.Clear();
        EditFromTrackPiece = null;
        OriginStation = null;
        TerminatingStation = null;
        GhostTrackPiece.gameObject.SetActive(false);
    }

    private void PlacePiece() {
        if (!GhostTrackPiece.IsValidPosition) {
            return;
        }

        TrackPiece piece = GhostTrackPiece.Position;
        Compass direction = GhostTrackPiece.Direction;

        TrackPieceController newTrack = Instantiate(_trackPreviewPrefab, transform);
        newTrack.TrackPiece = piece;
        newTrack.GetComponentInChildren<SpriteRenderer>().sprite = ToyMapManager.Instance.TrackPieceConfig[piece.Template.TrackPieceType].sprite;
        PreviewTrackPieces.Add((newTrack, direction));

        TerminatingStation = StationManager.Instance.GetConnectingStation(piece);

        if (TerminatingStation != null) {
            CommitRoute();
            StopEditing();
            Debug.Log("Route made!");
            return;
        }

        // Big assumption that there's only ever two connections, and the second one is the "exit" or "forward" connector
        Compass nextDirection = piece.Template.ConnectionPoints[1];
        EditFromTrackPiece = piece;
        GhostTrackPiece.SetPosition(nextDirection, piece);
    }

    private void CommitRoute() {
        Route route = new() {
            StartStation = OriginStation,
            EndStation = TerminatingStation,
        };

        PreviewTrackPieces.ForEach(preview => {
            route.AddConnection(preview.controller.TrackPiece, preview.direction);
        });

        RouteManager.Instance.AddRoute(route);

        ShowRouteScores();
    }

    private void ShowRouteScores() {
        List<TrackPiece> trackPieces = PreviewTrackPieces.Select(preview => preview.controller.TrackPiece).ToList();
        (int totalCool, List<(TrackPiece piece, int value)> coolnesses) = CoolManager.Instance.ScoreRoute(trackPieces);

        // Max time to show score should be about 2 seconds,
        // ideally about 0.3 seconds between each
        float delayPerScore = Mathf.Min(2.0f / coolnesses.Count, 0.3f);

        for (int i = 0; i < coolnesses.Count; i++) {
            (TrackPiece piece, int value) = coolnesses[i];

            TrackPieceController scoringTrackPiece = ToyMapManager.Instance.FindTrackPiece(piece);
            if (scoringTrackPiece != null) {
                FloatingTextManager.Instance.Show($"+{value}", scoringTrackPiece.gameObject, delayPerScore * i);
            }
        }

        TrackPieceController endStation = ToyMapManager.Instance.FindTrackPiece(TerminatingStation);
        FloatingTextManager.Instance.Show($"+{totalCool}", endStation.gameObject, delayPerScore * coolnesses.Count);
    }

    private void RemovePiece() {
        if (PreviewTrackPieces.Count == 0) {
            StopEditing();
            Debug.Log("Route cancelled!");
            return;
        }

        (TrackPieceController controller, Compass direction) = PreviewTrackPieces[^1];

        Destroy(controller.gameObject);
        PreviewTrackPieces.RemoveAt(PreviewTrackPieces.Count - 1);

        EditFromTrackPiece = PreviewTrackPieces.Count == 0 ? OriginStation : PreviewTrackPieces[^1].controller.TrackPiece;

        GhostTrackPiece.SetPosition(direction, EditFromTrackPiece);
    }
}